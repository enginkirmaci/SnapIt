namespace SnapScreen.Library.Entities
{
    public struct Dpi
    {
        public double X;
        public double Y;

        public static Dpi Default { get { return new Dpi { X = 1.0, Y = 1.0 }; } }

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}";
        }
    }
}