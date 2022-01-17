using System.Collections.Generic;
using SnapScreen.Library.Entities;

namespace SnapScreen.Library.Services
{
    public interface IWindowService
    {
        bool IsVisible { get; }

        void Initialize();

        void Release();

        void Show();

        void Hide();

        IList<Rectangle> SnapAreaBoundries();

        SnapAreaInfo SelectElementWithPoint(int x, int y);
    }
}