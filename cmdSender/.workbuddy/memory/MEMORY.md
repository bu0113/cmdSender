# cmdSender 项目记忆

## 项目概述
- **名称**: cmdSender (窗口命令发送器)
- **技术栈**: C# WinForms, .NET Framework 4.8
- **项目路径**: F:\files\cmdsender\cmdSender
- **构建方式**: `dotnet build` (SDK-style csproj, 需 .NET 9 SDK + net48 reference assemblies)

## 架构
- `CommandSender.cs` - Win32 P/Invoke 层：窗口查找(递归 ChildWindowFromPointEx)、PostMessage/SendKeys 两种发送方式
- `Scheduler.cs` - async/await 循环调度器，generation 机制防止旧任务 OnCompleted 误触发
- `MainForm.cs` / `MainForm.Designer.cs` - UI 与业务逻辑

## 关键设计决策
- 项目从旧式 csproj 转为 SDK-style 以支持 dotnet build
- 发送方式: PostMessage(WM_CHAR) 后台发送为默认; SendKeys 前台发送为备选
- 循环发送: 选中多行则发送选中行, 否则发送全部非空行
- 文件操作: 保存时若已有路径则直接保存, 否则弹另存为对话框; 脏标记跟踪

## 2026-07-25 完成工作
- 完整重写全部 4 个核心源文件, 修复原 Scheduler ResetQueue 丢命令 bug
- 修复窗口拖动选择(原代码 MouseUp 时 e.Location 不准确, 改用 Cursor.Position)
- 新增: 选中行循环发送、发送回车选项、发送方式选择、文件路径跟踪、脏标记提示
- 编译通过: 0 警告 0 错误, 输出 bin/Debug/cmdSender.exe
