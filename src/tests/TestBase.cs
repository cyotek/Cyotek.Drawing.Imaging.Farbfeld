using System;
using System.IO;
using Xunit;

namespace Cyotek.Drawing.Imaging.Farbfeld.Tests
{
  public abstract class TestBase
  {
    #region Properties

    protected FarbfeldImageData AlphaImageData
    {
      get
      {
        int width;
        int height;
        byte[] data;

        width = 1;
        height = 1;
        data = new byte[]
               {
                 255,
                 255,
                 0,
                 128
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
        byte[] data;

        width = 2;
        height = 3;
        data = new byte[]
               {
                 0,
                 0,
                 0,
                 255,
                 255,
                 255,
                 255,
                 255,
                 0,
                 0,
                 0,
                 255,
                 255,
                 255,
                 255,
                 255,
                 0,
                 0,
                 0,
                 255,
                 0,
                 0,
                 0,
                 255
               };

        return new FarbfeldImageData(width, height, data);
      }
    }

    #endregion

    #region Methods

    protected void AssertEqual(Stream stream1, Stream stream2)
    {
      this.AssertEqual(stream1, stream2, 4096);
    }

    protected void AssertEqual(Stream stream1, Stream stream2, int bufferSize)
    {
      byte[] buffer1;
      byte[] buffer2;

      buffer1 = new byte[bufferSize];
      buffer2 = new byte[bufferSize];

      while (stream1.Position < stream1.Length)
      {
        int bytesRead1;

        bytesRead1 = stream1.Read(buffer1, 0, bufferSize);
        stream2.Read(buffer2, 0, bufferSize);

        for (int i = 0; i < bytesRead1; i++)
        {
          Assert.Equal(buffer1[i], buffer2[i]);
        }
      }
    }

    protected void AssertEqual(FarbfeldImageData expected, FarbfeldImageData actual)
    {
      byte[] expectedData;
      byte[] actualData;

      Assert.Equal(expected.Width, actual.Width);
      Assert.Equal(expected.Height, actual.Height);

      expectedData = expected.GetData();
      actualData = actual.GetData();

      Assert.Equal(expectedData.Length, actualData.Length);

      for (int i = 0; i < expectedData.Length; i++)
      {
        Assert.Equal(expectedData[i], actualData[i]);
      }
    }

    protected void AssertFilesEqual(string filePath1, string filePath2)
    {
      this.AssertFilesEqual(filePath1, filePath2, 4096);
    }

    protected void AssertFilesEqual(string filePath1, string filePath2, int bufferSize)
    {
      using (
        Stream stream1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize,
                                        FileOptions.SequentialScan))
      {
        using (
          Stream stream2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize,
                                          FileOptions.SequentialScan))
        {
          this.AssertEqual(stream1, stream2, bufferSize);
        }
      }
    }

    #endregion
  }
}
