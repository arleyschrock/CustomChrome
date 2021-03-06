﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CustomChrome
{
    internal static class ImageMaps
    {
        public static readonly int[,] Corner = new[,] {
            { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 },
            { 0, 0, 0, 1, 1, 1, 3, 3, 3, 3 },
            { 0, 0, 1, 1, 3, 3, 3, 3, 4, 5 },
            { 0, 1, 1, 3, 3, 3, 4, 5, 6, 7 },
            { 0, 1, 3, 3, 3, 4, 7, 8, 11, 13 },
            { 0, 1, 3, 3, 4, 7, 10, 13, 18, 23 },
            { 1, 3, 3, 4, 7, 10, 15, 20, 28, 33 },
            { 1, 3, 3, 5, 8, 13, 20, 28, 38, 47 },
            { 1, 3, 4, 6, 11, 18, 28, 38, 255, 255 },
            { 1, 3, 5, 7, 13, 23, 33, 47, 255, 0 }
        };

        public static readonly int[,] Border = new[,] {
            { 3 },
            { 4 },
            { 6 },
            { 10 },
            { 17 },
            { 28 },
            { 42 },
            { 61 },
            { 255 }
        };

        public static readonly int[,] Close = new[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 128, 255, 128, 0, 0, 0, 0, 128, 255, 128 },
            { 0, 144, 255, 113, 0, 0, 113, 255, 144, 0 },
            { 0, 0, 144, 255, 113, 113, 255, 144, 0, 0 },
            { 0, 0, 0, 144, 255, 255, 144, 0, 0, 0 },
            { 0, 0, 0, 128, 255, 255, 128, 0, 0, 0 },
            { 0, 0, 113, 255, 144, 144, 255, 113, 0, 0 },
            { 0, 128, 255, 128, 0, 0, 128, 255, 128, 0 },
            { 128, 255, 128, 0, 0, 0, 0, 128, 255, 128 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public static readonly int[,] Maximize = new[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 255, 0, 0, 0, 0, 0, 0, 0, 255 },
            { 0, 255, 0, 0, 0, 0, 0, 0, 0, 255 },
            { 0, 255, 0, 0, 0, 0, 0, 0, 0, 255 },
            { 0, 255, 0, 0, 0, 0, 0, 0, 0, 255 },
            { 0, 255, 0, 0, 0, 0, 0, 0, 0, 255 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public static readonly int[,] Minimize = new[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 255, 255, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public static readonly int[,] Restore = new[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 0, 0, 255, 255, 255, 255, 255, 255, 255 },
            { 0, 0, 0, 255, 0, 0, 0, 0, 0, 255 },
            { 255, 255, 255, 255, 255, 255, 255, 0, 0, 255 },
            { 255, 255, 255, 255, 255, 255, 255, 0, 0, 255 },
            { 255, 0, 0, 0, 0, 0, 255, 0, 0, 255 },
            { 255, 0, 0, 0, 0, 0, 255, 255, 255, 255 },
            { 255, 0, 0, 0, 0, 0, 255, 0, 0, 0 },
            { 255, 0, 0, 0, 0, 0, 255, 0, 0, 0 },
            { 255, 255, 255, 255, 255, 255, 255, 0, 0, 0 }
        };
    }
}
