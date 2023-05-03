using MugenForever.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using BinaryReader = MugenForever.Util.BinaryReader;

/*
| SND file structure
|--------------------------------------------------*\
Version 1.01
HEADER
------
Bytes
00-11 "ElecbyteSnd\0" signature	[12]
12-15 4 verhi, 4 verlo	[04]
16-19 Number of sounds	[04]
20-23 File offset where first subfile is located.	[04]
24-511 Blank; can be used for comments.	[488]

SUBFILEHEADER
-------
Bytes
00-03 File offset where next subfile in the linked list is	[04]
located. Null if last subfile.
04-07 Subfile length (not including header.)	[04]
08-11 Group number	[04]
12-15 Sample number	[04]
08- Sound data (WAV)
*/

namespace MugenForever.IO.SND
{

    /*[RequireComponent(typeof(AudioSource))]*/
    internal class SNDImpl : ISND
    {

        private Dictionary<int, Dictionary<int, AudioClip>> _sounds;
        
        public SNDImpl(Stream data)
        {
            SNDHeader header = ReadHead(data);

            ProcessSubHeader(data, header);

            Debug.Log(header);
        }

        private SNDHeader ReadHead(Stream data)
        {
            SNDHeader header = new() {
                Signature           = BinaryReader.ReadString(data, 12),
                Verhi               = BinaryReader.ReadInt(data, 2),
                Verlo               = BinaryReader.ReadInt(data, 2),
                TotalSound          = BinaryReader.ReadInt(data, 4),
                OffsetFirstSubfile  = BinaryReader.ReadInt(data, 4),
                Comments            = BinaryReader.ReadString(data, 488),
            };

            return header;
        }

        private SNDSubHeader ReadSubRead(Stream data)
        {
            SNDSubHeader subHeader = new()
            {
                OffsetNextSubfile = BinaryReader.ReadInt(data, 4),
                Size = BinaryReader.ReadInt(data, 4),
                Group = BinaryReader.ReadInt(data, 4),
                Index = BinaryReader.ReadInt(data, 4)
            };

            subHeader.Sound = BinaryReader.ReadBytes(data, subHeader.Size);

            return subHeader;
        }

        private void ProcessSubHeader(Stream data, SNDHeader header)
        {
            _sounds = new();
            int nextOffset = header.OffsetFirstSubfile;

            for (int i=0; i<header.TotalSound; i++)
            {
                BinaryReader.ReadJump(data, nextOffset, SeekOrigin.Begin);
                
                SNDSubHeader subHeader = ReadSubRead(data);
                nextOffset = subHeader.OffsetNextSubfile;

                if (_sounds.ContainsKey(subHeader.Group))
                {
                    _sounds[subHeader.Group].Add(subHeader.Index, CreateAudioClip(subHeader.Sound, subHeader.Group, subHeader.Index));
                }
                else
                {
                    var listSounds = new Dictionary<int, AudioClip>
                    {
                        { subHeader.Index, CreateAudioClip(subHeader.Sound, subHeader.Group, subHeader.Index)}
                    };

                    _sounds.Add(subHeader.Group, listSounds);
                }
            }
            
        }

        private AudioClip CreateAudioClip(byte[] data, int group, int index)
        {
            return WavUtility.ToAudioClip(data, 0, string.Format("{0}-{1}", group, index));
        }
        
        public class SNDHeader
        {
            public string Signature;
            public int Verhi;
            public int Verlo;
            public int TotalSound;
            public int OffsetFirstSubfile;
            public string Comments;
        }

        public class SNDSubHeader
        {
            public int OffsetNextSubfile;
            public int Size;
            public int Group;
            public int Index;
            public byte[] Sound;
        }


        /*public string signature;
        public string version;
        public int totalSound;
        public int offsetSubFile;
        public string comments;

        public List<SndClip> sounds;
        public Dictionary<int, Dictionary<int, SndClip>> soundList;

        protected AudioSource aSource;

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

        public void ReadFromFile(string pathFile)
        {
            FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);
            // set the pointer in start of file
            fileStream.Seek(0, SeekOrigin.Begin);

            signature = ReadString(fileStream, 12);
            version = String.Format("{3}.{2}.{1}.{0}", fileStream.ReadByte(), fileStream.ReadByte(), fileStream.ReadByte(), fileStream.ReadByte());
            totalSound = ReadInt(fileStream, 4);
            offsetSubFile = ReadInt(fileStream, 4);
            comments = ReadString(fileStream, 488); //comments

            sounds = new List<SndClip>();
            soundList = new Dictionary<int, Dictionary<int, SndClip>>();

            for (int i = 0; i < totalSound; i++)
            {
                SndClip sndClip = new SndClip();

                //
                00-03 File offset where next subfile in the linked list is	[04] located. Null if last subfile.
                04-07 Subfile length (not including header.)	[04]
                08-11 Group number	[04]
                12-15 Sample number	[04]
                08- Sound data (WAV)*//*
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
        }*/

public Dictionary<int, Dictionary<int, AudioClip>> Sounds { get { return _sounds; } }
    }
}
