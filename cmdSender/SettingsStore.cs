using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CmdSender
{
    /// <summary>
    /// 应用设置（会话记忆），序列化到 %APPDATA%\CmdSender\settings.json。
    /// </summary>
    [DataContract]
    public class AppSettings
    {
        /// <summary>最后打开的文件路径</summary>
        [DataMember] public string LastFile { get; set; }

        /// <summary>最后使用的文件目录</summary>
        [DataMember] public string LastDirectory { get; set; }

        /// <summary>行间隔（ms）</summary>
        [DataMember] public int LineInterval { get; set; } = 100;

        /// <summary>循环间隔（ms）</summary>
        [DataMember] public int CycleInterval { get; set; } = 2000;

        /// <summary>循环次数（0 = 无限）</summary>
        [DataMember] public int CycleCount { get; set; } = 0;

        /// <summary>发送后回车</summary>
        [DataMember] public bool SendEnter { get; set; } = true;

        /// <summary>发送方式（0=PostMessage, 1=SendKeys）</summary>
        [DataMember] public int SendMethod { get; set; } = 1;

        /// <summary>窗口位置 X（-1 = 未记忆）</summary>
        [DataMember] public int WindowX { get; set; } = -1;

        /// <summary>窗口位置 Y（-1 = 未记忆）</summary>
        [DataMember] public int WindowY { get; set; } = -1;

        /// <summary>窗口宽度</summary>
        [DataMember] public int WindowWidth { get; set; }

        /// <summary>窗口高度</summary>
        [DataMember] public int WindowHeight { get; set; }

        /// <summary>是否最大化</summary>
        [DataMember] public bool Maximized { get; set; }
    }

    /// <summary>
    /// 设置读写。读写失败均静默回退默认值，不影响主流程。
    /// </summary>
    public static class SettingsStore
    {
        private static readonly string SettingsDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CmdSender");

        private static readonly string SettingsPath = Path.Combine(SettingsDir, "settings.json");

        public static AppSettings Load()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return new AppSettings();
                string json = File.ReadAllText(SettingsPath, Encoding.UTF8);
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var serializer = new DataContractJsonSerializer(typeof(AppSettings));
                    return (AppSettings)serializer.ReadObject(stream) ?? new AppSettings();
                }
            }
            catch
            {
                return new AppSettings();
            }
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                if (settings == null) return;
                Directory.CreateDirectory(SettingsDir);
                using (var stream = new MemoryStream())
                {
                    var serializer = new DataContractJsonSerializer(typeof(AppSettings));
                    serializer.WriteObject(stream, settings);
                    string json = Encoding.UTF8.GetString(stream.ToArray());
                    File.WriteAllText(SettingsPath, json, Encoding.UTF8);
                }
            }
            catch
            {
                // 忽略保存失败
            }
        }
    }
}
