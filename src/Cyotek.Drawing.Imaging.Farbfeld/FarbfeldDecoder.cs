using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Cyotek.Drawing.Imaging.Farbfeld
{
  /// <summary>
  /// Decoder class for the farbfeld image format.
  /// </summary>
  public class FarbfeldDecoder
  {
    #region Methods

    /// <summary>
    /// Creates a <see cref="Bitmap"/> from the specified file.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="InvalidDataException">Thrown when the source data is not a Farbfeld image.</exception>
    /// <param name="fileName">A string that contains the name of the file from which to create the <see cref="Bitmap"/>.</param>
    /// <returns>
    /// The <see cref="Bitmap"/> this method creates.
    /// </returns>
    public Bitmap Decode(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      using (Stream stream = File.OpenRead(fileName))
      {
        return this.Decode(stream);
      }
    }

    /// <summary>
    /// Creates a <see cref="Bitmap"/> from the specified stream.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="InvalidDataException">Thrown when the source data is not a Farbfeld image.</exception>
    /// <param name="stream">A <see cref="Stream"/> that contains the data for this <see cref="Bitmap"/>.</param>
    /// <returns>
    /// The <see cref="Bitmap"/> this method creates.
    /// </returns>
    public Bitmap Decode(Stream stream)
    {
      int width;
      int height;
      int length;
      ArgbColor[] pixels;

      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      if (!this.IsFarbfeldImage(stream))
      {
        throw new InvalidDataException("Stream does not contain a farbfeld image.");
      }

      width = stream.ReadUInt32BigEndian();
      height = stream.ReadUInt32BigEndian();
      length = width * height;
      pixels = this.ReadPixelData(stream, length);

      return this.CreateBitmap(width, height, pixels);
    }

    /// <summary>
    /// Tests to see if the specified file contains Farbfeld image data.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="fileName">A string that contains the name of the file to query.</param>
    /// <returns>
    /// <c>true</c> if the file contains Farbfeld image data, otherwise <c>false</c>.
    /// </returns>
    public bool IsFarbfeldImage(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      using (Stream stream = File.OpenRead(fileName))
      {
        return this.IsFarbfeldImage(stream);
      }
    }

    /// <summary>
    /// Tests to see if the specified file contains Farbfeld image data.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="stream">A <see cref="Stream"/> that contains the data to query.</param>
    /// <returns>
    /// <c>true</c> if the <see cref="Stream"/> contains Farbfeld image data, otherwise <c>false</c>.
    /// </returns>
    /// <remarks>The position of the <see cref="Stream"/> is not reset after reading data.</remarks>
    public bool IsFarbfeldImage(Stream stream)
    {
      byte[] buffer;

      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      buffer = new byte[8];

      stream.Read(buffer, 0, buffer.Length);

      return buffer[0] == 'f' && buffer[1] == 'a' && buffer[2] == 'r' && buffer[3] == 'b' && buffer[4] == 'f' &&
             buffer[5] == 'e' && buffer[6] == 'l' && buffer[7] == 'd';
    }

    private Bitmap CreateBitmap(int width, int height, IList<ArgbColor> pixels)
    {
      Bitmap bitmap;
      BitmapData bitmapData;

      bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

      bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                                   PixelFormat.Format32bppArgb);

      unsafe
      {
        ArgbColor* pixelPtr;

        pixelPtr = (ArgbColor*)bitmapData.Scan0;

        for (int i = 0; i < width * height; i++)
        {
          *pixelPtr = pixels[i];
          pixelPtr++;
        }
      }

      bitmap.UnlockBits(bitmapData);

      return bitmap;
    }

    private ArgbColor[] ReadPixelData(Stream stream, int length)
    {
      ArgbColor[] pixels;

      pixels = new ArgbColor[length];

      for (int i = 0; i < length; i++)
      {
        int r;
        int g;
        int b;
        int a;

        r = stream.ReadUInt16BigEndian() / 256;
        g = stream.ReadUInt16BigEndian() / 256;
        b = stream.ReadUInt16BigEndian() / 256;
        a = stream.ReadUInt16BigEndian() / 256;

        pixels[i] = new ArgbColor(a, r, g, b);
      }

      return pixels;
    }

    #endregion
  }
}
