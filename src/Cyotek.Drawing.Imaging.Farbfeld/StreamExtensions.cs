using System.IO;

namespace Cyotek.Drawing.Imaging.Farbfeld
{
  internal static class StreamExtensions
  {
    #region Static Methods

    /// <summary>
    /// Reads a 2-byte unsigned integer from the current stream using big-endian encoding and advances the position of the stream by two bytes.
    /// </summary>
    /// <param name="stream">The stream to read the data from.</param>
    /// <returns>A 2-byte unsigned integer read from the source stream.</returns>
    public static int ReadUInt16BigEndian(this Stream stream)
    {
      return (stream.ReadByte() << 8) | stream.ReadByte();
    }

    /// <summary>
    /// Reads a 4-byte unsigned integer from the current stream using big-endian encoding and advances the position of the stream by four bytes.
    /// </summary>
    /// <param name="stream">The stream to read the data from.</param>
    /// <returns>A 4-byte unsigned integer read from the source stream.</returns>
    public static int ReadUInt32BigEndian(this Stream stream)
    {
      return ((byte)stream.ReadByte() << 24) | ((byte)stream.ReadByte() << 16) | ((byte)stream.ReadByte() << 8) |
             (byte)stream.ReadByte();
    }

    /// <summary>
    /// Writes a 16bit unsigned integer in big-endian format.
    /// </summary>
    /// <param name="stream">The stream to write the data to.</param>
    /// <param name="value">The value to write</param>
    public static void WriteBigEndian(this Stream stream, ushort value)
    {
      stream.WriteByte((byte)(value >> 8));
      stream.WriteByte((byte)(value >> 0));
    }

    /// <summary>
    /// Writes a 32bit unsigned integer in big-endian format.
    /// </summary>
    /// <param name="stream">The stream to write the data to.</param>
    /// <param name="value">The value to write</param>
    public static void WriteBigEndian(this Stream stream, int value)
    {
      stream.WriteByte((byte)((value & 0xFF000000) >> 24));
      stream.WriteByte((byte)((value & 0x00FF0000) >> 16));
      stream.WriteByte((byte)((value & 0x0000FF00) >> 8));
      stream.WriteByte((byte)((value & 0x000000FF) >> 0));
    }

    #endregion
  }
}
