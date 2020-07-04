using System;
using System.Drawing;

namespace CoachBot.Extensions
{
    public static class ColorExtensions
    {
            public static Color FromHex(string hexColor)
            {
                Color c = Color.Empty;

                if ((hexColor == null) || (hexColor.Length == 0))
                    return c;

                if ((hexColor[0] == '#') &&
                    ((hexColor.Length == 7) || (hexColor.Length == 4)))
                {
                    if (hexColor.Length == 7)
                    {
                        c = Color.FromArgb(Convert.ToInt32(hexColor.Substring(1, 2), 16),
                                           Convert.ToInt32(hexColor.Substring(3, 2), 16),
                                           Convert.ToInt32(hexColor.Substring(5, 2), 16));
                    }
                    else
                    {
                        string r = char.ToString(hexColor[1]);
                        string g = char.ToString(hexColor[2]);
                        string b = char.ToString(hexColor[3]);

                        c = Color.FromArgb(Convert.ToInt32(r + r, 16),
                                           Convert.ToInt32(g + g, 16),
                                           Convert.ToInt32(b + b, 16));
                    }
                }

                // special case. Html requires LightGrey, but .NET uses LightGray
                if (c.IsEmpty && string.Equals(hexColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
                {
                    c = Color.LightGray;
                }

                return c;
            }
        
    }
}
