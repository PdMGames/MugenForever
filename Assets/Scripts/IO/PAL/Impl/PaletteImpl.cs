using System.Drawing;
using System.IO;
using UnityEngine;
using BinaryReader = MugenForever.Util.BinaryReader;

namespace MugenForever.IO.PAL
{
    public class PaletteImpl : IPalette

    {
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

            for (int i = 0; i < count; i++)
            {
                palleteColor[i] = new Color32(BinaryReader.ReadByte(pallete), BinaryReader.ReadByte(pallete), BinaryReader.ReadByte(pallete), 255);
            }

            _paletteColor = palleteColor;
        }

        public Color32[] PalleteColor { get { return _paletteColor; } }
    }
}