using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
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

        public List<ApplicationItem> ListInstalledApplications()
        {
            Windows.Management.Deployment.PackageManager packageManager = new Windows.Management.Deployment.PackageManager();
            var packages = packageManager.FindPackagesForUser("").ToList();

            // GUID taken from https://docs.microsoft.com/en-us/windows/win32/shell/knownfolderid
            var FOLDERID_AppsFolder = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");
            ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(FOLDERID_AppsFolder);

            var result = new List<ApplicationItem>();
            foreach (var app in (IKnownFolder)appsFolder)
            {
                result.Add(new ApplicationItem
                {
                    Title = app.Name,
                    Arguments = $"shell:appsFolder\\{app.ParsingName}",
                    Path = "explorer.exe"
                });
            }

            return result.OrderBy(i => i.Title).ToList();
        }

        public async Task<ActiveWindow> StartApplication(ApplicationItem application, Rectangle rectangle)
        {
            try
            {
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

                DevMode.Log($"{application.Path} - {application.Title} - started");
                openedWindow = GetOpenedWindow(application, process);

                await Task.Delay(500);

                if (string.IsNullOrEmpty(openedWindow.Title))
                {
                    DevMode.Log($"{application.Path} - {openedWindow.Title} tryTitleParts");
                    openedWindow = GetOpenedWindow(application, process, tryTitleParts: true);
                }
                if (string.IsNullOrEmpty(openedWindow.Title))
                {
                    DevMode.Log($"{application.Path} - {openedWindow.Title} useFirstOne");
                    openedWindow = GetOpenedWindow(application, process, useFirstOne: true);
                }

                return openedWindow;
            }
            catch (Exception ex)
            {
                return null;
            }
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
                    //result.Remove(handle);

                    return new ActiveWindow
                    {
                        Handle = handle,
                        Title = proc.MainWindowTitle
                    };
                }
                else if (proc.MainWindowTitle == application.Title || (!string.IsNullOrEmpty(application.Title) && proc.MainWindowTitle.Contains(application.Title)))
                {
                    cachedWindowHandles.Add(proc.MainWindowHandle, proc.MainWindowTitle);
                    //result.Remove(proc.MainWindowHandle);

                    return new ActiveWindow
                    {
                        Handle = proc.MainWindowHandle,
                        Title = proc.MainWindowTitle
                    };
                }
                else if (result[handle] == application.Title || (!string.IsNullOrEmpty(application.Title) && result[handle].Contains(application.Title)))
                {
                    cachedWindowHandles.Add(handle, result[handle]);
                    //result.Remove(handle);

                    return new ActiveWindow
                    {
                        Handle = handle,
                        Title = result[handle]
                    };
                }
                else if (tryTitleParts || useFirstOne)
                {
                    if (tryTitleParts)
                    {
                        if (!string.IsNullOrEmpty(proc.MainWindowTitle) && !string.IsNullOrEmpty(application.Title))
                        {
                            string[] vs = proc.MainWindowTitle.Split(new char[] { ' ', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] vs1 = application.Title.Split(new char[] { ' ', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                            var count = vs.Intersect(vs1, StringComparer.OrdinalIgnoreCase).Count();
                            if (count >= 1)
                            {
                                cachedWindowHandles.Add(proc.MainWindowHandle, proc.MainWindowTitle);
                                //result.Remove(proc.MainWindowHandle);

                                return new ActiveWindow
                                {
                                    Handle = proc.MainWindowHandle,
                                    Title = proc.MainWindowTitle
                                };
                            }
                        }
                        if (!string.IsNullOrEmpty(result[handle]) && !string.IsNullOrEmpty(application.Title))
                        {
                            string[] vs = result[handle].Split(new char[] { ' ', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                            string[] vs1 = application.Title.Split(new char[] { ' ', '-', '/', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);

                            var count = vs.Intersect(vs1, StringComparer.OrdinalIgnoreCase).Count();
                            if (count >= 1)
                            {
                                cachedWindowHandles.Add(handle, result[handle]);
                                //result.Remove(handle);

                                return new ActiveWindow
                                {
                                    Handle = handle,
                                    Title = result[handle]
                                };
                            }
                        }
                    }

                    if (useFirstOne)
                    {
                        cachedWindowHandles.Add(handle, result[handle]);
                        //result.Remove(handle);

                        return new ActiveWindow
                        {
                            Handle = handle,
                            Title = result[handle]
                        };
                    }
                }
            }

            return activeWindow;
        }
    }
}