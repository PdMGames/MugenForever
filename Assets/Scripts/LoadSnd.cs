using MugenForever.IO.SND;
using MugenForever.Util;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

namespace MugenForever.Scripts
{
    [RequireComponent(typeof(AudioSource))]
    internal class LoadSnd : MonoBehaviour
    {

        private AudioSource _audioSource;
        private void Start()
        {
            using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\mugen_2010\\chars\\kfm\\kfm.snd");
            ISND snd = new SNDImpl(st);
                        
            AudioSource audioSource = GetComponent<AudioSource>();
            /* audioSource.clip = WavUtility.ToAudioClip(snd.Sounds[5][0]);
             audioSource.Play();*/

            audioSource.clip = snd.Sounds[1][1];
            audioSource.Play();
        }

        private void Update()
        {
            
        }

    }
}
