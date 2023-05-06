using System.IO;
using UnityEngine;
using BinaryReader = MugenForever.Util.BinaryReader;

namespace MugenForever.IO.PAL
{
    public class PaletteImpl : IPalette

    {
        private Color32[] _paletteColor;
        private readonly int _palletaSize = 768;

        public PaletteImpl()
        {

        }

        public PaletteImpl(Stream pallete)
        {
            Load(pallete);
        }

        public void Load(Stream pallete)
        {
            Color32[] palleteColor = new Color32[256];
            pallete.Seek((_palletaSize * -1), SeekOrigin.End);

            for (int i = 0; i < 256; i++)
            {
                palleteColor[i] = new Color32(BinaryReader.ReadByte(pallete), BinaryReader.ReadByte(pallete), BinaryReader.ReadByte(pallete), 255);
            }
            
            _paletteColor = palleteColor;
        }

        public Color32[] PalleteColor { get { return _paletteColor; } }
    }
}