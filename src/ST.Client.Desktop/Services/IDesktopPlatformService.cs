﻿using System.Application.Models;
using System.Application.Models.Settings;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using ResizeMode = System.Int32;

namespace System.Application.Services
{
    public interface IDesktopPlatformService
    {
        const string TAG = "DesktopPlatformS";

        void SetResizeMode(IntPtr hWnd, ResizeMode value);

        /// <summary>
        /// 获取一个正在运行的进程的命令行参数。
        /// 与 <see cref="Environment.GetCommandLineArgs"/> 一样，使用此方法获取的参数是包含应用程序路径的。
        /// 关于 <see cref="Environment.GetCommandLineArgs"/> 可参见：
        /// .NET 命令行参数包含应用程序路径吗？https://blog.walterlv.com/post/when-will-the-command-line-args-contain-the-executable-path.html
        /// </summary>
        /// <param name="process">一个正在运行的进程。</param>
        /// <returns>表示应用程序运行命令行参数的字符串。</returns>
        string GetCommandLineArgs(Process process);

        /// <summary>
        /// hosts 文件所在目录
        /// </summary>
        string HostsFilePath => "/etc/hosts";

        void StartProcess(string name, string filePath) => Process.Start(name, filePath);

        /// <summary>
        /// 使用文本阅读器打开文件
        /// </summary>
        /// <param name="filePath"></param>
        void OpenFileByTextReader(string filePath)
        {
            TextReaderProvider? userProvider = null;
            var p = GeneralSettings.TextReaderProvider.Value;
            if (p != null)
            {
                var platform = DI.Platform;
                if (p.ContainsKey(platform))
                {
                    var value = p[platform];
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (Enum.TryParse<TextReaderProvider>(value, out var enumValue))
                        {
                            userProvider = enumValue;
                        }
                        else
                        {
                            try
                            {
                                StartProcess(value, filePath);
                                return;
                            }
                            catch (Exception e)
                            {
                                Log.Error(TAG, e, "UserSettings OpenFileByTextReader Fail.");
                            }
                        }
                    }
                }
            }

            var providers = new List<TextReaderProvider>() {
                TextReaderProvider.VSCode,
                TextReaderProvider.Notepad };

            if (userProvider.HasValue)
            {
                providers.Remove(userProvider.Value);
                providers.Insert(0, userProvider.Value);
            }

            foreach (var item in providers)
            {
                try
                {
                    var fileName = GetFileName(item);
                    if (fileName == null) continue;
                    StartProcess(fileName, filePath);
                    return;
                }
                catch (Exception e)
                {
                    if (item == TextReaderProvider.Notepad)
                    {
                        Log.Error(TAG, e, "OpenFileByTextReader Fail.");
                    }
                }
            }
        }

        /// <summary>
        /// 获取文本阅读器提供商程序文件路径或文件名(如果提供程序已注册环境变量)
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        string? GetFileName(TextReaderProvider provider);

        /// <summary>
        /// 设置开机自启动
        /// </summary>
        /// <param name="isAutoStart">开启<see langword="true"/>、关闭<see langword="false"/></param>
        /// <param name="name"></param>
        void SetBootAutoStart(bool isAutoStart, string name);

        #region Steam

        /// <inheritdoc cref="ISteamService.SteamDirPath"/>
        string? GetSteamDirPath();

        /// <inheritdoc cref="ISteamService.SteamProgramPath"/>
        string? GetSteamProgramPath();

        /// <inheritdoc cref="ISteamService.GetLastLoginUserName"/>
        string GetLastSteamLoginUserName();

        /// <inheritdoc cref="ISteamService.SetCurrentUser(string)"/>
        void SetCurrentUser(string userName);

        #endregion

        public const ResizeMode ResizeMode_NoResize = 0;
        public const ResizeMode ResizeMode_CanMinimize = 1;
        public const ResizeMode ResizeMode_CanResize = 2;
        public const ResizeMode ResizeMode_CanResizeWithGrip = 3;

        #region MachineUniqueIdentifier

        private static (byte[] key, byte[] iv) GetMachineSecretKey(string? value)
        {
            value ??= string.Empty;
            var result = AESUtils.GetParameters(value);
            return result;
        }

        protected static Lazy<(byte[] key, byte[] iv)> GetMachineSecretKey(Func<string?> action) => new(() =>
        {
            string? value = null;
            try
            {
                value = action();
            }
            catch (Exception e)
            {
                Log.Warn(TAG, e, "GetMachineSecretKey Fail.");
            }
            if (string.IsNullOrWhiteSpace(value))
            {
                value = Environment.MachineName;
            }
            return GetMachineSecretKey(value);
        });

        (byte[] key, byte[] iv) MachineSecretKey { get; }

        #endregion

        /// <summary>
        /// 是否应使用 亮色主题 <see langword="true"/> / 暗色主题 <see langword="false"/>
        /// </summary>
        bool? IsLightOrDarkTheme { get; }

        /// <summary>
        /// 设置亮色或暗色主题跟随系统
        /// </summary>
        /// <param name="enable"></param>
        void SetLightOrDarkThemeFollowingSystem(bool enable);
    }
}