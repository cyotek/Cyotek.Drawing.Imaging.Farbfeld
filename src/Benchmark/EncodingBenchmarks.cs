using System;
using System.Collections.Generic;
using System.IO;
using Cyotek.Drawing.Imaging;

namespace FarbfeldBenchmarks
{
  internal static class EncodingBenchmarks
  {
    #region Static Properties

    static string SampleFileName
    {
      get { return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\res\dragon.ff")); }
    }

    #endregion

    #region Static Methods

    public static void Check()
    {
      try
      {
        ImageData original;
        ArgbColor[] originalPixels;

        if (_testResults == null || _testResults.Count != _iterations)
        {
          throw new Exception("No output.");
        }

        original = new ImageData(SampleFileName);
        originalPixels = original.PixelData;

        foreach (string fileName in _testResults)
        {
          ImageData copy;
          ArgbColor[] copyPixels;

          copy = new ImageData(fileName);
          copyPixels = copy.PixelData;

          if (copy.Width != original.Width || copy.Height != original.Height ||
              copyPixels.Length != originalPixels.Length)
          {
            throw new Exception("Invalid data.");
          }

          for (int j = 0; j < copyPixels.Length; j++)
          {
            ArgbColor expected;
            ArgbColor actual;

            expected = originalPixels[j];
            actual = copyPixels[j];

            if (expected.A != actual.A || expected.R != actual.R || expected.G != actual.G || expected.B != actual.B)
            {
              throw new Exception(
                $"Data at position {j} mismatch.\n\nExpected: R:{expected.R} G:{expected.G} B:{expected.B} A:{expected.A}\nActual: R:{actual.R} G:{actual.G} B:{actual.B} A:{actual.A}");
            }
          }
        }
      }
      finally
      {
        if (_testResults != null)
        {
          foreach (string fileName in _testResults)
          {
            File.Delete(fileName);
          }
        }
      }
    }

    public static void Init(string[] args)
    {
      if (args.Length > 0)
      {
        _iterations = int.Parse(args[0]);
      }
    }

    public static void Reset()
    {
      _testResults = null;
    }

    [Benchmark]
    public static void Test1_write_a_byte_at_a_time_original()
    {
      ImageData source;
      List<string> results;
      int count;

      source = new ImageData(SampleFileName);
      results = new List<string>();
      count = _iterations;

      for (int i = 0; i < count; i++)
      {
        string fileName;

        fileName = Path.GetTempFileName();

        using (Stream stream = File.Create(fileName))
        {
          ArgbColor[] pixels;

          stream.WriteByte((byte)'f');
          stream.WriteByte((byte)'a');
          stream.WriteByte((byte)'r');
          stream.WriteByte((byte)'b');
          stream.WriteByte((byte)'f');
          stream.WriteByte((byte)'e');
          stream.WriteByte((byte)'l');
          stream.WriteByte((byte)'d');

          stream.WriteBigEndian(source.Width);
          stream.WriteBigEndian(source.Height);

          pixels = source.PixelData;

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

          results.Add(fileName);
        }
      }

      _testResults = results;
    }

    [Benchmark]
    public static void Test2_write_all_pixel_data_at_once()
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      ImageData source;
      List<string> results;
      int count;

      source = new ImageData(SampleFileName);
      results = new List<string>();
      count = _iterations;

      for (int i = 0; i < count; i++)
      {
        string fileName;

        fileName = Path.GetTempFileName();

        using (Stream stream = File.Create(fileName))
        {
          ArgbColor[] pixels;
          byte[] header;
          byte[] data;
          int width;
          int height;
          int length;

          width = source.Width;
          height = source.Height;
          length = width * height * recordLength;
          header = new byte[8];
          data = new byte[length];

          header[0] = (byte)'f';
          header[1] = (byte)'a';
          header[2] = (byte)'r';
          header[3] = (byte)'b';
          header[4] = (byte)'f';
          header[5] = (byte)'e';
          header[6] = (byte)'l';
          header[7] = (byte)'d';

          pixels = source.PixelData;

          for (int j = 0; j < width * height; j++)
          {
            int index;
            ushort r;
            ushort g;
            ushort b;
            ushort a;
            ArgbColor pixel;

            pixel = pixels[j];

            index = j * recordLength;

            r = (ushort)(pixel.R * 256);
            g = (ushort)(pixel.G * 256);
            b = (ushort)(pixel.B * 256);
            a = (ushort)(pixel.A * 256);

            data[index] = (byte)(r >> 8);
            data[index + 1] = (byte)r;
            data[index + 2] = (byte)(g >> 8);
            data[index + 3] = (byte)g;
            data[index + 4] = (byte)(b >> 8);
            data[index + 5] = (byte)b;
            data[index + 6] = (byte)(a >> 8);
            data[index + 7] = (byte)a;
          }

          stream.Write(header, 0, 8);
          stream.WriteBigEndian(width);
          stream.WriteBigEndian(height);
          stream.Write(data, 0, length);

          results.Add(fileName);
        }
      }

      _testResults = results;
    }

