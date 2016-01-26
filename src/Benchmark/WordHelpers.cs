namespace FarbfeldBenchmarks
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

    public static int MakeWordBigEndian(byte value1, byte value2)
    {
      return value1 << 8 | value2;
    }

    public static int MakeWordLittleEndian(byte value1, byte value2)
    {
      return value1 | value2 << 8;
    }

    #endregion
  }
}