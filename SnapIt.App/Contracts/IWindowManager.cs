using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Application.Contracts;

public interface IWindowManager : IInitialize
{
    bool IsVisible { get; }

    void Release();

    void Show();

    void Hide();

    IList<Rectangle> SnapAreaBoundries();

    Dictionary<int, Rectangle> GetSnapAreaRectangles(SnapScreen snapScreen);

    SnapAreaInfo SelectElementWithPoint(int x, int y);
}