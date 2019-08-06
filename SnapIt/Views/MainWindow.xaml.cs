﻿using System;
using System.Windows;

namespace SnapIt.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool oneTime = true;

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnActivated(EventArgs e)
		{
			base.OnActivated(e);

			if (!DevMode.IsActive && oneTime)
			{
				oneTime = false;
				Hide();
			}
		}
	}
}