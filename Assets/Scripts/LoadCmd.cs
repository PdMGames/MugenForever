﻿using Assets.Scripts.IO.CMD.Impl;
using System.IO;
using UnityEngine;

namespace MugenForever.Scripts
{
    public class LoadCmd : MonoBehaviour
    {

        void Start()
        {
            FileStream fs = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\kfm.air");

            new CMDImpl(fs);
        }

    }
}
