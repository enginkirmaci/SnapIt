using SnapIt.Common.Entities;

namespace SnapIt.Common.Graphics;

public class Line
{
    [JsonIgnore]
    public Point Start { get; set; }

    [JsonIgnore]
    public Point End { get; set; }

    public Point Point { get; set; }
    public Size Size { get; set; }

    public SplitDirection SplitDirection { get; set; }

    public override string ToString()
    {
        return $"{Start.X}, {Start.Y}   {End.X}, {End.Y} ";
    }
}