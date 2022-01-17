﻿using System.Collections.Generic;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.Services
{
    public interface IFileOperationService
    {
        void Save<T>(T config);

        T Load<T>() where T : new();

        void SaveLayout(Layout layout);

        void ExportLayout(Layout layout, string layoutPath);

        void DeleteLayout(Layout layout);

        Layout ImportLayout(string layoutPath);

        IList<Layout> GetLayouts();
    }
}