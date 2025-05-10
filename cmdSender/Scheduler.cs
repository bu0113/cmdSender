using System;
using System.Timers;
using System.Collections.Generic;

namespace CmdSender
{
    public class SchedulerConfig
    {
        public int LineInterval { get; set; }
        public int CycleInterval { get; set; }
        public int CycleCount { get; set; }
    }

    public class Scheduler
    {
        private Timer _lineTimer;
        private Timer _cycleTimer;
        private Queue<string> _commandQueue;
        private int _cycleCounter = 0;
        private IntPtr _targetHandle;
        private SchedulerConfig _config;

        public event EventHandler<int> OnCommandSent;
        public event EventHandler OnCompleted;

        public void Start(IntPtr handle, IEnumerable<string> commands, SchedulerConfig config)
        {
            _targetHandle = handle;
            _config = config;
            _commandQueue = new Queue<string>(commands);
            InitializeTimers();
            StartTimers();
        }

        private void InitializeTimers()
        {
            _lineTimer = new Timer(_config.LineInterval);
            _lineTimer.Elapsed += ProcessLine;

            _cycleTimer = new Timer(_config.CycleInterval);
            _cycleTimer.Elapsed += (s, e) => ResetQueue();
        }

        private void StartTimers()
        {
            _lineTimer.Start();
            if (_config.CycleInterval > 0) _cycleTimer.Start();
        }

        private void ProcessLine(object sender, ElapsedEventArgs e)
        {
            if (_commandQueue.Count == 0)
            {
                HandleEmptyQueue();
                return;
            }
            SendNextCommand();
        }

        private void HandleEmptyQueue()
        {
            if (ShouldContinueCycle()) ResetQueue();
            else Stop();
        }

        private bool ShouldContinueCycle()
        {
            return _config.CycleCount == 0 || ++_cycleCounter < _config.CycleCount;
        }

        private void SendNextCommand()
        {
            string cmd = _commandQueue.Dequeue();
            CommandSender.AppendText(_targetHandle, cmd);
            OnCommandSent?.Invoke(this, _cycleCounter);
        }

        private void ResetQueue()
        {
            _commandQueue = new Queue<string>(_commandQueue);
        }

        public void Stop()
        {
            _lineTimer?.Stop();
            _cycleTimer?.Stop();
            _cycleCounter = 0;
            OnCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}