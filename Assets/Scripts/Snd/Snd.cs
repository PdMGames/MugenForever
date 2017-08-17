using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace MugenForever.Snd
{
    /**
     00-11 "ElecbyteSnd\0" signature	[12]
12-15 4 verhi, 4 verlo	[04]
16-19 Number of sounds	[04]
20-23 File offset where first subfile is located.	[04]
24-511 Blank; can be used for comments.	[488]
    */
    [RequireComponent(typeof(AudioSource))]
    public class Snd : MugenForever.Reader.Binary
    {
        public string signature;
        public string version;
        public int totalSound;
        public int offsetSubFile;
        public string comments;

        public List<SndClip> sounds;
        public Dictionary<int, Dictionary<int, SndClip>> soundList;

        protected AudioSource  aSource;

        public void Start()
        {
            aSource = GetComponent<AudioSource>();

            if (!string.IsNullOrEmpty(fileName))
            {
                ReadFromFile(fileName);
            }
        }

        [ContextMenu("Load From File")]
        public void LoadInEditor()
        {
            string file = EditorUtility.OpenFilePanel("Select Mugen SND File", "", "snd");

            if (file.Length != 0)
            {
                ReadFromFile(file);
            }
        }

        public override void ReadFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
            // set the pointer in start of file
            fileStream.Seek(0, SeekOrigin.Begin);

            signature = ReadString(fileStream, 12);
            version = String.Format("{3}.{2}.{1}.{0}", fileStream.ReadByte(), fileStream.ReadByte(), fileStream.ReadByte(), fileStream.ReadByte());
            totalSound = ReadInt(fileStream, 4);
            offsetSubFile = ReadInt(fileStream, 4);
            comments = ReadString(fileStream, 488); //comments

            sounds= new List<SndClip>();
            soundList = new Dictionary<int, Dictionary<int, SndClip>>();

            for (int i = 0; i < totalSound; i++)
            {
                SndClip sndClip = new SndClip();

                /**
                00-03 File offset where next subfile in the linked list is	[04] located. Null if last subfile.
                04-07 Subfile length (not including header.)	[04]
                08-11 Group number	[04]
                12-15 Sample number	[04]
                08- Sound data (WAV)*/
                sndClip.nextFileOffset = ReadInt(fileStream, 4);
                sndClip.subfileLength = ReadInt(fileStream, 4);

                sndClip.groupNumber = ReadInt(fileStream, 4);
                sndClip.sampleNumber = ReadInt(fileStream, 4);

                sndClip.name = sndClip.groupNumber + "-" + sndClip.sampleNumber;

                byte[] sndBytes = ReadBytes(fileStream, sndClip.subfileLength);

                sndClip.clip = WavUtility.ToAudioClip(sndBytes, 0, sndClip.name);

                //aSource.PlayOneShot(sndClip.clip);

                BinaryWriter w = new BinaryWriter(File.OpenWrite(String.Format("sound.g-{0}.i-{1}.wav", sndClip.groupNumber, sndClip.sampleNumber)));
                w.Write(sndBytes);
                w.Flush();
                w.Close();

                //create dictionary for group
                if (soundList.ContainsKey(sndClip.groupNumber))
                {
                    Dictionary<int, SndClip> dicSnd = soundList[sndClip.groupNumber];
                    dicSnd.Add(sndClip.sampleNumber, sndClip);
                }
                else
                {
                    sounds.Add(sndClip);
                    Dictionary<int, SndClip> dicSnd = new Dictionary<int, SndClip>();
                    dicSnd.Add(sndClip.sampleNumber, sndClip);
                    soundList.Add(sndClip.groupNumber, dicSnd);
                }
            }
        }
    }
}
