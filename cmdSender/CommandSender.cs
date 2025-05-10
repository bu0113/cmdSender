using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;

namespace CmdSender
{
    public static class CommandSender
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static IntPtr GetWindowFromPoint(Point screenPoint)
        {
            return WindowFromPoint(screenPoint);
        }

        public static void SendText(IntPtr hWnd, string text)
        {
            if (hWnd == IntPtr.Zero) return;

            const uint WM_SETTEXT = 0x000C;
            SetForegroundWindow(hWnd);
            SendMessage(hWnd, WM_SETTEXT, IntPtr.Zero, text);
        }

        //追加文本
        public static void AppendText(IntPtr hWnd, string text)
        {
            const uint EM_GETTEXTLENGTH = 0x000E;
            const uint EM_SETSEL = 0x00B1;
            const uint EM_REPLACESEL = 0x00C2;

            // 获取当前文本长度
            int textLength = SendMessage(hWnd, EM_GETTEXTLENGTH, 0, 0);

            // 移动光标到文本末尾
            SendMessage(hWnd, EM_SETSEL, textLength, textLength);

            // 插入新文本（使用fCanUndo=1允许撤销）
            SendMessage(hWnd, EM_REPLACESEL, (IntPtr)1, text);
        }
    }
}