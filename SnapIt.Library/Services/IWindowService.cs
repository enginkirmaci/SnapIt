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

        Dictionary<int, Rectangle> GetSnapAreaRectangles(SnapScreen snapScreen);

        SnapAreaInfo SelectElementWithPoint(int x, int y);
    }
}