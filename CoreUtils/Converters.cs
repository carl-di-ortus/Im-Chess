using System;
using System.Windows;

namespace CoreUtils
{
    public class Converters
    {
        public static bool Invert { get; set; }

        public static Point ConvertFromCoord(string coord)
        {
            return Invert ? new Point(7 - (char)(coord[0] - 'a'), Convert.ToInt32(coord[1]) - 49) : new Point((char)(coord[0] - 'a'), 8 - (Convert.ToInt32(coord[1]) - 48));
        }

        public static string ConvertToCoord(double col, double row)
        {
            char letter;
            if (Invert)
            {
                letter = (char)('h' - Convert.ToInt32(col));
                return letter.ToString() + (Convert.ToInt32(row) + 1);
            }

            letter = (char)('a' + Convert.ToInt32(col));
            return letter.ToString() + (8 - Convert.ToInt32(row));
        }

        public static void ToggleInvert()
        {
            Invert = !Invert;
        }
    }
}
