using System;
using System.Drawing;
using System.IO;

namespace Cyotek.Drawing.Imaging
{
  /// <summary>
  /// Encoder class for the farbfeld image format.
  /// </summary>
  public static class FarbfeldEncoder
  {
    #region Constants

    private static readonly bool _isLittleEndian;

    #endregion

    #region Static Constructors

    static FarbfeldEncoder()
    {
      _isLittleEndian = BitConverter.IsLittleEndian;
    }

    #endregion

    #region Static Methods

    public static void Encode(string fileName, FarbfeldImageData imageData)
    {
      if (string.IsNullOrEmpty(fileName))
      {
        throw new ArgumentNullException(nameof(fileName));
      }

      if (imageData == null)
      {
        throw new ArgumentNullException(nameof(imageData));
      }

      using (Stream stream = File.Create(fileName))
      {
        Encode(stream, imageData);
      }
    }

    public static void Encode(string fileName, Bitmap image)
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
        Encode(stream, image);
      }
    }

    public static void Encode(Stream stream, Bitmap image)
    {
      if (image == null)
      {
        throw new ArgumentNullException(nameof(image));
      }

      Encode(stream, new FarbfeldImageData(image));
    }

    public static void Encode(Stream stream, FarbfeldImageData imageData)
    {
      if (stream == null)
      {
        throw new ArgumentNullException(nameof(imageData));
      }

      if (imageData == null)
      {
        throw new ArgumentNullException(nameof(imageData));
      }

      ushort[] data;
      byte[] header;
      byte[] buffer;
      int width;
      int height;
      int rowLength;
      int dataIndex;

      width = imageData.Width;
      height = imageData.Height;
      data = imageData.GetData();

      rowLength = width * Farbfeld.PixelDataLength;
      dataIndex = 0;

      header = new byte[8];
      header[0] = (byte)'f';
      header[1] = (byte)'a';
      header[2] = (byte)'r';
      header[3] = (byte)'b';
      header[4] = (byte)'f';
      header[5] = (byte)'e';
      header[6] = (byte)'l';
      header[7] = (byte)'d';

      stream.Write(header, 0, 8);
      stream.WriteBigEndian(width);
      stream.WriteBigEndian(height);

      buffer = new byte[rowLength];

      for (int row = 0; row < height; row++)
      {
        for (int col = 0; col < width; col++)
        {
          int index;
          ushort r;
          ushort g;
          ushort b;
          ushort a;

          index = col * Farbfeld.PixelDataLength;

          r = data[dataIndex];
          g = data[dataIndex + 1];
          b = data[dataIndex + 2];
          a = data[dataIndex + 3];

          buffer[index] = (byte)(r >> 8);
          buffer[index + 1] = (byte)r;
          buffer[index + 2] = (byte)(g >> 8);
          buffer[index + 3] = (byte)g;
          buffer[index + 4] = (byte)(b >> 8);
          buffer[index + 5] = (byte)b;
          buffer[index + 6] = (byte)(a >> 8);
          buffer[index + 7] = (byte)a;

          dataIndex += 4;
        }

        stream.Write(buffer, 0, rowLength);
      }
    }

    #endregion
  }
}
