using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Cyotek.Drawing.Imaging
{
  public sealed class FarbfeldImageData
  {
    #region Constants

    private static readonly ushort[] _emptyData = new ushort[0];

    #endregion

    #region Fields

    private ushort[] _data;

    #endregion

    #region Constructors

    public FarbfeldImageData(Bitmap image)
    {
      if (image == null)
      {
        throw new ArgumentNullException(nameof(image));
      }

      this.LoadFrom(image);
    }

    public FarbfeldImageData()
    {
      _data = _emptyData;
    }

    [CLSCompliant(false)]
    public FarbfeldImageData(int width, int height, ushort[] data)
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

    [CLSCompliant(false)]
    public ushort[] GetData()
    {
      return (ushort[])_data.Clone();
    }

    [CLSCompliant(false)]
    public void SetData(ushort[] data)
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

      _data = (ushort[])data.Clone();
    }

    public Bitmap ToBitmap()
    {
      Bitmap bitmap;
      BitmapData bitmapData;
      int width;
      int height;
      ushort[] data;

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
          *pixelPtr = new ArgbColor(data[index + 3] / 256, data[index] / 256, data[index + 1] / 256,
                                    data[index + 2] / 256);
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
      ushort[] data;
      ArgbColor[] pixels;

      width = image.Width;
      height = image.Height;
      data = new ushort[width * height * 4];

      pixels = image.GetPixels();

      for (int i = 0; i < pixels.Length; i++)
      {
        ArgbColor pixel;
        int index;
        byte a;
        byte r;
        byte g;
        byte b;

        pixel = pixels[i];
        index = i * 4;

        r = pixel.R;
        g = pixel.G;
        b = pixel.B;
        a = pixel.A;

        // HACK: I have noticed that data files using BE seem to treat values
        // where the right hand byte is zero as if both bytes were zero. I am
        // missing something obvious about this behaviour I'm sure. Regardless,
        // it means converting a byte to a uint16 is going wrong as first came
        // up here:
        // https://forums.cyotek.com/cyotek-palette-editor/colour-palettes-not-displaying-colours/
        // So, I'm taking that same byte and applying it to both bytes for a
        // uint16 - this seems to resolve the issue, althought I would like
        // to know why other programs dislike it when the right hand side is zero

        data[index] = WordHelpers.MakeWordBigEndian(r, r);
        data[index + 1] = WordHelpers.MakeWordBigEndian(g, g);
        data[index + 2] = WordHelpers.MakeWordBigEndian(b, b);
        data[index + 3] = WordHelpers.MakeWordBigEndian(a, a);
      }

      this.Width = width;
      this.Height = height;
      this.SetData(data);
    }

    #endregion
  }
}
