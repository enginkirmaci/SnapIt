using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Shell;
using SnapIt.Common;
using SnapIt.Common.Entities;
using SnapIt.Common.Extensions;
using SnapIt.Common.Graphics;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class ApplicationService : IApplicationService
{
    private readonly IWinApiService winApiService;
    private IDictionary<nint, string> cachedWindowHandles;

    public bool IsInitialized { get; private set; }

    public ApplicationService(
        IWinApiService winApiService)
    {
        this.winApiService = winApiService;
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        await winApiService.InitializeAsync();

        cachedWindowHandles = winApiService.GetOpenWindows();

        IsInitialized = true;
    }

    public void Clear()
    {
        cachedWindowHandles?.Clear();
    }

    public List<ApplicationItem> ListInstalledApplications()
    {
        var packageManager = new Windows.Management.Deployment.PackageManager();
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
            var window = new ActiveWindow();

            Dev.Log($"{application.Path} - {application.Title} - StartProcess");

            var process = StartProcess(application);

            if (application.DelayAfterOpen < 1)
            {
                application.DelayAfterOpen = 1;
            }

            await Task.Delay(new TimeSpan(0, 0, application.DelayAfterOpen));

            Dev.Log($"{application.Path} - {application.Title} - GetWindowFromProcess");

            window = await GetWindowFromProcess(process);
            if (IsWindowOpened(window))
            {
                return window;
            }

            Dev.Log($"{application.Path} - {application.Title} - GetOpenedWindow");
            window = GetWindowFromOpenedWindows(application, process);
            if (IsWindowOpened(window))
            {
                return window;
            }
            Dev.Log($"{application.Path} - tryTitleParts");
            window = GetWindowFromOpenedWindows(application, process, tryTitleParts: true);
            if (IsWindowOpened(window))
            {
                Dev.Log($"{application.Path} - {window?.Title} tryTitleParts");
                return window;
            }

            Dev.Log($"{application.Path} - useFirstOne");
            window = GetWindowFromOpenedWindows(application, process, useFirstOne: true);
            if (IsWindowOpened(window))
            {
                Dev.Log($"{application.Path} - {window?.Title} useFirstOne");
                return window;
            }
        }
        catch (Exception ex)
        {
            Dev.Log(ex);
        }

        return null;
    }

    private Process StartProcess(ApplicationItem application)
    {
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = application.Path,
            Arguments = application.Arguments,
            WorkingDirectory = application.StartIn,
            UseShellExecute = true,
        });

        return process;
    }

    private async Task<ActiveWindow> GetWindowFromProcess(Process process)
    {
        var tryCount = 3;
        while (string.IsNullOrEmpty(process.MainWindowTitle) && tryCount > 0)
        {
            await Task.Delay(400);
            process.Refresh();

            tryCount--;

            Dev.Log($"{tryCount} - {process.MainWindowTitle}");
        }

        if (!string.IsNullOrEmpty(process.MainWindowTitle))
        {
            return new ActiveWindow
            {
                Handle = process.MainWindowHandle,
                Title = process.MainWindowTitle
            };
        }

        return null;
    }

    private ActiveWindow GetWindowFromOpenedWindows(ApplicationItem application, Process process, bool tryTitleParts = false, bool useFirstOne = false)
    {
        var openedApps = winApiService.GetOpenWindows();
        var result = openedApps
            .Where(kvp => !cachedWindowHandles.ContainsKey(kvp.Key))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var handle in result.Keys)
        {
            PInvoke.User32.GetWindowThreadProcessId(handle, out int processId);
            var proc = Process.GetProcessById(processId);

            if (tryTitleParts || useFirstOne)
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

                    return new ActiveWindow
                    {
                        Handle = handle,
                        Title = result[handle]
                    };
                }
            }
            else if (proc.ProcessExecutablePath() == application.Path)
            {
                cachedWindowHandles.Add(handle, proc.MainWindowTitle);

                return new ActiveWindow
                {
                    Handle = handle,
                    Title = proc.MainWindowTitle
                };
            }
            else if (proc.MainWindowTitle == application.Title || !string.IsNullOrEmpty(application.Title) && proc.MainWindowTitle.Contains(application.Title))
            {
                cachedWindowHandles.Add(proc.MainWindowHandle, proc.MainWindowTitle);

                return new ActiveWindow
                {
                    Handle = proc.MainWindowHandle,
                    Title = proc.MainWindowTitle
                };
            }
            else if (result[handle] == application.Title || !string.IsNullOrEmpty(application.Title) && result[handle].Contains(application.Title))
            {
                cachedWindowHandles.Add(handle, result[handle]);

                return new ActiveWindow
                {
                    Handle = handle,
                    Title = result[handle]
                };
            }
        }

        return null;
    }

    private bool IsWindowOpened(ActiveWindow window)
    {
        if (window == null || window.Handle == nint.Zero)
        {
            return false;
        }

        if (PInvoke.User32.GetWindowRect(window.Handle, out PInvoke.RECT rct))
        {
            window.Boundry = new Rectangle(rct.left, rct.top, rct.right, rct.bottom);
            window.Dpi = DpiHelper.GetDpiFromPoint(window.Boundry.Left, window.Boundry.Right);
        }

        if (window.Boundry.Equals(Rectangle.Empty))
        {
            return false;
        }

        return true;
    }
}