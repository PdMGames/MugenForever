using UnityEngine;
using System.Collections;

namespace MugenForever { 
    public class CompressorFactory
    {
        public static Compressor getCompressor(CompressorType type)
        {
            switch (type) { 
                case CompressorType.RAW:
                case CompressorType.INVALID:
                    return null;
                case CompressorType.RLE8:
                    return new RLE8();
                case CompressorType.RLE5:
                    return null;
                case CompressorType.LZ5:
                    return null;
            }

            throw new CompressorException("Formato não encontrato");
        }
        
    }
}
