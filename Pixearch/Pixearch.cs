using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pixearch
{
    /*  
    Author: LBLZR_
    License: Creative Commons Attribution-ShareAlike 4.0 International License.
    (https://creativecommons.org/licenses/by-sa/4.0/)
    Modified from https://codereview.stackexchange.com/questions/138011/find-a-bitmap-within-another-bitmap 
    */
    public static class Searcher
    {
        // Color difference margin
        static byte margin = 0;

        // Finds the location of needle bitmap in haystack bitmap with provided tolerance.
        // If needle is not found returns null.
        public static Point? Find(Bitmap haystack,  // Bitmap with needle
            Bitmap needle,                          // Bitmap to find
            float tolerance = 1f                    // Color tolerance; 0 = 100% accuracy, 1 = 0% accuracy
            )
        {
            // If bitmaps are null we return null
            if (null == haystack || null == needle)
            {
                return null;
            }

            // If haystack bitmap is smaller than needle bitmap we return null
            if (haystack.Width < needle.Width || haystack.Height < needle.Height)
            {
                return null;
            }

            // We clamp the margin to 0f-1f and convert it to value 0-255
            margin = Convert.ToByte(Clamp(tolerance, 0f, 1f) * 255);

            // We get bitmap data for each bitmap
            var haystackArray = GetPixelArray(haystack);
            var needleArray = GetPixelArray(needle);

            // We loop over each haystack line until we find line with part of needle bitmap
            foreach (var firstLineMatchPoint in FindMatch(haystackArray.Take(haystack.Height - needle.Height), needleArray[0]))
            {
                // If we find one matching line we check if the whole needle bitmap is present 
                if (IsNeedlePresentAtLocation(haystackArray, needleArray, firstLineMatchPoint, 1))
                {
                    return firstLineMatchPoint;
                }
            }

            return null;
        }

        private static uint[][] GetPixelArray(Bitmap bitmap)
        {
            // We copy the raw bitmap data and load it in ram
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            for (int y = 0; y < bitmap.Height; ++y)
            {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            // Now we magically convert int array to uint array
            return (uint[][])(object)result; //Fuck this
        }

        private static IEnumerable<Point> FindMatch(IEnumerable<uint[]> haystackLines, uint[] needleLine)
        {
            // We try to iterate over haystack lines and increase the offset until we find the matching needle line
            var y = 0;
            foreach (var haystackLine in haystackLines)
            {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x)
                {
                    if (ContainSameElements(haystackLine, x, needleLine, 0, needleLine.Length))
                    {
                        yield return new Point(x, y);
                    }
                }
                y += 1;
            }
        }

        private static bool ContainSameElements(uint[] first, int firstStart, uint[] second, int secondStart, int length)
        {
            for (int i = 0; i < length; ++i)
            {
                // We select pixels from each bitmap
                uint firstPixel = first[i + firstStart];
                uint secondPixel = second[i + secondStart];

                // we extract B, G, R and A value (0-255) from the pixel uint
                // a = color >> 24
                // r = color >> 16
                // g = color >> 8
                // b = color >> 0
                for (byte j = 0; j <= 24; j += 8)
                {
                    byte fcolor = (byte)(firstPixel >> j);
                    byte scolor = (byte)(secondPixel >> j);

                    // We compare the pixel values with min margin and max margin
                    if (scolor > (fcolor + margin) || scolor < (fcolor - margin))
                    {
                        // The value is out of margin range
                        return false;
                    }
                }
            }
            // Each pixel value was in margin range
            return true;
        }

        private static bool IsNeedlePresentAtLocation(uint[][] haystack, uint[][] needle, Point point, int alreadyVerified)
        {
            // We already know that "alreadyVerified" lines already match, so skip them
            for (int y = alreadyVerified; y < needle.Length; ++y)
            {
                if (!ContainSameElements(haystack[y + point.Y], point.X, needle[y], 0, needle[y].Length))
                {
                    return false;
                }
            }
            return true;
        }

        private static float Clamp(float value, float min, float max)
        {
            // Basic float clamping function
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
