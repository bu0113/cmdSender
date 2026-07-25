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
        /// <summary>前台 SetForegroundWindow + SendKeys</summary>
        SendKeys = 1
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
    }

    /// <summary>
    /// 命令调度器。基于 async/await 实现行间间隔与循环间隔的精确控制。
    /// </summary>
    public class Scheduler
    {
        private CancellationTokenSource _cts;
        private Task _task;
        private IntPtr _targetHandle;
        private string[] _commands;
        private SchedulerConfig _config;
        private int _generation;

        /// <summary>每条命令发送后触发</summary>
        public event EventHandler<CommandSentEventArgs> OnCommandSent;
        /// <summary>状态变更时触发</summary>
        public event EventHandler<string> OnStatusChanged;
        /// <summary>发送完成（正常结束或取消）时触发</summary>
        public event EventHandler OnCompleted;

        /// <summary>调度器是否正在运行</summary>
        public bool IsRunning => _task != null && !_task.IsCompleted;

        /// <summary>
        /// 启动循环发送。
        /// </summary>
        public void Start(IntPtr handle, string[] commands, SchedulerConfig config)
        {
            Stop();

            _targetHandle = handle;
            _commands = commands;
            _config = config;
            _generation++;
            int gen = _generation;

            _cts = new CancellationTokenSource();
            _task = RunLoopAsync(_cts.Token, gen);
        }

        /// <summary>
        /// 停止发送。
        /// </summary>
        public void Stop()
        {
            _cts?.Cancel();
        }

        /// <summary>
        /// 异步发送循环主体。
        /// 使用 generation 机制确保只有最新一次 Start 的 OnCompleted 会被触发，
        /// 避免"停止→立即重启"时旧任务的完成事件干扰新任务。
        /// </summary>
        private async Task RunLoopAsync(CancellationToken token, int generation)
        {
            int cycleNumber = 0;

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

                        OnCommandSent?.Invoke(this, new CommandSentEventArgs
                        {
                            LineNumber = i + 1,
                            TotalLines = _commands.Length,
                            CycleNumber = cycleNumber,
                            TotalCycles = _config.CycleCount,
                            Command = cmd
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
                        OnStatusChanged?.Invoke(this, $"发送完成，共 {cycleNumber} 轮");
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
                OnStatusChanged?.Invoke(this, $"发送异常: {ex.Message}");
            }
            finally
            {
                // 仅最新 generation 的完成事件才会触发
                if (generation == _generation)
                {
                    OnCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// 根据配置的发送方式发送单条命令。
        /// </summary>
        private void SendCommand(string cmd)
        {
            try
            {
                if (_config.Method == SendMethod.SendKeys)
                {
                    CommandSender.SendBySendKeys(_targetHandle, cmd, _config.SendEnter);
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
