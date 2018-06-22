# Pixearch
Lightweight C# library for finding bitmaps inside of other bitmaps

## Usage
Searcher.Find(OriginalBitmap, BitmapToFind, ColorTolerance);
```
// Load the bitmaps
Bitmap haystack = new Bitmap("haystack.bmp");
Bitmap needle = new Bitmap("needle.bmp");

// Find the needle bitmap position in the haystack bitmap. 
Point? p = Searcher.Find(haystack, needle, .1f);
```
Color tolerance should be in 0-1 range

## Getting Started
Download the compiled library from the releases tab or clone this project and compile the library manually with VS2017+

## Nuget
Comming soon

## License
Creative Commons Attribution-ShareAlike 4.0 International License.  
https://creativecommons.org/licenses/by-sa/4.0/
