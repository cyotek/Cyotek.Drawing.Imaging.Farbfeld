using System.Runtime.InteropServices;

namespace Cyotek.Drawing.Imaging.Farbfeld
{
  /// <summary>
  /// Represents an ARGB (alpha, red, green, blue) color.
  /// </summary>
  /// <remarks>The color of each pixel is represented as a 32-bit number: 8 bits each for alpha, red, green, and blue (ARGB). Each of the four components is a number from 0 through 255, with 0 representing no intensity and 255 representing full intensity. The alpha component specifies the transparency of the color: 0 is fully transparent, and 255 is fully opaque. To determine the alpha, red, green, or blue component of a color, use the A, R, G, or B property, respectively.</remarks>
  [StructLayout(LayoutKind.Explicit)]
  internal struct ArgbColor
  {
    #region Constants

    [FieldOffset(0)]
    private readonly byte _b;

    [FieldOffset(1)]
    private readonly byte _g;

    [FieldOffset(2)]
    private readonly byte _r;

    [FieldOffset(3)]
    private readonly byte _a;

    #endregion

    #region Constructors

    /// <summary>
    /// Creates a new <see cref="ArgbColor"/> value.
    /// </summary>
    /// <param name="alpha">The alpha.</param>
    /// <param name="red">The red.</param>
    /// <param name="green">The green.</param>
    /// <param name="blue">The blue.</param>
    public ArgbColor(int alpha, int red, int green, int blue)
    {
      _a = (byte)alpha;
      _r = (byte)red;
      _g = (byte)green;
      _b = (byte)blue;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the alpha component value of this <see cref="ArgbColor"/> structure.
    /// </summary>
    public byte A
    {
      get { return _a; }
    }

    /// <summary>
    /// Gets the blue component value of this <see cref="ArgbColor"/> structure.
    /// </summary>
    public byte B
    {
      get { return _b; }
    }

    /// <summary>
    /// Gets the green component value of this <see cref="ArgbColor"/> structure.
    /// </summary>
    public byte G
    {
      get { return _g; }
    }

    /// <summary>
    /// Gets the red component value of this <see cref="ArgbColor"/> structure.
    /// </summary>
    public byte R
    {
      get { return _r; }
    }

    #endregion
  }
}
