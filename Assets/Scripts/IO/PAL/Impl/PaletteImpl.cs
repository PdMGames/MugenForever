using System;
using System.IO;
using UnityEngine;
using BinaryReader = MugenForever.Util.BinaryReader;

namespace MugenForever.IO.PAL
{
    [CreateAssetMenu(menuName = "MugenForever/ColorTable")]
    public class PaletteImpl : ScriptableObject, IPalette
    {
        [SerializeField]
        private Color32[] _paletteColor;

        public PaletteImpl()
        {

        }

        public PaletteImpl(Stream pallete)
        {
            Load(pallete);
        }

        public void Load(Stream pallete)
        {
            Load(pallete, IPalette.SIZE);
        }

        public void Load(Stream pallete, int size)
        {
            int count = size / 3;
            Color32[] palleteColor = new Color32[count];
            pallete.Seek((size * -1), SeekOrigin.End);

            for (int i = count-1; i >= 0; i--)
            {
                palleteColor[i] = new Color32(BinaryReader.ReadByte(pallete), BinaryReader.ReadByte(pallete), BinaryReader.ReadByte(pallete), 255);
            }

            _paletteColor = palleteColor;
        }

        public Color32[] PalleteColor { get { return _paletteColor; } }
    }
}