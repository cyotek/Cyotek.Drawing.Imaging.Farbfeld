using System;
using System.IO;

namespace Cyotek.Drawing.Imaging
{
  /// <summary>
  /// Decoder class for the farbfeld image format.
  /// </summary>
  public static class FarbfeldDecoder
  {
    #region Static Methods

    public static FarbfeldImageData Decode(string fileName)
    {
      using (Stream stream = File.OpenRead(fileName))
      {
        return Decode(stream);
      }
    }

    public static FarbfeldImageData Decode(Stream stream)
    {
      int width;
      int height;
      byte[] header;
      byte[] buffer;
      byte[] data;
      int rowLength;
      int dataIndex;

      if (stream == null)
      {
        throw new ArgumentNullException(nameof(stream));
      }

      if (!IsFarbfeldImage(stream))
      {
        throw new InvalidDataException("Stream does not contain a farbfeld image.");
      }

      header = new byte[8];

      stream.Read(header, 0, header.Length);
      width = WordHelpers.MakeDWordBigEndian(header[0], header[1], header[2], header[3]);
      height = WordHelpers.MakeDWordBigEndian(header[4], header[5], header[6], header[7]);
      rowLength = width * Farbfeld.PixelDataLength;
      buffer = new byte[rowLength];
      data = new byte[width * height * 4];
      dataIndex = 0;

      for (int row = 0; row < height; row++)
      {
        stream.Read(buffer, 0, rowLength);

        for (int col = 0; col < width; col++)
        {
          byte r;
          byte g;
          byte b;
          byte a;
          int index;

          index = col * Farbfeld.PixelDataLength;

          r = (byte)(WordHelpers.MakeWordBigEndian(buffer[index], buffer[index + 1]) / 256);
          g = (byte)(WordHelpers.MakeWordBigEndian(buffer[index + 2], buffer[index + 3]) / 256);
          b = (byte)(WordHelpers.MakeWordBigEndian(buffer[index + 4], buffer[index + 5]) / 256);
          a = (byte)(WordHelpers.MakeWordBigEndian(buffer[index + 6], buffer[index + 7]) / 256);

          data[dataIndex] = r;
          data[dataIndex + 1] = g;
          data[dataIndex + 2] = b;
          data[dataIndex + 3] = a;

          dataIndex += 4;
        }
      }

      return new FarbfeldImageData(width, height, data);
    }

    /// <summary>
    /// Tests to see if the specified file contains Farbfeld image data.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="fileName">A string that contains the name of the file to query.</param>
    /// <returns>
    /// <c>true</c> if the file contains Farbfeld image data, otherwise <c>false</c>.
    /// </returns>
    public static bool IsFarbfeldImage(string fileName)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      using (Stream stream = File.OpenRead(fileName))
      {
        return IsFarbfeldImage(stream);
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
    public static bool IsFarbfeldImage(Stream stream)
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

    #endregion
  }
}
