using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Cyotek.Drawing.Imaging
{
  public sealed class FarbfeldImageData
  {
    #region Constants

    private static readonly byte[] _emptyData = new byte[0];

    #endregion

    #region Fields

    private byte[] _data;

    #endregion

    #region Constructors

    public FarbfeldImageData(Bitmap image)
    {
      if (image == null)
      {
        throw new ArgumentNullException(nameof(image));
      }

      if (image.PixelFormat == PixelFormat.Format32bppArgb)
      {
        this.LoadFrom(image);
      }
      else
      {
        using (Bitmap copy = image.Copy())
        {
          this.LoadFrom(copy);
        }
      }
    }

    public FarbfeldImageData()
    {
      _data = _emptyData;
    }

    public FarbfeldImageData(int width, int height, byte[] data)
    {
      this.Width = width;
      this.Height = height;
      this.SetData(data);
    }

    #endregion

    #region Properties

    public int Height { get; set; }

    public int Width { get; set; }

    #endregion

    #region Methods

    public byte[] GetData()
    {
      return (byte[])_data.Clone();
    }

    public void SetData(byte[] data)
    {
      int length;

      if (data == null)
      {
        throw new ArgumentNullException(nameof(data));
      }

      length = this.Width * this.Height * 4;

      if (data.Length != length)
      {
        throw new ArgumentException($"Data must contain {length} elements.");
      }

      _data = (byte[])data.Clone();
    }

    public Bitmap ToBitmap()
    {
      Bitmap bitmap;
      BitmapData bitmapData;
      int width;
      int height;
      byte[] data;

      width = this.Width;
      height = this.Height;
      data = this.GetData();

      bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

      bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                                   PixelFormat.Format32bppArgb);

      unsafe
      {
        ArgbColor* pixelPtr;
        int index;

        pixelPtr = (ArgbColor*)bitmapData.Scan0;
        index = 0;

        for (int i = 0; i < width * height; i++)
        {
          *pixelPtr = new ArgbColor(data[index + 3], data[index], data[index + 1], data[index + 2]);
          pixelPtr++;
          index += 4;
        }
      }

      bitmap.UnlockBits(bitmapData);

      return bitmap;
    }

    private void LoadFrom(Bitmap image)
    {
      int width;
      int height;
      byte[] data;

      width = image.Width;
      height = image.Height;
      data = new byte[width * height * 4];

      this.Width = width;
      this.Height = height;
      this.SetData(data);
    }

    #endregion
  }
}
