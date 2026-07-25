using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
        private static extern bool SetForegroundWindow(IntPtr hWnd);

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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region 消息常量

        private const uint WM_CHAR = 0x0102;
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_SETFOCUS = 0x0007;
        private const int VK_RETURN = 0x0D;

        private const uint EM_GETTEXTLENGTH = 0x000E;
        private const uint EM_SETSEL = 0x00B1;
        private const uint EM_REPLACESEL = 0x00C2;

        private const uint CWP_SKIPINVISIBLE = 0x0001;

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
        /// 通过 PostMessage WM_CHAR 后台发送文本。
        /// 不激活目标窗口，直接向句柄投递字符消息。
        /// </summary>
        public static void SendByPostMessage(IntPtr hWnd, string text, bool sendEnter)
        {
            if (hWnd == IntPtr.Zero) return;

            // 给目标控件投递 WM_SETFOCUS，使其在无焦点状态下也能处理 WM_CHAR。
            // 许多 Edit/RichEdit 控件仅在拥有焦点时才处理键盘消息，否则消息会滞留队列
            // 直到窗口被激活。预先设置焦点可显著提升后台发送的兼容性。
            SendMessage(hWnd, WM_SETFOCUS, 0, 0);

            if (!string.IsNullOrEmpty(text))
            {
                foreach (char c in text)
                {
                    PostMessage(hWnd, WM_CHAR, (IntPtr)c, IntPtr.Zero);
                }
            }

            if (sendEnter)
            {
                SendEnterKey(hWnd);
            }
        }

        /// <summary>
        /// 通过 SetForegroundWindow + SendKeys 前台发送文本。
        /// 激活目标窗口后模拟键盘输入，兼容性更好（支持控制台等）。
        /// </summary>
        public static void SendBySendKeys(IntPtr hWnd, string text, bool sendEnter)
        {
            if (hWnd == IntPtr.Zero) return;

            ForceSetForegroundWindow(hWnd);
            Thread.Sleep(50);

            string sendKeysText = EscapeForSendKeys(text ?? "");
            if (sendEnter)
            {
                sendKeysText += "{ENTER}";
            }

            if (!string.IsNullOrEmpty(sendKeysText))
            {
                SendKeys.SendWait(sendKeysText);
            }
        }

        /// <summary>
        /// 向 Edit 控件追加文本（不发送回车）。
        /// </summary>
        public static void AppendText(IntPtr hWnd, string text)
        {
            if (hWnd == IntPtr.Zero || string.IsNullOrEmpty(text)) return;

            int textLength = SendMessage(hWnd, EM_GETTEXTLENGTH, 0, 0);
            SendMessage(hWnd, EM_SETSEL, textLength, textLength);
            SendMessage(hWnd, EM_REPLACESEL, (IntPtr)1, text);
        }

        #endregion

        #region 内部方法

        private static void SendEnterKey(IntPtr hWnd)
        {
            PostMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_RETURN, IntPtr.Zero);
            PostMessage(hWnd, WM_KEYUP, (IntPtr)VK_RETURN, IntPtr.Zero);
        }

        /// <summary>
        /// 强制将目标窗口置前。通过 AttachThreadInput 绕过前台窗口限制。
        /// </summary>
        private static void ForceSetForegroundWindow(IntPtr hWnd)
        {
            IntPtr foreWnd = GetForegroundWindow();
            uint procId;
            uint foreThread = GetWindowThreadProcessId(foreWnd, out procId);
            uint appThread = GetWindowThreadProcessId(hWnd, out procId);

            if (foreThread != appThread)
            {
                uint currentThread = GetCurrentThreadId();
                AttachThreadInput(foreThread, appThread, true);
                SetForegroundWindow(hWnd);
                AttachThreadInput(foreThread, appThread, false);
            }
            else
            {
                SetForegroundWindow(hWnd);
            }
        }

        /// <summary>
        /// 转义 SendKeys 特殊字符：+ ^ % ~ ( ) { }
        /// </summary>
        private static string EscapeForSendKeys(string text)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if ("+^%~(){}".IndexOf(c) >= 0)
                {
                    sb.Append('{');
                    sb.Append(c);
                    sb.Append('}');
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}
