using System.IO;
using Cyotek.Drawing.Imaging;

namespace FarbfeldBenchmarks
{
  internal sealed class ImageData
  {
    #region Constructors

    public ImageData(string fileName)
    {
      this.Load(fileName);
    }

    #endregion

    #region Properties

    public int Height { get; private set; }

    public ArgbColor[] PixelData { get; private set; }

    public int Width { get; private set; }

    #endregion

    #region Methods

    private void Load(string fileName)
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      int width;
      int height;
      ArgbColor[] pixels;

      using (Stream stream = File.OpenRead(fileName))
      {
        byte[] header;
        int length;
        int rowLength;
        byte[] buffer;

        header = new byte[8];

        stream.Read(header, 0, header.Length);

        stream.Read(header, 0, header.Length);
        width = WordHelpers.MakeDWordBigEndian(header[0], header[1], header[2], header[3]);
        height = WordHelpers.MakeDWordBigEndian(header[4], header[5], header[6], header[7]);
        length = width * height;
        pixels = new ArgbColor[length];
        rowLength = width * recordLength;
        buffer = new byte[rowLength];

        for (int row = 0; row < height; row++)
        {
          stream.Read(buffer, 0, rowLength);

          for (int col = 0; col < width; col++)
          {
            int r;
            int g;
            int b;
            int a;
            int index;

            index = col * recordLength;

            r = WordHelpers.MakeWordBigEndian(buffer[index], buffer[index + 1]) / 257;
            g = WordHelpers.MakeWordBigEndian(buffer[index + 2], buffer[index + 3]) / 257;
            b = WordHelpers.MakeWordBigEndian(buffer[index + 4], buffer[index + 5]) / 257;
            a = WordHelpers.MakeWordBigEndian(buffer[index + 6], buffer[index + 7]) / 257;

            pixels[row * width + col] = new ArgbColor(a, r, g, b);
          }
        }
      }

      this.Width = width;
      this.Height = height;
      this.PixelData = pixels;
    }

    #endregion
  }
}
