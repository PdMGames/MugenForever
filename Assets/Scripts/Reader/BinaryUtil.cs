using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever.Reader
{ 
    public class BinaryUtil : Reader
    {
        public static void ReadJump(Stream fileSream, int size)
        {
            fileSream.Seek(size, SeekOrigin.Current);
        }

        public static int ReadInt(Stream fileSream, int size)
        {
            if (size == 1)
            {
                return fileSream.ReadByte();
            }

            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);

            if (size == 2) 
            {
                // [1]00000000 [2]00000000 16bits
                return BitConverter.ToInt16(myVar, 0);
            }else
            {
                // [1]00000000 [2]00000000 [3]00000000 [4]00000000 32bits
                return BitConverter.ToInt32(myVar, 0);
            }
        }

        public static bool ReadBool(Stream fileSream)
        {
            return fileSream.ReadByte() == 1 ? true : false;
        }

        public static String ReadString(Stream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return System.Text.Encoding.UTF8.GetString(myVar);
        }

        public static byte[] ReadBytes(Stream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return myVar;
        }
    }


}
