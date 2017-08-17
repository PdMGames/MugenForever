using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MugenForever.Reader
{
    public class Reader : MonoBehaviour
    {
        public string fileName;

        public virtual void ReadFromFile(string pathFile)
        {
        }
    }
}
