using System.Windows;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.ViewModels;

namespace SnapIt.Views
{
	/// <summary>
	/// Interaction logic for DesignWindow.xaml
	/// </summary>
	public partial class DesignWindow : Window
	{
		private SnapScreen snapScreen;

		public DesignWindow()
		{
			InitializeComponent();
		}

		public void SetScreen(SnapScreen snapScreen, Layout layout)
		{
			this.snapScreen = snapScreen;

			Width = snapScreen.Base.WorkingArea.Width;
			Height = snapScreen.Base.WorkingArea.Height;
			Left = snapScreen.Base.WorkingArea.X;
			Top = snapScreen.Base.WorkingArea.Y;

			var model = DataContext as DesignWindowViewModel;
			model.Window = this;
			model.SnapScreen = snapScreen;
			model.Layout = layout;

			if (DevMode.IsActive)
			{
				Topmost = false;
			}
		}
	}
}