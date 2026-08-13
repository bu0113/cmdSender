using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;
using System.Threading;

namespace CmdSender
{
    /// <summary>
    /// 窗口查找与命令发送的 Win32 互操作层。
    /// </summary>
    public static class CommandSender
    {
        #region Win32 API

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(POINT pt);

        [DllImport("user32.dll")]
        private static extern IntPtr ChildWindowFromPointEx(IntPtr hwndParent, POINT pt, uint uFlags);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowUnicode(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct INPUT
        {
            public uint type;
            public InputUnion U;
        }

        // 联合体必须包含全部三种输入结构，保证 Marshal.SizeOf(INPUT) 与
        // Win32 的 sizeof(INPUT) 一致（x64=40, x86=28），否则 SendInput 返回失败。
        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public MOUSEINPUT mi;
            [FieldOffset(0)] public KEYBDINPUT ki;
            [FieldOffset(0)] public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        #endregion

        #region 消息常量

        private const uint WM_CHAR = 0x0102;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_SETFOCUS = 0x0007;
        private const int VK_RETURN = 0x0D;

        private const uint INPUT_KEYBOARD = 1;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_UNICODE = 0x0004;

        private const uint EM_GETTEXTLENGTH = 0x000E;
        private const uint EM_SETSEL = 0x00B1;
        private const uint EM_REPLACESEL = 0x00C2;

        private const uint CWP_SKIPINVISIBLE = 0x0001;

        private const uint GA_ROOT = 2;

        #endregion

        #region 窗口查找

        /// <summary>
        /// 获取屏幕坐标处最深层可见子窗口的句柄。
        /// 先用 WindowFromPoint 获取顶层窗口，再递归调用 ChildWindowFromPointEx 向下查找。
        /// </summary>
        public static IntPtr GetWindowFromPoint(Point screenPoint)
        {
            POINT pt = new POINT { X = screenPoint.X, Y = screenPoint.Y };

            IntPtr hWnd = WindowFromPoint(pt);
            if (hWnd == IntPtr.Zero) return IntPtr.Zero;

            // 递归查找最深层子窗口
            IntPtr current = hWnd;
            while (true)
            {
                POINT clientPt = pt;
                ScreenToClient(current, ref clientPt);

                IntPtr child = ChildWindowFromPointEx(current, clientPt, CWP_SKIPINVISIBLE);

                if (child == IntPtr.Zero || child == current)
                    break;

                current = child;
            }

            return current;
        }

        /// <summary>
        /// 获取窗口标题文本。
        /// </summary>
        public static string GetWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return "";
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// 获取窗口类名。
        /// </summary>
        public static string GetWindowClassName(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return "";
            StringBuilder sb = new StringBuilder(256);
            GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// 检查窗口句柄是否仍然有效。
        /// </summary>
        public static bool IsWindowValid(IntPtr hWnd)
        {
            return hWnd != IntPtr.Zero && IsWindow(hWnd);
        }

        #endregion

        #region 发送方法

        /// <summary>
        /// 通过 PostMessage 后台发送文本，不抢焦点。
        /// 文本编辑类控件（Edit/RichEdit 等）走 EM_REPLACESEL 追加，
        /// 中文完美支持（Unicode 与 ANSI 控件通吃）；
        /// 其他控件按系统 ANSI 代码页（如 GBK）逐字节投递 WM_CHAR，
        /// 兼容现代记事本等按 ANSI 字节解释字符的窗口。
        /// 注意：控制台窗口、VSCode 等不使用 WM_CHAR 的应用请改用前台发送。
        /// </summary>
        public static void SendByPostMessage(IntPtr hWnd, string text, bool sendEnter)
        {
            if (hWnd == IntPtr.Zero) return;

            if (IsEditLikeControl(hWnd))
            {
                AppendByReplaceSel(hWnd, text, sendEnter);
                return;
            }

            // 非编辑类控件：WM_SETFOCUS 提升兼容性，再按 ANSI 字节投递 WM_CHAR
            SendMessage(hWnd, WM_SETFOCUS, 0, 0);

            if (!string.IsNullOrEmpty(text))
            {
                byte[] bytes = Encoding.Default.GetBytes(text);
                foreach (byte b in bytes)
                {
                    PostMessage(hWnd, WM_CHAR, (IntPtr)b, IntPtr.Zero);
                }
            }

            if (sendEnter)
            {
                SendEnterKey(hWnd);
            }
        }

        /// <summary>
        /// 通过 SetForegroundWindow + SendInput 前台发送文本。
        /// 先把句柄提升到顶层窗口并激活，再注入键盘输入（UNICODE 逐字符），
        /// 兼容控制台（cmd）、VSCode、终端等一切接受真实键盘输入的窗口。
        /// SendInput 无需 STA 线程或消息泵，可在后台线程安全调用。
        /// </summary>
        public static void SendBySendInput(IntPtr hWnd, string text, bool sendEnter)
        {
            if (hWnd == IntPtr.Zero) return;

            // SetForegroundWindow 需要顶层窗口句柄，先提升；最小化则先还原
            IntPtr top = GetAncestor(hWnd, GA_ROOT);
            if (top == IntPtr.Zero) top = hWnd;
            if (IsIconic(top)) ShowWindow(top, 9 /*SW_RESTORE*/);
            ForceSetForegroundWindow(top);

            // 轮询等待目标真正成为前台窗口，再注入输入，
            // 避免激活尚未完成时输入被投递到其他窗口
            IntPtr beforeFg = GetForegroundWindow();
            if (!WaitForeground(top, beforeFg, 1500))
            {
                throw new InvalidOperationException(
                    "无法将目标窗口置于前台，发送已取消（避免误输入到其他窗口）");
            }

            // 焦点就位等待
            Thread.Sleep(60);

            SendUnicodeText(text ?? "");

            if (sendEnter)
            {
                // 现代记事本等应用处理 Unicode 文本后需要短暂时间，立即注入回车可能被丢弃
                Thread.Sleep(40);
                SendEnterKeyInput();
            }
        }

        /// <summary>
        /// 向 Edit 控件追加文本（不发送回车）。
        /// </summary>
        public static void AppendText(IntPtr hWnd, string text)
        {
            AppendByReplaceSel(hWnd, text, false);
        }

        #endregion

        #region 内部方法

        /// <summary>
        /// 判断窗口是否为文本编辑类控件（支持 EM_REPLACESEL 追加）。
        /// 现代记事本 RichEditD2DPT 虽标记为 Unicode 窗口，但按 ANSI 解释 WM_CHAR，
        /// 因此编辑类控件统一走 EM_REPLACESEL 以获得正确的中文支持。
        /// </summary>
        private static bool IsEditLikeControl(IntPtr hWnd)
        {
            string cls = GetWindowClassName(hWnd);
            if (string.IsNullOrEmpty(cls)) return false;

            return cls.StartsWith("Edit", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("RichEdit", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("RICHEDIT", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("NotepadTextBox", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("WindowsForms10.EDIT", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("TEdit", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("TMemo", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("TRichEdit", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("Scintilla", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 通过 EM_REPLACESEL 在编辑控件末尾追加文本（Unicode 消息，中文无乱码）。
        /// </summary>
        private static void AppendByReplaceSel(IntPtr hWnd, string text, bool sendEnter)
        {
            if (hWnd == IntPtr.Zero) return;

            if (!string.IsNullOrEmpty(text))
            {
                int len = SendMessage(hWnd, EM_GETTEXTLENGTH, 0, 0);
                SendMessage(hWnd, EM_SETSEL, len, len);
                SendMessage(hWnd, EM_REPLACESEL, (IntPtr)1, text);
            }

            if (sendEnter)
            {
                int len = SendMessage(hWnd, EM_GETTEXTLENGTH, 0, 0);
                SendMessage(hWnd, EM_SETSEL, len, len);
                SendMessage(hWnd, EM_REPLACESEL, (IntPtr)1, "\r\n");
            }
        }

        private static void SendEnterKey(IntPtr hWnd)
        {
            PostMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_RETURN, IntPtr.Zero);
            PostMessage(hWnd, WM_KEYUP, (IntPtr)VK_RETURN, IntPtr.Zero);
        }

        /// <summary>
        /// 通过 SendInput UNICODE 逐字符注入文本。无特殊字符转义问题。
        /// 字符间加 15ms 间隔，避免现代记事本等应用在高速连续注入时丢字。
        /// </summary>
        private static void SendUnicodeText(string text)
        {
            foreach (char c in text)
            {
                INPUT[] inputs = new INPUT[2];

                inputs[0].type = INPUT_KEYBOARD;
                inputs[0].U.ki.wVk = 0;
                inputs[0].U.ki.wScan = c;
                inputs[0].U.ki.dwFlags = KEYEVENTF_UNICODE;
                inputs[0].U.ki.time = 0;
                inputs[0].U.ki.dwExtraInfo = IntPtr.Zero;

                inputs[1] = inputs[0];
                inputs[1].U.ki.dwFlags = KEYEVENTF_UNICODE | KEYEVENTF_KEYUP;

                InjectInputs(inputs);
                Thread.Sleep(15);
            }
        }

        /// <summary>
        /// 通过 SendInput 发送回车键。
        /// </summary>
        private static void SendEnterKeyInput()
        {
            INPUT[] inputs = new INPUT[2];

            inputs[0].type = INPUT_KEYBOARD;
            inputs[0].U.ki.wVk = VK_RETURN;
            inputs[0].U.ki.wScan = 0;
            inputs[0].U.ki.dwFlags = 0;
            inputs[0].U.ki.time = 0;
            inputs[0].U.ki.dwExtraInfo = IntPtr.Zero;

            inputs[1] = inputs[0];
            inputs[1].U.ki.dwFlags = KEYEVENTF_KEYUP;

            InjectInputs(inputs);
        }

        /// <summary>
        /// 调用 SendInput 并校验注入结果，失败时抛出带错误码的异常。
        /// </summary>
        private static void InjectInputs(INPUT[] inputs)
        {
            uint sent = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
            if (sent != (uint)inputs.Length)
            {
                int error = Marshal.GetLastWin32Error();
                throw new InvalidOperationException(
                    $"SendInput 注入失败（错误码 {error}，成功 {sent}/{inputs.Length} 个事件）");
            }
        }

        /// <summary>
        /// 轮询等待目标窗口成为前台窗口，超时返回 false。
        /// 控制台窗口（cmd 等）由宿主进程（Windows Terminal/conhost）托管：
        /// - 目标与前台均为控制台类窗口 → 视为激活成功（宿主窗口即目标）
        /// - 前台因本次激活发生变化且与目标同进程 → 视为激活成功
        /// 其余情况要求前台窗口句柄与目标一致，避免激活失败时输入投递到错误窗口。
        /// </summary>
        private static bool WaitForeground(IntPtr hWnd, IntPtr before, int timeoutMs)
        {
            int waited = 0;
            while (waited < timeoutMs)
            {
                IntPtr fg = GetForegroundWindow();
                if (fg == hWnd) return true;
                if (IsConsoleClass(hWnd) && IsConsoleClass(fg)) return true;
                if (fg != before && IsSameProcess(fg, hWnd)) return true;
                Thread.Sleep(20);
                waited += 20;
            }

            IntPtr last = GetForegroundWindow();
            return last == hWnd
                || (IsConsoleClass(hWnd) && IsConsoleClass(last))
                || (last != before && IsSameProcess(last, hWnd));
        }

        /// <summary>
        /// 判断两个窗口是否属于同一进程。
        /// </summary>
        private static bool IsSameProcess(IntPtr a, IntPtr b)
        {
            if (a == IntPtr.Zero || b == IntPtr.Zero) return false;
            uint pidA, pidB;
            GetWindowThreadProcessId(a, out pidA);
            GetWindowThreadProcessId(b, out pidB);
            return pidA == pidB;
        }

        /// <summary>
        /// 判断窗口是否为控制台类窗口（控制台宿主托管，前台可能是宿主窗口）。
        /// </summary>
        private static bool IsConsoleClass(IntPtr hWnd)
        {
            string cls = GetWindowClassName(hWnd);
            return cls.Equals("ConsoleWindowClass", StringComparison.OrdinalIgnoreCase)
                || cls.Equals("PseudoConsoleWindow", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("CASCADIA", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("ConEmu", StringComparison.OrdinalIgnoreCase)
                || cls.StartsWith("mintty", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 强制将目标窗口置前。经典方案：把前台窗口线程的输入队列挂到目标窗口线程，
        /// 使目标线程获得"最近输入"资格，从而允许 SetForegroundWindow 绕过前台锁；
        /// 并验证激活结果、必要时重试一次。
        /// </summary>
        private static void ForceSetForegroundWindow(IntPtr hWnd)
        {
            // 目标已在前台则无需处理
            if (GetForegroundWindow() == hWnd) return;

            IntPtr foreWnd = GetForegroundWindow();
            uint procId;
            uint foreThread = GetWindowThreadProcessId(foreWnd, out procId);
            uint appThread = GetWindowThreadProcessId(hWnd, out procId);

            bool attached = false;
            if (foreThread != appThread)
            {
                attached = AttachThreadInput(foreThread, appThread, true);
            }
            try
            {
                SetForegroundWindow(hWnd);
                Thread.Sleep(30);
                if (GetForegroundWindow() != hWnd)
                {
                    // 兜底重试一次（某些窗口激活较慢或首次被拒）
                    SetForegroundWindow(hWnd);
                }
            }
            finally
            {
                if (attached) AttachThreadInput(foreThread, appThread, false);
            }
        }

        #endregion
    }
}
