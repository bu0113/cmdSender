using System;
using System.Threading;
using System.Windows.Forms;
using CmdSender;

namespace cmdSender
{
    internal static class Program
    {
        /// <summary>单实例互斥锁名</summary>
        private const string MutexName = "Global\\CmdSender_SingleInstance_7F3A9C2E";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createdNew;
            using (var mutex = new Mutex(true, MutexName, out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("cmdSender 已在运行，请勿重复启动。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
        }
    }
}
