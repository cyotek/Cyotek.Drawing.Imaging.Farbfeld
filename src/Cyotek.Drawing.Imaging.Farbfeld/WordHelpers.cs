namespace Cyotek.Drawing.Imaging
{
  internal static class WordHelpers
  {
    #region Static Methods

    public static int MakeDWordBigEndian(byte value1, byte value2, byte value3, byte value4)
    {
      return value1 << 24 | value2 << 16 | value3 << 8 | value4;
    }

    public static int MakeDWordLittleEndian(byte value1, byte value2, byte value3, byte value4)
    {
      return value1 | value2 << 8 | value3 << 16 | value4 << 24;
    }

    public static ushort MakeWordBigEndian(byte value1, byte value2)
    {
      return (ushort)(value1 << 8 | value2);
    }

    public static ushort MakeWordLittleEndian(byte value1, byte value2)
    {
      return (ushort)(value1 | value2 << 8);
    }

    public static ushort SwapBytes(ushort value)
    {
      return (ushort)((value << 8) + (value >> 8));
    }

    #endregion
  }
}
