using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Controls;
using SnapIt.Entities;
using SnapIt.UI.Views;

namespace SnapIt.UI.ViewModels
{
	public class DesignWindowViewModel : BindableBase
	{
		public SnapScreen SnapScreen { get; set; }
		public Layout Layout { get; set; }
		public SnapArea MainSnapArea { get; set; }
		public DesignWindow Window { get; set; }

		public DelegateCommand<object> LoadedCommand { get; }
		public DelegateCommand SaveLayoutCommand { get; private set; }

		public DesignWindowViewModel()
		{
			LoadedCommand = new DelegateCommand<object>((mainSnapArea) =>
			{
				MainSnapArea = mainSnapArea as SnapArea;

				MainSnapArea.ApplyLayout(Layout.LayoutArea, true);
			});

			SaveLayoutCommand = new DelegateCommand(() =>
			{
				Layout.GenerateLayoutArea(MainSnapArea);

				Window.Close();
			});
		}
	}
}