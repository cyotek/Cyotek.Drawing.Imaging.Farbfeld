using System;
using System.IO;
using NUnit.Framework;

namespace Cyotek.Drawing.Imaging.Farbfeld.Tests
{
  [TestFixture]
  public abstract class TestBase
  {
    #region Properties

    protected FarbfeldImageData AlphaImageData
    {
      get
      {
        int width;
        int height;
        ushort[] data;

        width = 1;
        height = 1;
        data = new ushort[]
               {
                 65535,
                 65535,
                 0,
                 32896
               };

        return new FarbfeldImageData(width, height, data);
      }
    }

    protected string DataPath
    {
      get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"); }
    }

    protected FarbfeldImageData TetrominoImageData
    {
      get
      {
        int width;
        int height;
        ushort[] data;

        width = 2;
        height = 3;
        data = new ushort[]
               {
                 0,
                 0,
                 0,
                 65535,
                 65535,
                 65535,
                 65535,
                 65535,
                 0,
                 0,
                 0,
                 65535,
                 65535,
                 65535,
                 65535,
                 65535,
                 0,
                 0,
                 0,
                 65535,
                 0,
                 0,
                 0,
                 65535
               };

        return new FarbfeldImageData(width, height, data);
      }
    }

    #endregion

    #region Methods

    protected void AssertEqual(Stream expected, Stream actual)
    {
      this.AssertEqual(expected, actual, 4096);
    }

    protected void AssertEqual(Stream expected, Stream actual, int bufferSize)
    {
      byte[] buffer1;
      byte[] buffer2;

      buffer1 = new byte[bufferSize];
      buffer2 = new byte[bufferSize];

      while (expected.Position < expected.Length)
      {
        int bytesRead1;

        bytesRead1 = expected.Read(buffer1, 0, bufferSize);
        actual.Read(buffer2, 0, bufferSize);

        for (int i = 0; i < bytesRead1; i++)
        {
          Assert.AreEqual(buffer1[i], buffer2[i]);
        }
      }
    }

    protected void AssertEqual(FarbfeldImageData expected, FarbfeldImageData actual, bool fuzzyMatch)
    {
      ushort[] expectedData;
      ushort[] actualData;

      Assert.AreEqual(expected.Width, actual.Width);
      Assert.AreEqual(expected.Height, actual.Height);

      expectedData = expected.GetData();
      actualData = actual.GetData();

      Assert.AreEqual(expectedData.Length, actualData.Length);

      for (int i = 0; i < expectedData.Length; i++)
      {
        if (!fuzzyMatch)
        {
          Assert.AreEqual(expectedData[i], actualData[i]);
        }
        else
        {
          byte expectedByte;
          byte actualByte;

          expectedByte = (byte)(expectedData[i] >> 8);
          actualByte = (byte)(actualData[i] >> 8);

          Assert.AreEqual(expectedByte, actualByte);
        }
      }
    }

    protected void AssertFilesEqual(string expected, string actual)
    {
      this.AssertFilesEqual(expected, actual, 4096);
    }

    protected void AssertFilesEqual(string expected, string actual, int bufferSize)
    {
      using (
        Stream expectedStream = new FileStream(expected, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize,
                                               FileOptions.SequentialScan))
      {
        using (
          Stream actualStream = new FileStream(actual, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize,
                                               FileOptions.SequentialScan))
        {
          this.AssertEqual(expectedStream, actualStream, bufferSize);
        }
      }
    }

    #endregion
  }
}
