using UnityEngine;
using System.Collections;
using System;

namespace MugenForever {
    public class CompressorException : Exception
    {
        public CompressorException(string message) : base(message)
        {
        }
    }
}