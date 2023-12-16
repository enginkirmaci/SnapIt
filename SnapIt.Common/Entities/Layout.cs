using SnapIt.Common.Graphics;
using SnapIt.Common.Mvvm;

namespace SnapIt.Common.Entities;

public class Layout : Bindable
{
    private string name;
    private int areaPadding;

    public string Version = "2.0";
    public Guid Guid { get; set; }

    [JsonIgnore]
    public bool IsNew { get; set; }

    [JsonIgnore]
    public LayoutStatus Status { get; set; }

    [JsonIgnore]
    public SnapAreaTheme Theme { get; set; }

    public string Name
    {
        get => name;
        set
        {
            Status = LayoutStatus.NotSaved;
            SetProperty(ref name, value);
        }
    }

    public Size Size { get; set; } = Size.Empty;
    public List<Line> LayoutLines { get; set; }
    public List<LayoutOverlay> LayoutOverlays { get; set; }

    public int AreaPadding
    {
        get => areaPadding;
        set
        {
            Status = LayoutStatus.NotSaved;
            SetProperty(ref areaPadding, value);
        }
    }

    public Layout()
    {
        LayoutLines = [];
        LayoutOverlays = [];
    }
}