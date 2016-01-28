using System.IO;
using Xunit;

namespace Cyotek.Drawing.Imaging.Farbfeld.Tests
{
  public class FarbfeldDecoderTests : TestBase
  {
    #region Methods

    [Fact]
    public void Decoder_can_read_basic_image_from_file()
    {
      // arrange
      string fileName;
      FarbfeldImageData expected;
      FarbfeldImageData actual;

      fileName = Path.Combine(this.DataPath, "tetromino-l.ff");

      expected = this.TetrominoImageData;

      // act
      actual = FarbfeldDecoder.Decode(fileName);

      // assert
      this.AssertEqual(expected, actual, false);
    }

    [Fact]
    public void Decoder_can_read_basic_image_from_stream()
    {
      // arrange
      string fileName;
      FarbfeldImageData expected;
      FarbfeldImageData actual;

      fileName = Path.Combine(this.DataPath, "tetromino-l.ff");

      expected = this.TetrominoImageData;

      // act
      using (Stream stream = File.OpenRead(fileName))
      {
        actual = FarbfeldDecoder.Decode(stream);
      }

      // assert
      this.AssertEqual(expected, actual, false);
    }

    [Fact]
    public void Decoder_reads_alpha_channel()
    {
      // arrange
      string fileName;
      FarbfeldImageData expected;
      FarbfeldImageData actual;

      fileName = Path.Combine(this.DataPath, "yellow-1x1-semitransparent.png.ff");

      expected = this.AlphaImageData;

      // act
      using (Stream stream = File.OpenRead(fileName))
      {
        actual = FarbfeldDecoder.Decode(stream);
      }

      // assert
      this.AssertEqual(expected, actual, false);
    }

    [Fact]
    public void IsFarbfeldImage_returns_false_for_invalid_image_file()
    {
      // arrange
      string fileName;
      bool actual;

      fileName = Path.Combine(this.DataPath, "tetromino-l.png");

      // act
      actual = FarbfeldDecoder.IsFarbfeldImage(fileName);

      // assert
      Assert.False(actual);
    }

    [Fact]
    public void IsFarbfeldImage_returns_false_for_invalid_image_stream()
    {
      // arrange
      string fileName;
      bool actual;

      fileName = Path.Combine(this.DataPath, "tetromino-l.png");

      // act
      using (Stream stream = File.OpenRead(fileName))
      {
        actual = FarbfeldDecoder.IsFarbfeldImage(stream);
      }

      // assert
      Assert.False(actual);
    }

    [Fact]
    public void IsFarbfeldImage_returns_true_for_image_file()
    {
      // arrange
      string fileName;
      bool actual;

      fileName = Path.Combine(this.DataPath, "tetromino-l.ff");

      // act
      actual = FarbfeldDecoder.IsFarbfeldImage(fileName);

      // assert
      Assert.True(actual);
    }

    [Fact]
    public void IsFarbfeldImage_returns_true_for_image_stream()
    {
      // arrange
      string fileName;
      bool actual;

      fileName = Path.Combine(this.DataPath, "tetromino-l.ff");

      // act
      using (Stream stream = File.OpenRead(fileName))
      {
        actual = FarbfeldDecoder.IsFarbfeldImage(stream);
      }

      // assert
      Assert.True(actual);
    }

    #endregion
  }
}
