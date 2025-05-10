using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace CmdSender
{
    public static class CommandSender
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, string lParam);

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

        public static void AppendText(IntPtr hWnd, string text)
        {
            const uint WM_KEYDOWN = 0x0100;
            const uint VK_END = 0x23; // End键
            const uint EM_SETSEL = 0x00B1;
            const uint EM_REPLACESEL = 0x00C2;

            // 移动光标到末尾
            SendMessage(hWnd, WM_KEYDOWN, (IntPtr)VK_END, IntPtr.Zero);

            // 设置选中范围为末尾
            SendMessage(hWnd, EM_SETSEL, (IntPtr)(-1), (IntPtr)(-1));

            // 插入文本
            SendMessage(hWnd, EM_REPLACESEL, IntPtr.Zero, text + "\r\n");
        }
    }
}