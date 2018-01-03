using UnityEngine;
using UnityEditor;

namespace MugenForever { 
    
    public interface Compressor
    {
        byte[] compress(byte[] buff);
        byte[] descompress(byte[] buff);
    }

}