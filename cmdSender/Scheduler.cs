using System;
using System.Threading;
using System.Threading.Tasks;

namespace CmdSender
{
    /// <summary>
    /// 发送方式枚举。
    /// </summary>
    public enum SendMethod
    {
        /// <summary>后台 PostMessage WM_CHAR</summary>
        PostMessage = 0,
        /// <summary>前台 SetForegroundWindow + SendInput 模拟键盘</summary>
        SendInput = 1
    }

    /// <summary>
    /// 循环发送配置参数。
    /// </summary>
    public class SchedulerConfig
    {
        /// <summary>行间间隔（毫秒）</summary>
        public int LineInterval { get; set; }
        /// <summary>循环间隔（毫秒）</summary>
        public int CycleInterval { get; set; }
        /// <summary>循环次数（0 = 无限循环）</summary>
        public int CycleCount { get; set; }
        /// <summary>每条命令后是否发送回车键</summary>
        public bool SendEnter { get; set; }
        /// <summary>发送方式</summary>
        public SendMethod Method { get; set; }
    }

    /// <summary>
    /// 命令发送事件参数。
    /// </summary>
    public class CommandSentEventArgs : EventArgs
    {
        /// <summary>当前行号（1-based）</summary>
        public int LineNumber { get; set; }
        /// <summary>总行数</summary>
        public int TotalLines { get; set; }
        /// <summary>当前轮次（1-based）</summary>
        public int CycleNumber { get; set; }
        /// <summary>总轮次（0 = 无限）</summary>
        public int TotalCycles { get; set; }
        /// <summary>发送的命令内容</summary>
        public string Command { get; set; }
        /// <summary>累计已发送条数</summary>
        public long TotalSent { get; set; }
        /// <summary>预计剩余秒数（无限循环时为 null）</summary>
        public int? EstimatedRemainingSeconds { get; set; }
    }

    /// <summary>
    /// 命令调度器。基于 async/await 实现行间间隔与循环间隔的精确控制。
    /// 循环主体在后台线程（ThreadPool）执行，不阻塞 UI 线程；
    /// 事件在后台线程触发，UI 侧负责跨线程封送。
    /// </summary>
    public class Scheduler
    {
        private CancellationTokenSource _cts;
        private Task _task;
        private IntPtr _targetHandle;
        private string[] _commands;
        private SchedulerConfig _config;
        private int _generation;

        /// <summary>每条命令发送后触发（后台线程）</summary>
        public event EventHandler<CommandSentEventArgs> OnCommandSent;
        /// <summary>状态变更时触发（后台线程）</summary>
        public event EventHandler<string> OnStatusChanged;
        /// <summary>发送完成（正常结束或取消）时触发（后台线程）</summary>
        public event EventHandler OnCompleted;

        /// <summary>调度器是否正在运行</summary>
        public bool IsRunning => _task != null && !_task.IsCompleted;

        /// <summary>
        /// 启动循环发送。立即返回，循环在后台线程执行。
        /// </summary>
        public void Start(IntPtr handle, string[] commands, SchedulerConfig config)
        {
            Stop();

            _targetHandle = handle;
            _commands = commands ?? new string[0];
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _generation++;
            int gen = _generation;

            _cts = new CancellationTokenSource();
            // Task.Run: 循环主体在线程池执行，await 后续代码不再回到 UI 上下文，
            // 避免 SendKeys(含 Sleep) 阻塞界面。
            _task = Task.Run(() => RunLoopAsync(_cts.Token, gen), CancellationToken.None);
        }

        /// <summary>
        /// 请求停止发送。不阻塞，循环将在最近的检查点退出。
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// 请求停止并等待任务结束（限时）。
        /// </summary>
        public void WaitForStop(int timeoutMs = 1000)
        {
            _cts?.Cancel();
            try
            {
                _task?.Wait(timeoutMs);
            }
            catch (AggregateException)
            {
                // 取消导致的异常忽略
            }
            catch (Exception)
            {
                // 忽略
            }
        }

