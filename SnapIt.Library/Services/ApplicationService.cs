using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using SnapIt.Library.Entities;
using SnapIt.Library.Extensions;

namespace SnapIt.Library.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IWinApiService winApiService;
        private IDictionary<IntPtr, string> cachedWindowHandles;

        public ApplicationService(
            IWinApiService winApiService)
        {
            this.winApiService = winApiService;
        }

        public void Initialize()
        {
            cachedWindowHandles = winApiService.GetOpenWindows();
        }

        public void Clear()
        {
            cachedWindowHandles?.Clear();
        }

        public async Task<ActiveWindow> StartApplication(ApplicationItem application, Rectangle rectangle)
        {
            DevMode.Log($"{application.Path} started");
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = application.Path,
                Arguments = application.Arguments,
                WorkingDirectory = application.StartIn,
                UseShellExecute = true,
            });

            if (application.DelayAfterOpen < 1)
            {
                application.DelayAfterOpen = 1;
            }

            await Task.Delay(new TimeSpan(0, 0, application.DelayAfterOpen));

            var openedWindow = new ActiveWindow();

            var maxCount = 20;
            while (string.IsNullOrEmpty(openedWindow.Title) && maxCount > 0)
            {
                DevMode.Log($"{application.Path} - {maxCount} - 20 try started");
                openedWindow = GetOpenedWindow(application, process);

                maxCount--;

                await Task.Delay(500);

                DevMode.Log($"{application.Path} - {maxCount} - 20 try ended");
            }

            if (string.IsNullOrEmpty(openedWindow.Title))
            {
                openedWindow = GetOpenedWindow(application, process, tryTitleParts: true);
                DevMode.Log($"{application.Path} - {openedWindow.Title} tryTitleParts");
            }
            if (string.IsNullOrEmpty(openedWindow.Title))
            {
                openedWindow = GetOpenedWindow(application, process, useFirstOne: true);
                DevMode.Log($"{application.Path} - {openedWindow.Title} useFirstOne");
            }

            return openedWindow;
        }

        private ActiveWindow GetOpenedWindow(ApplicationItem application, Process process, bool tryTitleParts = false, bool useFirstOne = false)
        {
            var openedApps = winApiService.GetOpenWindows();
            var result = openedApps
                .Where(kvp => !cachedWindowHandles.ContainsKey(kvp.Key))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            var activeWindow = new ActiveWindow();

            foreach (var handle in result.Keys)
            {
                PInvoke.User32.GetWindowThreadProcessId(handle, out int processId);
                var proc = Process.GetProcessById(processId);

                if (proc.ProcessExecutablePath() == application.Path)
                {
                    cachedWindowHandles.Add(handle, proc.MainWindowTitle);

                    return new ActiveWindow
                    {
                        Handle = handle,
                        Title = proc.MainWindowTitle
                    };
                }
                else if (proc.MainWindowTitle == application.Title)
                {
                    cachedWindowHandles.Add(proc.MainWindowHandle, proc.MainWindowTitle);

                    return new ActiveWindow
                    {
                        Handle = proc.MainWindowHandle,
                        Title = proc.MainWindowTitle
                    };
                }
                else if (tryTitleParts || useFirstOne)
                {
                    if (tryTitleParts && !string.IsNullOrEmpty(proc.MainWindowTitle) && !string.IsNullOrEmpty(application.Title))
                    {
                        string[] vs = proc.MainWindowTitle.Split(new char[] { ' ', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] vs1 = application.Title.Split(new char[] { ' ', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                        var count = vs.Intersect(vs1, StringComparer.OrdinalIgnoreCase).Count();
                        if (count >= 1)
                        {
                            cachedWindowHandles.Add(proc.MainWindowHandle, proc.MainWindowTitle);

                            return new ActiveWindow
                            {
                                Handle = proc.MainWindowHandle,
                                Title = proc.MainWindowTitle
                            };
                        }
                    }
                    if (useFirstOne)
                    {
                        cachedWindowHandles.Add(proc.MainWindowHandle, proc.MainWindowTitle);

                        return new ActiveWindow
                        {
                            Handle = proc.MainWindowHandle,
                            Title = proc.MainWindowTitle
                        };
                    }
                }
            }

            return activeWindow;
        }
    }
}