using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace FarbfeldViewer
{
  internal static class ClipboardHelpers
  {
    #region Constants

    public const string PngFormat = "PNG";

    #endregion

    #region Static Methods

    public static bool CopyImage(Image image)
    {
      bool result;

      // http://csharphelper.com/blog/2014/09/copy-an-irregular-area-from-one-picture-to-another-in-c/

      try
      {
        IDataObject data;
        Bitmap opaqueBitmap;
        Bitmap transparentBitmap;
        MemoryStream transparentBitmapStream;

        data = new DataObject();
        opaqueBitmap = null;
        transparentBitmap = null;
        transparentBitmapStream = null;

        try
        {
          opaqueBitmap = image.Copy(Color.White);
          transparentBitmap = image.Copy(Color.Transparent);

          transparentBitmapStream = new MemoryStream();
          transparentBitmap.Save(transparentBitmapStream, ImageFormat.Png);

          data.SetData(DataFormats.Bitmap, opaqueBitmap);
          data.SetData(PngFormat, false, transparentBitmapStream);

          Clipboard.Clear();
          Clipboard.SetDataObject(data, true);
        }
        finally
        {
          opaqueBitmap?.Dispose();
          transparentBitmapStream?.Dispose();
          transparentBitmap?.Dispose();
        }

        result = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to copy image. {ex.GetBaseException(). Message}", "Copy Image", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

        result = false;
      }

      return result;
    }

    public static Image GetImage()
    {
      Image result;

      // http://csharphelper.com/blog/2014/09/paste-a-png-format-image-with-a-transparent-background-from-the-clipboard-in-c/

      result = null;

      try
      {
        if (Clipboard.ContainsData(PngFormat))
        {
          object data;

          data = Clipboard.GetData(PngFormat);

          if (data != null)
          {
            Stream stream;

            stream = data as MemoryStream;

            if (stream == null)
            {
              byte[] buffer;

              buffer = data as byte[];

              if (buffer != null)
              {
                stream = new MemoryStream(buffer);
              }
            }

            if (stream != null)
            {
              result = Image.FromStream(stream);

              stream.Dispose();
            }
          }
        }

        if (result == null)
        {
          result = Clipboard.GetImage();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to obtain image. {ex.GetBaseException(). Message}", "Paste Image", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
      }

      return result;
    }

    public static bool HasImage()
    {
      bool result;

      try
      {
        result = Clipboard.ContainsImage();
      }
      catch
      {
        result = false;
      }

      return result;
    }

    #endregion
  }
}