        /// <summary>
        /// 异步发送循环主体。
        /// 使用 generation 机制确保只有最新一次 Start 的 OnCompleted 会被触发，
        /// 避免"停止→立即重启"时旧任务的完成事件干扰新任务。
        /// </summary>
        private async Task RunLoopAsync(CancellationToken token, int generation)
        {
            int cycleNumber = 0;
            long totalSent = 0;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    cycleNumber++;

                    for (int i = 0; i < _commands.Length; i++)
                    {
                        if (token.IsCancellationRequested) return;

                        // 检查目标窗口是否仍然有效
                        if (!CommandSender.IsWindowValid(_targetHandle))
                        {
                            OnStatusChanged?.Invoke(this, "目标窗口已关闭，停止发送");
                            return;
                        }

                        string cmd = _commands[i];
                        SendCommand(cmd);
                        totalSent++;

                        OnCommandSent?.Invoke(this, new CommandSentEventArgs
                        {
                            LineNumber = i + 1,
                            TotalLines = _commands.Length,
                            CycleNumber = cycleNumber,
                            TotalCycles = _config.CycleCount,
                            Command = cmd,
                            TotalSent = totalSent,
                            EstimatedRemainingSeconds = EstimateRemainingSeconds(cycleNumber, i)
                        });

                        // 行间等待（最后一行后不再等待行间隔）
                        if (i < _commands.Length - 1)
                        {
                            await Task.Delay(_config.LineInterval, token);
                        }
                    }

                    // 检查循环次数（0 = 无限）
                    if (_config.CycleCount > 0 && cycleNumber >= _config.CycleCount)
                    {
                        OnStatusChanged?.Invoke(this, $"发送完成，共 {cycleNumber} 轮 {totalSent} 条命令");
                        break;
                    }

                    // 循环间等待
                    await Task.Delay(_config.CycleInterval, token);
                }
            }
            catch (TaskCanceledException)
            {
                // 正常取消，不处理
            }
            catch (Exception ex)
            {
                try { OnStatusChanged?.Invoke(this, $"发送异常: {ex.Message}"); } catch { }
            }
            finally
            {
                // 仅最新 generation 的完成事件才会触发
                if (generation == _generation)
                {
                    try { OnCompleted?.Invoke(this, EventArgs.Empty); } catch { }
                }
            }
        }

        /// <summary>
        /// 估算剩余发送时间（秒）。无限循环返回 null。
        /// 每轮耗时 = (行数-1)*行间隔 + 循环间隔（最后一轮无循环间隔）。
        /// </summary>
        private int? EstimateRemainingSeconds(int cycleNumber, int lineIndex)
        {
            if (_config.CycleCount <= 0) return null;

            long rowTime = (_commands.Length - 1) * (long)_config.LineInterval;
            long linesLeftInCurrent = _commands.Length - (lineIndex + 1);
            long currentCycleRemaining = linesLeftInCurrent * (long)_config.LineInterval;

            long cyclesLeft = _config.CycleCount - cycleNumber;
            long futureMs = 0;
            if (cyclesLeft > 0)
            {
                futureMs = cyclesLeft * rowTime + (cyclesLeft - 1) * (long)_config.CycleInterval;
            }

            long totalMs = currentCycleRemaining + futureMs;
            return (int)Math.Ceiling(totalMs / 1000.0);
        }

        /// <summary>
        /// 根据配置的发送方式发送单条命令。
        /// </summary>
        private void SendCommand(string cmd)
        {
            try
            {
                if (_config.Method == SendMethod.SendInput)
                {
                    CommandSender.SendBySendInput(_targetHandle, cmd, _config.SendEnter);
                }
                else
                {
                    CommandSender.SendByPostMessage(_targetHandle, cmd, _config.SendEnter);
                }
            }
            catch (Exception ex)
            {
                OnStatusChanged?.Invoke(this, $"发送错误: {ex.Message}");
            }
        }
    }
}
