using SnapIt.Common.Contracts;
using SnapIt.Common.Entities;
using SnapIt.Common.Graphics;

namespace SnapIt.Application.Contracts;

public interface IWindowManager : IInitialize
{
    void Release();

    void Show();

    void Hide();

    Dictionary<int, Rectangle> GetSnapAreaRectangles(SnapScreen snapScreen);
}