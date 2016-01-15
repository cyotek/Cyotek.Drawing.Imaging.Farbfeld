using System.Drawing;
using System.Drawing.Imaging;

namespace FarbfeldViewer
{
  internal static class ImageExtensions
  {
    #region Static Methods

    public static Bitmap Copy(this Image image)
    {
      return Copy(image, Color.Transparent);
    }

    public static Bitmap Copy(this Image image, Color transparentColor)
    {
      Bitmap copy;

      copy = new Bitmap(image.Size.Width, image.Size.Height, PixelFormat.Format32bppArgb);

      using (Graphics g = Graphics.FromImage(copy))
      {
        g.Clear(transparentColor);
        g.PageUnit = GraphicsUnit.Pixel;
        g.DrawImage(image, new Rectangle(Point.Empty, image.Size));
      }

      return copy;
    }

    #endregion
  }
}
