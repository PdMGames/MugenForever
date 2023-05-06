using System;
using System.IO;
using BinaryReaderSystem = System.IO.BinaryReader;

namespace MugenForever.Util
{
    public static class BinaryReader
    {
        public static void ReadJump(Stream fileSream, int size)
        {
            fileSream.Seek(size, SeekOrigin.Current);
        }

        public static void ReadJump(Stream fileSream, int size, SeekOrigin seek)
        {
            fileSream.Seek(size, seek);
        }

        public static int ReadInt(Stream fileSream, int size)
        {
            if (size == 1)
            {
                return fileSream.ReadByte();
            }

            var binaryReader = new BinaryReaderSystem(fileSream);

            return size switch
            {
                2 => binaryReader.ReadInt16(),// [1]00000000 [2]00000000 16bits
                _ => binaryReader.ReadInt32(),// [1]00000000 [2]00000000 [3]00000000 [4]00000000 32bits
            };
        }

        public static bool ReadBool(Stream fileSream)
        {
            return fileSream.ReadByte() == 1;
        }

        public static String ReadString(Stream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return System.Text.Encoding.UTF8.GetString(myVar);
        }

        public static byte ReadByte(Stream fileSream)
        {
            var binaryReader = new BinaryReaderSystem(fileSream);
            return binaryReader.ReadByte();
        }

        public static byte[] ReadBytes(Stream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return myVar;
        }
    }
}