    [Benchmark]
    public static void Test3_write_one_pixel_at_a_time()
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      ImageData source;
      List<string> results;
      int count;

      source = new ImageData(SampleFileName);
      results = new List<string>();
      count = _iterations;

      for (int i = 0; i < count; i++)
      {
        string fileName;

        fileName = Path.GetTempFileName();

        using (Stream stream = File.Create(fileName))
        {
          ArgbColor[] pixels;
          byte[] header;
          byte[] data;
          int width;
          int height;

          width = source.Width;
          height = source.Height;
          header = new byte[8];
          data = new byte[recordLength];

          header[0] = (byte)'f';
          header[1] = (byte)'a';
          header[2] = (byte)'r';
          header[3] = (byte)'b';
          header[4] = (byte)'f';
          header[5] = (byte)'e';
          header[6] = (byte)'l';
          header[7] = (byte)'d';

          pixels = source.PixelData;

          stream.Write(header, 0, 8);
          stream.WriteBigEndian(width);
          stream.WriteBigEndian(height);

          for (int j = 0; j < width * height; j++)
          {
            ushort r;
            ushort g;
            ushort b;
            ushort a;
            ArgbColor pixel;

            pixel = pixels[j];

            r = (ushort)(pixel.R * 256);
            g = (ushort)(pixel.G * 256);
            b = (ushort)(pixel.B * 256);
            a = (ushort)(pixel.A * 256);

            data[0] = (byte)(r >> 8);
            data[1] = (byte)r;
            data[2] = (byte)(g >> 8);
            data[3] = (byte)g;
            data[4] = (byte)(b >> 8);
            data[5] = (byte)b;
            data[6] = (byte)(a >> 8);
            data[7] = (byte)a;

            stream.Write(data, 0, recordLength);
          }

          results.Add(fileName);
        }
      }

      _testResults = results;
    }

    [Benchmark]
    public static void Test4_write_pixel_data_by_row()
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      ImageData source;
      List<string> results;
      int count;

      source = new ImageData(SampleFileName);
      results = new List<string>();
      count = _iterations;

      for (int i = 0; i < count; i++)
      {
        string fileName;

        fileName = Path.GetTempFileName();

        using (Stream stream = File.Create(fileName))
        {
          ArgbColor[] pixels;
          byte[] header;
          byte[] data;
          int width;
          int height;
          int rowLength;

          width = source.Width;
          rowLength = width * recordLength;
          height = source.Height;
          header = new byte[8];
          data = new byte[rowLength];

          header[0] = (byte)'f';
          header[1] = (byte)'a';
          header[2] = (byte)'r';
          header[3] = (byte)'b';
          header[4] = (byte)'f';
          header[5] = (byte)'e';
          header[6] = (byte)'l';
          header[7] = (byte)'d';

          pixels = source.PixelData;

          stream.Write(header, 0, 8);
          stream.WriteBigEndian(width);
          stream.WriteBigEndian(height);

          for (int row = 0; row < height; row++)
          {
            for (int col = 0; col < width; col++)
            {
              int index;
              ushort r;
              ushort g;
              ushort b;
              ushort a;
              ArgbColor pixel;

              index = col * recordLength;
              pixel = pixels[row * width + col];

              r = (ushort)(pixel.R * 256);
              g = (ushort)(pixel.G * 256);
              b = (ushort)(pixel.B * 256);
              a = (ushort)(pixel.A * 256);

              data[index] = (byte)(r >> 8);
              data[index + 1] = (byte)r;
              data[index + 2] = (byte)(g >> 8);
              data[index + 3] = (byte)g;
              data[index + 4] = (byte)(b >> 8);
              data[index + 5] = (byte)b;
              data[index + 6] = (byte)(a >> 8);
              data[index + 7] = (byte)a;
            }

            stream.Write(data, 0, rowLength);
          }

          results.Add(fileName);
        }
      }

      _testResults = results;
    }

    #endregion

    #region Other

    private static int _iterations = 1;

    private static List<string> _testResults;

    #endregion
  }
}
