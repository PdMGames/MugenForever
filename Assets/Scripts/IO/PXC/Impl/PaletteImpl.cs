using MugenForever.IO.PXC;
using System.IO;
using UnityEngine;

namespace MugenForever.IO.PCX
{
    public class PaletteImpl : IPalette

    {
        private Color32[] _paletteColor;
        private readonly int _palletaSize = 769;

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
            BinaryReader binaryReader = new BinaryReader(pallete);

            var hasPalletaCode = binaryReader.ReadByte();

            if (hasPalletaCode == 12)
            {
                for (int i = 0; i < 256; i++)
                {
                    palleteColor[i] = new Color32(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte(), 255);
                }
            }

            _paletteColor = palleteColor;
        }

        public Color32[] PalleteColor { get { return _paletteColor; } }
    }
}