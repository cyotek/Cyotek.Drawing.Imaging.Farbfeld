using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Cyotek.Drawing.Imaging.Farbfeld
{
  /// <summary>
  /// Encoder class for the farbfeld image format.
  /// </summary>
  public class FarbfeldEncoder
  {
    #region Methods

    /// <summary>
    /// Saves the given <see cref="Bitmap"/> into the specified file using the farbfeld image format.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="fileName">A string that contains the name of the file to which to save the <see cref="Bitmap"/>.</param>
    /// <param name="image">The <see cref="Bitmap"/> to encode.</param>
    public void Encode(string fileName, Bitmap image)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      if (image == null)
      {
        throw new ArgumentNullException(nameof(image));
      }

      using (Stream stream = File.Create(fileName))
      {
        this.Encode(stream, image);
      }
    }

    /// <summary>
    /// Saves the given <see cref="Bitmap"/> into the specified file using the farbfeld image format.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="stream">The <see cref="Stream"/> where the image will be saved.</param>
    /// <param name="image">The <see cref="Bitmap"/> to encode.</param>
    /// <exception cref="ArgumentException">Thrown if the source image is not a 32bpp ARGB image.</exception>
    public void Encode(Stream stream, Bitmap image)
    {
      int width;
      int height;
      ArgbColor[] pixels;

      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      if (image == null)
      {
        throw new ArgumentNullException(nameof(image));
      }

      if (image.PixelFormat != PixelFormat.Format32bppArgb)
      {
        throw new ArgumentException("Only 32bpp ARGB images are supported.", nameof(image));
      }

      stream.WriteByte((byte)'f');
      stream.WriteByte((byte)'a');
      stream.WriteByte((byte)'r');
      stream.WriteByte((byte)'b');
      stream.WriteByte((byte)'f');
      stream.WriteByte((byte)'e');
      stream.WriteByte((byte)'l');
      stream.WriteByte((byte)'d');

      width = image.Width;
      height = image.Height;

      stream.WriteBigEndian(width);
      stream.WriteBigEndian(height);

      pixels = this.GetPixels(image);

      foreach (ArgbColor pixel in pixels)
      {
        ushort r;
        ushort g;
        ushort b;
        ushort a;

        r = (ushort)(pixel.R * 256);
        g = (ushort)(pixel.G * 256);
        b = (ushort)(pixel.B * 256);
        a = (ushort)(pixel.A * 256);

        stream.WriteBigEndian(r);
        stream.WriteBigEndian(g);
        stream.WriteBigEndian(b);
        stream.WriteBigEndian(a);
      }
    }

    private ArgbColor[] GetPixels(Bitmap bitmap)
    {
      int width;
      int height;
      BitmapData bitmapData;
      ArgbColor[] results;

      width = bitmap.Width;
      height = bitmap.Height;
      results = new ArgbColor[width * height];
      bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly,
                                   PixelFormat.Format32bppArgb);

      unsafe
      {
        ArgbColor* pixel;

        pixel = (ArgbColor*)bitmapData.Scan0;

        for (int row = 0; row < height; row++)
        {
          for (int col = 0; col < width; col++)
          {
            results[row * width + col] = *pixel;

            pixel++;
          }
        }
      }

      bitmap.UnlockBits(bitmapData);

      return results;
    }

    #endregion
  }
}
