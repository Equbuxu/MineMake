namespace MineMake
{
    internal static class MathHelper
    {
        internal static int Mod(int x, int m) => (x % m + m) % m;
    }
}
