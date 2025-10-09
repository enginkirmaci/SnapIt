using SharpHook;
using SnapIt.Services.Contracts;

namespace SnapIt.Services;

public class GlobalHookService : IGlobalHookService
{
    public SimpleGlobalHook? Hook { get; set; }

    public bool IsInitialized { get; private set; }

    public GlobalHookService()
    {
    }

    public async Task InitializeAsync()
    {
        if (IsInitialized)
        {
            return;
        }

        Hook = new SimpleGlobalHook();

        _ = System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (Hook != null && !Hook.IsRunning)
            {
                Hook?.Run();
            }
        });

        IsInitialized = true;
    }

    public void Dispose()
    {
        Hook?.Dispose();
    }
}