using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Prism.Commands;
using Prism.Mvvm;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;
using SnapIt.Library.Services;
using SnapIt.Resources;
using SnapIt.Views;

namespace SnapIt.ViewModels
{
	public class MainWindowViewModel : BindableBase
	{
		private readonly ISnapService snapService;
		private readonly ISettingService settingService;

		private bool HideWindowAtStartup = true;
		private ObservableCollection<MouseButton> mouseButtons;
		private ObservableCollection<SnapScreen> snapScreens;
		private SnapScreen selectedSnapScreen;
		private ObservableCollection<Layout> layouts;
		private Layout selectedLayout;
		private bool isStartupTaskActive;

		public string Title { get; set; } = $"{Constants.AppName} {System.Windows.Forms.Application.ProductVersion}";
		public bool EnableKeyboard { get => settingService.Config.EnableKeyboard; set { settingService.Config.EnableKeyboard = value; ApplyChanges(); } }
		public bool EnableMouse { get => settingService.Config.EnableMouse; set { settingService.Config.EnableMouse = value; ApplyChanges(); } }
		public bool DragByTitle { get => settingService.Config.DragByTitle; set { settingService.Config.DragByTitle = value; ApplyChanges(); } }
		public MouseButton MouseButton { get => settingService.Config.MouseButton; set { settingService.Config.MouseButton = value; ApplyChanges(); } }
		public ObservableCollection<MouseButton> MouseButtons { get => mouseButtons; set => SetProperty(ref mouseButtons, value); }
		public bool DisableForFullscreen { get => settingService.Config.DisableForFullscreen; set { settingService.Config.DisableForFullscreen = value; ApplyChanges(); } }
		public bool IsStartupTaskActive
		{
			get => isStartupTaskActive;
			set
			{
				SetProperty(ref isStartupTaskActive, value);
				settingService.SetStartupTaskStatusAsync(value);
			}
		}

		public ObservableCollection<SnapScreen> SnapScreens { get => snapScreens; set => SetProperty(ref snapScreens, value); }
		public SnapScreen SelectedSnapScreen
		{
			get => selectedSnapScreen;
			set
			{
				SetProperty(ref selectedSnapScreen, value);
				SelectedLayout = selectedSnapScreen.Layout;
			}
		}

		public ObservableCollection<Layout> Layouts { get => layouts; set => SetProperty(ref layouts, value); }

		public Layout SelectedLayout
		{
			get => selectedLayout;
			set
			{
				if (value != null)
				{
					SetProperty(ref selectedLayout, value);
					//SaveLayoutCommand.RaiseCanExecuteChanged();

					SelectedSnapScreen.Layout = selectedLayout;
					settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);
					ApplyChanges();
				}
			}
		}

		public DelegateCommand<Window> ActivatedCommand { get; private set; }
		public DelegateCommand<Window> CloseWindowCommand { get; private set; }
		public DelegateCommand NewLayoutCommand { get; private set; }
		public DelegateCommand DesignLayoutCommand { get; private set; }
		public DelegateCommand ExportLayoutCommand { get; private set; }
		public DelegateCommand ImportLayoutCommand { get; private set; }

		public DelegateCommand<string> HandleLinkClick { get; private set; }

		public MainWindowViewModel(
			ISnapService snapService,
			ISettingService settingService)
		{
			this.snapService = snapService;
			this.settingService = settingService;

			Layouts = new ObservableCollection<Layout>(settingService.Layouts);
			SnapScreens = new ObservableCollection<SnapScreen>(settingService.SnapScreens);
			SelectedSnapScreen = SnapScreens.FirstOrDefault();

			ActivatedCommand = new DelegateCommand<Window>(async (mainWindow) =>
			{
				if (settingService.Config.ShowMainWindow)
				{
					settingService.Config.ShowMainWindow = false;
					HideWindowAtStartup = false;
				}
				else if (!DevMode.IsActive && HideWindowAtStartup)
				{
					HideWindowAtStartup = false;
					mainWindow.Hide();
				}

				IsStartupTaskActive = await settingService.GetStartupTaskStatusAsync();
			});

			CloseWindowCommand = new DelegateCommand<Window>(CloseWindow);

			NewLayoutCommand = new DelegateCommand(() =>
			{
				var layout = new Layout
				{
					Guid = Guid.NewGuid(),
					IsSaved = false,
					Name = "New layout"
				};

				Layouts.Add(layout);
				SelectedLayout = Layouts.FirstOrDefault(i => i.Guid == layout.Guid);

				var designWindow = new DesignWindow();
				designWindow.Closing += DesignWindow_Closing;
				designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
				designWindow.Show();
			});

			DesignLayoutCommand = new DelegateCommand(() =>
			{
				var designWindow = new DesignWindow();
				designWindow.Closing += DesignWindow_Closing;
				designWindow.SetScreen(SelectedSnapScreen, SelectedLayout);
				designWindow.Show();
			});

			ExportLayoutCommand = new DelegateCommand(() =>
			{
				var fileDialog = new SaveFileDialog
				{
					DefaultExt = ".json",
					Filter = "Json (.json)|*.json",
					FileName = selectedLayout.Name
				};

				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					settingService.ExportLayout(SelectedLayout, fileDialog.FileName);
				}
			});

			ImportLayoutCommand = new DelegateCommand(() =>
			{
				try
				{
					var fileDialog = new OpenFileDialog
					{
						DefaultExt = ".json",
						Filter = "Json (.json)|*.json"
					};

					if (fileDialog.ShowDialog() == DialogResult.OK)
					{
						var imported = settingService.ImportLayout(fileDialog.FileName);
						Layouts.Insert(0, imported);
					}
				}
				catch
				{
					System.Windows.Forms.MessageBox.Show("Layout file seems to be corrupted. Please try again with other layout file.");
				}
			});

			HandleLinkClick = new DelegateCommand<string>((url) =>
			{
				var ps = new ProcessStartInfo("http://" + url)
				{
					UseShellExecute = true,
					Verb = "open"
				};
				Process.Start(ps);
			});

			MouseButtons = new ObservableCollection<MouseButton>
			{
				MouseButton.Left,
				MouseButton.Middle,
				MouseButton.Right
			};

			if (!DevMode.IsActive)
			{
				snapService.Initialize();
			}
		}

		private void DesignWindow_Closing(object sender, CancelEventArgs e)
		{
			settingService.SaveLayout(SelectedLayout);
			settingService.LinkScreenLayout(SelectedSnapScreen, SelectedLayout);

			var position = Layouts.IndexOf(SelectedLayout);

			Layouts.Remove(SelectedLayout);
			Layouts.Insert(position, SelectedLayout);

			ApplyChanges();
		}

		private void ApplyChanges()
		{
			if (!DevMode.IsActive)
			{
				snapService.Release();
				snapService.Initialize();
			}
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