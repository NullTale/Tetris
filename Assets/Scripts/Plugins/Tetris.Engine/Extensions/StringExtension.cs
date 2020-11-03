namespace Tetris.Engine.Extensions
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringExtension
    {
        public static bool[][] StringToBoolMatrix(this string str, int size)
        {
            return new Regex("\\D+").Replace(str, string.Empty).Select(x => x == '1').ToArray().Split(size).Reverse().ToArray();
        }
    }
}
