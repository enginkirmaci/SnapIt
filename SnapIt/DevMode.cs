namespace SnapIt
{
    public class DevMode
    {
#if DEBUG
        public const bool IsActive = false;
# else
        public const bool IsActive = false;
#endif
    }
}