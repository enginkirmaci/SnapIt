using System.Collections.Generic;
using SnapIt.Library.Entities;

namespace SnapIt.Library.Services
{
    public interface IWindowService
    {
        bool IsVisible { get; }

        void Initialize();

        void Release();

        void Show();

        void Hide();

        IList<Rectangle> SnapAreaBoundries();

        Rectangle SelectElementWithPoint(int x, int y);
    }
}