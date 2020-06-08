using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace General.Colors
{
    public class Colors
    {
        public static Color32 colorRed = new Color32(209, 0, 16, 255);
        public static Color32 colorRedLight = colorRed;

        public static Color32 colorBlue = new Color32(49, 128, 183, 255);
        public static Color32 colorBlueLight = new Color32(118, 193, 255, 255);

        public static Color32 colorGreenLight = new Color32(112, 197, 66, 255); // #589118
        public static Color32 colorGreen = new Color32(88, 145, 24, 255); // #589118
        public static Color32 colorYellow = new Color32(255, 233, 0, 255);

        public static Color32 outlineBasic = new Color32(43, 43, 43, 255);
        public static Color32 outlineWhite = new Color32(255, 255, 255, 255);
    }
}