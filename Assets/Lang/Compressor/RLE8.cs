using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MugenForever {
    public class RLE8 : Compressor
    {
        public byte[] compress(byte[] buff)
        {
            throw new System.NotImplementedException();
        }

        public byte[] descompress(byte[] buff)
        {
            List<byte> result = new List<byte>();
            Debug.Log("Buff" + buff);
            //System.IO.Stream buffStream;
            using (BinaryWriter binWriter = new BinaryWriter(new MemoryStream()))
            {
                binWriter.Write(buff);

                Debug.Log("DentroAqui");

                System.Drawing.Bitmap img = new System.Drawing.Bitmap(binWriter.BaseStream);

                Debug.Log("Tamanho" + img.Width);

                /*buffStream = binWriter.BaseStream;
                long totalLoop = (buffStream.Length / 2);
                Debug.Log("TotalLoop:" + totalLoop);
                for (int i=0; i<totalLoop; i++) { 
                    int count_byte = buffStream.ReadByte();
                    Debug.Log("count_byte:" + count_byte);
                    int run_byte = buffStream.ReadByte();
                    Debug.Log("run_byte:" + run_byte);
                    for (int z=0; z<count_byte; z++)
                    {
                        result.Add((byte)run_byte);
                    }
                    Debug.Log("-------");
                }*/

                /*int one_byte = buff[0];
                if ((one_byte & 0xC0) == 0x40){
                    Debug.Log("Entrou.." + one_byte);
                    int color = buffStream.ReadByte();
                    for(int i=0; i> one_byte; i++) { 
                        result.Add((byte)color);
                    }
                }else{
                    result.Add((byte)one_byte);
                }*/
            }
            Debug.Log(result.Count);
            return result.ToArray();
        }
    }
}
