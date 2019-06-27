using System.Windows.Forms;
using Prism.Mvvm;

namespace SnapIt.UI.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public string Title { get; set; }
            = $"Structure Copy Tool - {Application.ProductVersion}";
    }
}