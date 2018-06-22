using Pixearch;
using PixearchTest.Properties;
using System;
using System.Drawing;

namespace PixearchTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load bitmaps
            Bitmap haystack = new Bitmap(Resources.haystack);
            Bitmap needle = new Bitmap(Resources.needle);

            // Finds the needle bitmap position in the haystack bitmap. 
            Point? p = Searcher.Find(haystack, needle, .1f);

            //Print the needle position
            Console.WriteLine(p);
            Console.ReadKey();
        }
    }
}
