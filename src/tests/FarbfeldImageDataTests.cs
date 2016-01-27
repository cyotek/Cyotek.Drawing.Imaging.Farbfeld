using System;
using Xunit;

namespace Cyotek.Drawing.Imaging.Farbfeld.Tests
{
  public class FarbfeldImageDataTests
  {
    #region Methods

    [Fact]
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
      ex = Assert.Throws<ArgumentException>(() => { target.SetData(new byte[16]); });

      // assert
      Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
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
