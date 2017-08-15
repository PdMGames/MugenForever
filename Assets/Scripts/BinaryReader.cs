using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever.Sff
{
    public class BinaryReader : MonoBehaviour
    {
        protected void ReadJump(FileStream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
        }

        protected int ReadInt(FileStream fileSream, int size)
        {
            if (size == 1)
            {
                return fileSream.ReadByte();
            }
            else if (size == 2)
            {
                return fileSream.ReadByte() + fileSream.ReadByte();
            }

            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return BitConverter.ToInt32(myVar, 0);
        }

        protected bool ReadBool(FileStream fileSream)
        {
            return fileSream.ReadByte() == 1 ? true : false;
        }

        protected String ReadString(FileStream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return System.Text.Encoding.UTF8.GetString(myVar);
        }

        protected byte[] ReadBytes(FileStream fileSream, int size)
        {
            byte[] myVar = new byte[size];
            fileSream.Read(myVar, 0, size);
            return myVar;
        }
    }


}
