using System.Diagnostics;
using Prism.Commands;
using Prism.Mvvm;

namespace SnapIt.ViewModels
{
    public class AboutViewModel : BindableBase
    {
        public DelegateCommand<string> HandleLinkClick { get; private set; }

        public AboutViewModel()
        {
            HandleLinkClick = new DelegateCommand<string>((url) =>
            {
                //TODO change here without violating rules
                var ps = new ProcessStartInfo("http://" + url)
                {
                    UseShellExecute = true,
                    Verb = "open"
                };
                Process.Start(ps);
            });
        }
    }
}