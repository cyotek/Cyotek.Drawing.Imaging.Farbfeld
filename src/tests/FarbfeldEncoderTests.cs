using System.IO;
using NUnit.Framework;

namespace Cyotek.Drawing.Imaging.Farbfeld.Tests
{
  public class FarbfeldEncoderTests : TestBase
  {
    #region Methods

    [Test]
    public void Encoder_can_write_basic_image_file()
    {
      // arrange
      string target;
      string source;
      FarbfeldImageData data;

      source = Path.Combine(this.DataPath, "tetromino-l.ff");
      target = Path.GetTempFileName();
      data = this.TetrominoImageData;

      // act
      FarbfeldEncoder.Encode(target, data);

      // assert
      this.AssertFilesEqual(source, target);
      File.Delete(target);
    }

    [Test]
    public void Encoder_can_write_basic_image_stream()
    {
      // arrange
      string target;
      string source;
      FarbfeldImageData data;

      source = Path.Combine(this.DataPath, "tetromino-l.ff");
      target = Path.GetTempFileName();
      data = this.TetrominoImageData;

      // act
      using (Stream stream = File.Create(target))
      {
        FarbfeldEncoder.Encode(stream, data);
      }

      // assert
      this.AssertFilesEqual(source, target);
      File.Delete(target);
    }

    [Test]
    public void Encoder_stores_alpha_channel()
    {
      // arrange
      string target;
      string source;
      FarbfeldImageData data;

      source = Path.Combine(this.DataPath, "yellow-1x1-semitransparent.png.ff");
      target = Path.GetTempFileName();
      data = FarbfeldDecoder.Decode(source);

      // act
      FarbfeldEncoder.Encode(target, data);

      // assert
      this.AssertFilesEqual(source, target);
      File.Delete(target);
    }

    #endregion
  }
}
