using System;
using System.Collections.ObjectModel;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Controls;
using SnapIt.Entities;
using SnapIt.Services;
using SnapIt.UI.Views;

namespace SnapIt.UI.ViewModels
{
	public class MainWindowViewModel : BindableBase
	{
		private readonly ISnapService snapService;
		private readonly ISettingService settingService;

		private ObservableCollection<MouseButton> mouseButtons;
		private ObservableCollection<SnapScreen> snapScreens;
		private SnapScreen selectedSnapScreen;
		private ObservableCollection<Layout> layouts;
		private Layout selectedLayout;

		public string Title { get; set; } = $"Snap It {System.Windows.Forms.Application.ProductVersion}";
		public bool DragByTitle { get => settingService.Config.DragByTitle; set { settingService.Config.DragByTitle = value; ApplyChanges(); } }
		public MouseButton MouseButton { get => settingService.Config.MouseButton; set { settingService.Config.MouseButton = value; ApplyChanges(); } }
		public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
		public bool DisableForFullscreen { get => settingService.Config.DisableForFullscreen; set { settingService.Config.DisableForFullscreen = value; ApplyChanges(); } }
		public ObservableCollection<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }
		public SnapScreen SelectedSnapScreen
		{
			get => selectedSnapScreen;
			set
			{
				SetProperty(ref selectedSnapScreen, value);
				selectedLayout = selectedSnapScreen.Layout;
			}
		}

		public ObservableCollection<Layout> Layouts { get => layouts; set => SetProperty(ref layouts, value); }

		public Layout SelectedLayout
		{
			get => selectedLayout;
			set
			{
				SetProperty(ref selectedLayout, value);
				SaveLayoutCommand.RaiseCanExecuteChanged();

				if (SelectedSnapScreen != null)
				{
					SelectedSnapScreen.Layout = selectedLayout;
					settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);
					ApplyChanges();
				}
			}
		}

		public DelegateCommand<Window> CloseWindowCommand { get; private set; }
		public DelegateCommand NewLayoutCommand { get; private set; }
		public DelegateCommand SaveLayoutCommand { get; private set; }
		public DelegateCommand DesignLayoutCommand { get; private set; }

		public MainWindowViewModel(
			ISnapService snapService,
			ISettingService settingService)
		{
			this.snapService = snapService;
			this.settingService = settingService;

			Initialize();
		}

		private void Initialize()
		{
			Layouts = new ObservableCollection<Layout>(settingService.Layouts);
			SnapScreens = new ObservableCollection<SnapScreen>(settingService.SnapScreens);

			CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);

			NewLayoutCommand = new DelegateCommand(() =>
			{
				SelectedLayout = new Layout
				{
					Guid = Guid.NewGuid()
				};

				var designWindow = new DesignWindow();
				designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
				designWindow.Show();
			});

			SaveLayoutCommand = new DelegateCommand(() =>
			{
				settingService.SaveLayout(SelectedLayout);
			},
			() =>
			{
				return !string.IsNullOrWhiteSpace(SelectedLayout?.Name);
			}).ObservesProperty(() => SelectedLayout.Name);

			DesignLayoutCommand = new DelegateCommand(() =>
			{
				var designWindow = new DesignWindow();
				designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
				designWindow.Show();
			},
			() =>
			{
				return SelectedLayout != null;
			}).ObservesProperty(() => SelectedLayout);

			MouseButtons = new ObservableCollection<MouseButton>
			{
				MouseButton.Left,
				MouseButton.Middle,
				MouseButton.Right
			};

			//if (!DevMode.IsActive)
			//{
			//    snapService.Initialize();
			//}
			//else
			//{
			//    //var test = new TestWindow();
			//    //test.Show();
			//    OpenDesigner();
			//}
		}

		private void ApplyChanges()
		{
			snapService.Release();
			snapService.Initialize();
		}

		private void CloseWindow(Window window)
		{
			if (window != null)
			{
				settingService.Save();

				window.Hide();
			}

			if (DevMode.IsActive)
			{
				System.Windows.Application.Current.Shutdown();
			}
		}
	}
}