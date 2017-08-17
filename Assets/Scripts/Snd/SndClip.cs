using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
00-03 File offset where next subfile in the linked list is	[04] located. Null if last subfile.
04-07 Subfile length (not including header.)	[04]
08-11 Group number	[04]
12-15 Sample number	[04]
08- Sound data (WAV)
*/
[System.Serializable]
public class SndClip
{
    public string name;
    public int nextFileOffset;
    public int subfileLength;
    public int groupNumber;
    public int sampleNumber;
    public AudioClip clip;
}
