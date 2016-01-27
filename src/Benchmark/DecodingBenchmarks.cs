using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Cyotek.Drawing.Imaging;

// http://yoda.arachsys.com/csharp/benchmark.html

namespace FarbfeldBenchmarks
{
  internal static class DecodingBenchmarks
  {
    #region Static Properties

    static string SampleFileName
    {
      get { return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\res\dragon.ff")); }
    }

    static string SamplePngFileName
    {
      get { return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\res\dragon.png")); }
    }

    #endregion

    #region Static Methods

    public static void Check()
    {
      ArgbColor[] sourcePixels;

      if (_testResults == null || _testResults.Count != _iterations)
      {
        throw new Exception("No output.");
      }

      using (Bitmap image = (Bitmap)Image.FromFile(SamplePngFileName))
      {
        sourcePixels = image.GetPixels();
      }

      for (int i = 0; i < _iterations; i++)
      {
        ArgbColor[] pixels;

        pixels = _testResults[i];

        if (pixels.Length != sourcePixels.Length)
        {
          throw new Exception("Data length mismatch.");
        }

        for (int j = 0; j < pixels.Length; j++)
        {
          ArgbColor expected;
          ArgbColor actual;

          expected = sourcePixels[j];
          actual = pixels[j];

          if (expected.A != actual.A || expected.R != actual.R || expected.G != actual.G || expected.B != actual.B)
          {
            throw new Exception(
              $"Data at position {j} mismatch.\n\nExpected: R:{expected.R} G:{expected.G} B:{expected.B} A:{expected.A}\nActual: R:{actual.R} G:{actual.G} B:{actual.B} A:{actual.A}");
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
    public static void Test1_read_a_byte_at_a_time_original()
    {
      int count;
      string sampleFileName;
      List<ArgbColor[]> results;

      count = _iterations;
      sampleFileName = SampleFileName;
      results = new List<ArgbColor[]>();

      for (int i = 0; i < count; i++)
      {
        using (Stream stream = File.OpenRead(sampleFileName))
        {
          byte[] header;
          int width;
          int height;
          int length;
          ArgbColor[] pixels;

          header = new byte[8];

          stream.Read(header, 0, header.Length);
          width = stream.ReadUInt32BigEndian();
          height = stream.ReadUInt32BigEndian();
          length = width * height;
          pixels = new ArgbColor[length];

          for (int j = 0; j < length; j++)
          {
            int r;
            int g;
            int b;
            int a;

            r = stream.ReadUInt16BigEndian() / 256;
            g = stream.ReadUInt16BigEndian() / 256;
            b = stream.ReadUInt16BigEndian() / 256;
            a = stream.ReadUInt16BigEndian() / 256;

            pixels[j] = new ArgbColor(a, r, g, b);
          }

          results.Add(pixels);
        }
      }

      _testResults = results;
    }

    [Benchmark]
    public static void Test2_read_all_pixel_data_at_once()
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      int count;
      string sampleFileName;
      List<ArgbColor[]> results;

      count = _iterations;
      sampleFileName = SampleFileName;
      results = new List<ArgbColor[]>();

      for (int i = 0; i < count; i++)
      {
        using (Stream stream = File.OpenRead(sampleFileName))
        {
          byte[] header;
          byte[] buffer;
          int width;
          int height;
          int length;
          ArgbColor[] pixels;

          header = new byte[8];

          stream.Read(header, 0, header.Length);

          stream.Read(header, 0, header.Length);
          width = WordHelpers.MakeDWordBigEndian(header[0], header[1], header[2], header[3]);
          height = WordHelpers.MakeDWordBigEndian(header[4], header[5], header[6], header[7]);
          length = width * height;
          pixels = new ArgbColor[length];
          buffer = new byte[width * height * recordLength];

          stream.Read(buffer, 0, buffer.Length);

          for (int j = 0; j < length; j++)
          {
            int r;
            int g;
            int b;
            int a;
            int index;

            index = j * recordLength;

            r = WordHelpers.MakeWordBigEndian(buffer[index], buffer[index + 1]) / 256;
            g = WordHelpers.MakeWordBigEndian(buffer[index + 2], buffer[index + 3]) / 256;
            b = WordHelpers.MakeWordBigEndian(buffer[index + 4], buffer[index + 5]) / 256;
            a = WordHelpers.MakeWordBigEndian(buffer[index + 6], buffer[index + 7]) / 256;

            pixels[j] = new ArgbColor(a, r, g, b);
          }
          results.Add(pixels);
        }
      }

      _testResults = results;
    }

    [Benchmark]
    public static void Test3_read_one_pixel_at_a_time()
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      int count;
      string sampleFileName;
      List<ArgbColor[]> results;
      byte[] buffer;

      count = _iterations;
      sampleFileName = SampleFileName;
      results = new List<ArgbColor[]>();
      buffer = new byte[recordLength];

      for (int i = 0; i < count; i++)
      {
        using (Stream stream = File.OpenRead(sampleFileName))
        {
          byte[] header;
          int width;
          int height;
          int length;
          ArgbColor[] pixels;

          header = new byte[8];

          stream.Read(header, 0, header.Length);

          stream.Read(header, 0, header.Length);
          width = WordHelpers.MakeDWordBigEndian(header[0], header[1], header[2], header[3]);
          height = WordHelpers.MakeDWordBigEndian(header[4], header[5], header[6], header[7]);
          length = width * height;
          pixels = new ArgbColor[length];

          for (int j = 0; j < length; j++)
          {
            int r;
            int g;
            int b;
            int a;

            stream.Read(buffer, 0, recordLength);

            r = WordHelpers.MakeWordBigEndian(buffer[0], buffer[1]) / 256;
            g = WordHelpers.MakeWordBigEndian(buffer[2], buffer[3]) / 256;
            b = WordHelpers.MakeWordBigEndian(buffer[4], buffer[5]) / 256;
            a = WordHelpers.MakeWordBigEndian(buffer[6], buffer[7]) / 256;

            pixels[j] = new ArgbColor(a, r, g, b);
          }
          results.Add(pixels);
        }
      }

      _testResults = results;
    }

    [Benchmark]
    public static void Test4_read_pixel_data_by_row()
    {
      const int recordLength = 4 * 2; // four values per pixel, two bytes per value

      int count;
      string sampleFileName;
      List<ArgbColor[]> results;

      count = _iterations;
      sampleFileName = SampleFileName;
      results = new List<ArgbColor[]>();

      for (int i = 0; i < count; i++)
      {
        using (Stream stream = File.OpenRead(sampleFileName))
        {
          byte[] header;
          int width;
          int height;
          int length;
          int rowLength;
          ArgbColor[] pixels;
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

              r = WordHelpers.MakeWordBigEndian(buffer[index], buffer[index + 1]) / 256;
              g = WordHelpers.MakeWordBigEndian(buffer[index + 2], buffer[index + 3]) / 256;
              b = WordHelpers.MakeWordBigEndian(buffer[index + 4], buffer[index + 5]) / 256;
              a = WordHelpers.MakeWordBigEndian(buffer[index + 6], buffer[index + 7]) / 256;

              pixels[row * width + col] = new ArgbColor(a, r, g, b);
            }
          }

          results.Add(pixels);
        }
      }

      _testResults = results;
    }

    #endregion

    #region Other

    private static int _iterations = 1;

    private static List<ArgbColor[]> _testResults;

    #endregion
  }
}
