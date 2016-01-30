using System;
using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace Cyotek.Drawing.Imaging.Farbfeld.Tests
{
  public class FarbfeldImageDataTests : TestBase
  {
    #region Methods

    [Test]
    public void Constructor_with_image_should_extract_data_properly()
    {
      // arrange
      FarbfeldImageData target;
      FarbfeldImageData expected;

      expected = FarbfeldDecoder.Decode(Path.Combine(this.DataPath, "polygon.png.ff"));

      // act
      using (Bitmap bitmap = (Bitmap)Image.FromFile(Path.Combine(this.DataPath, "polygon.png")))
      {
        target = new FarbfeldImageData(bitmap);
      }

      // assert
      // As .NET images used 0-255 for RGB colors, there might not be a direct match between
      // the uint16 and byte versions. So when we compare the colours, we'll discard one of the
      // bytes and compare only that
      this.AssertEqual(expected, target, true);
    }

    [Test]
    public void SetData_with_invalid_argument_throws_exception()
    {
      // arrange
      FarbfeldImageData target;
      Exception ex;
      string expectedMessage;

      target = new FarbfeldImageData
               {
                 Width = 32,
                 Height = 32
               };

      expectedMessage = "Data must contain 4096 elements.";

      // act
      ex = Assert.Throws<ArgumentException>(() => { target.SetData(new ushort[16]); });

      // assert
      Assert.AreEqual(expectedMessage, ex.Message);
    }

    [Test]
    public void SetData_with_null_argument_throws_exception()
    {
      // arrange
      FarbfeldImageData target;

      target = new FarbfeldImageData();

      // act & assert
      Assert.Throws<ArgumentNullException>(() => { target.SetData(null); });
    }

    #endregion
  }
}
