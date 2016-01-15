using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Cyotek.Drawing.Imaging.Farbfeld;
using Cyotek.Windows.Forms;

namespace FarbfeldViewer
{
  public partial class MainForm : Form
  {
    #region Fields

    private Bitmap _currentImage;

    private string _fileName;

    private bool _isFarbfeldImage;

    #endregion

    #region Constructors

    public MainForm()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Form.Load"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data. </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.UpdateText();
      this.UpdateZoomLabel();
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Form.Shown"/> event.
    /// </summary>
    /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the event data. </param>
    protected override void OnShown(EventArgs e)
    {
      string[] args;

      base.OnShown(e);

      args = Environment.GetCommandLineArgs();

      if (args.Length > 1)
      {
        string fileName;

        fileName = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, args[1]));

        this.OpenFile(fileName);
      }
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (AboutDialog dialog = new AboutDialog())
      {
        dialog.ShowDialog(this);
      }
    }

    private void copyToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (_currentImage != null)
      {
        ClipboardHelpers.CopyImage(_currentImage);
      }
      else
      {
        MessageBox.Show("Nothing to copy.", "Copy Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private string FormatPoint(Point point)
    {
      return $"{point.X}, {point.Y}";
    }

    private void OpenFile()
    {
      if (openFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        this.OpenFile(openFileDialog.FileName);
      }
    }

    private void OpenFile(string fileName)
    {
      if (File.Exists(fileName))
      {
#if !DEBUG
        try
#endif
        {
          Bitmap image;
          FarbfeldDecoder decoder;

          this.SetStatus("Opening...");

          decoder = new FarbfeldDecoder();

          _isFarbfeldImage = decoder.IsFarbfeldImage(fileName);

          if (_isFarbfeldImage)
          {
            // opening a farbfeld image
            image = decoder.Decode(fileName);
          }
          else
          {
            // opening some other image
            // copy it (to avoid .NET file locks) and also to convert it to 32bpp
            using (Image src = Image.FromFile(fileName))
            {
              image = src.Copy();
            }
          }

          this.SetImage(image);
          _fileName = fileName;
          this.UpdateText();
        }
#if DEBUG
        try
        { }
#endif
        catch (Exception ex)
        {
          MessageBox.Show($"Failed to open image. {ex.GetBaseException(). Message}", "Open File", MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation);
        }
        finally
        {
          this.ResetStatus();
        }
      }
      else
      {
        MessageBox.Show($"Cannot find file '{fileName}'", "Open File", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    private void openToolStripMenuItem_Click(object sender, EventArgs e)
    {
      this.OpenFile();
    }

    private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (ClipboardHelpers.HasImage())
      {
        Bitmap image;

        image = ClipboardHelpers.GetImage().
                                 Copy();

        this.SetImage(image);
        _fileName = null;
        _isFarbfeldImage = false;
        this.UpdateText();
      }
      else
      {
        MessageBox.Show("Nothing to paste.", "Paste Image", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void previewImageBox_MouseLeave(object sender, EventArgs e)
    {
      cursorToolStripStatusLabel.Text = string.Empty;
    }

    private void previewImageBox_MouseMove(object sender, MouseEventArgs e)
    {
      this.UpdateCursorPosition(e.Location);
    }

    private void previewImageBox_Zoomed(object sender, ImageBoxZoomEventArgs e)
    {
      this.UpdateZoomLabel();
    }

    private void ResetStatus()
    {
      this.SetStatus(string.Empty);
    }

    private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (_currentImage != null)
      {
        this.SaveFile();
      }
      else
      {
        MessageBox.Show("Nothing to save.", "Save File As", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void SaveFile()
    {
      if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
      {
        this.SaveFile(saveFileDialog.FileName);
      }
    }

    private void SaveFile(string fileName)
    {
#if !DEBUG
        try
#endif
      {
        this.SetStatus("Saving...");

        new FarbfeldEncoder().Encode(fileName, _currentImage);

        _fileName = fileName;
        _isFarbfeldImage = true;
        this.UpdateText();
      }
#if DEBUG
      try
      { }
#endif
      catch (Exception ex)
      {
        MessageBox.Show($"Failed to open image. {ex.GetBaseException(). Message}", "Open File", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
      }
      finally
      {
        this.ResetStatus();
      }
    }

    private void saveToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (_currentImage != null)
      {
        if (_isFarbfeldImage)
        {
          this.SaveFile(_fileName);
        }
        else
        {
          this.SaveFile();
        }
      }
      else
      {
        MessageBox.Show("Nothing to save.", "Save File", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void SetImage(Bitmap image)
    {
      previewImageBox.Image = null;

      _currentImage?.Dispose();

      _currentImage = image;

      previewImageBox.Image = image;

      sizeToolStripStatusLabel.Text = $"{image.Width} x {image.Height}";
    }

    private void SetStatus(string statusText)
    {
      Cursor.Current = string.IsNullOrEmpty(statusText) ? Cursors.Default : Cursors.WaitCursor;

      statusToolStripStatusLabel.Text = statusText;

      statusStrip.Refresh();
    }

    private void UpdateCursorPosition(Point location)
    {
      if (previewImageBox.IsPointInImage(location))
      {
        Point point;

        point = previewImageBox.PointToImage(location);

        cursorToolStripStatusLabel.Text = this.FormatPoint(point);
      }
      else
      {
        cursorToolStripStatusLabel.Text = string.Empty;
      }
    }

    private void UpdateText()
    {
      string name;

      name = string.IsNullOrEmpty(_fileName) ? "Untitled" : Path.GetFileName(_fileName);

      this.Text = $"{name} - {Application.ProductName}";
    }

    private void UpdateZoomLabel()
    {
      zoomToolStripStatusLabel.Text = $"{previewImageBox.Zoom}%";
    }

    #endregion
  }
}
