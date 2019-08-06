﻿using System.Collections.Generic;
using SnapIt.Library.Controls;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
	public interface ISettingService
	{
		Config Config { get; }
		IList<Layout> Layouts { get; }
		IList<SnapScreen> SnapScreens { get; }

		void Save();

		void SaveLayout(Layout layout);

		void LinkScreenLayout(SnapScreen snapScreen, Layout layout);
	}
}