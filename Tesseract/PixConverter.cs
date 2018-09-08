
using System;
using System.Collections.Generic;
using System.Drawing;
namespace Tesseract
{
    /// <summary>
    /// Handles converting between different image formats supported by DotNet.
    /// </summary>
    public static class PixConverter
    {
        private static readonly BitmapToPixConverter bitmapConverter = new BitmapToPixConverter();

        /// <summary>
        /// Converts the specified <paramref name="img"/> to a Pix.
        /// </summary>
        /// <param name="img">The source image to be converted.</param>
        /// <returns>The converted bitmap image as a <see cref="Pix"/>.</returns>
        public static Pix ToPix(Bitmap img)
        {
            return bitmapConverter.Convert(img);
        }
    }
}