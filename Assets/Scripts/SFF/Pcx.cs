using UnityEngine;
using System.IO;

// Namespace adjusted to match the one in SffV1.cs and SffV2.cs
namespace MugenForever.Sff
{
    public class Pcx
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public byte[] DecodedPixels { get; private set; }
        public Color32[] Palette { get; private set; }

        public Pcx() { }

        public bool Load(byte[] pcxData)
        {
            if (pcxData == null || pcxData.Length < 128)
            {
                Debug.LogError("PCX_LOAD: Data is null or too short for header.");
                return false;
            }

            using (MemoryStream stream = new MemoryStream(pcxData))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // Cabeçalho PCX (128 bytes)
                    byte manufacturer = reader.ReadByte(); // Deve ser 0x0A
                    byte version = reader.ReadByte();       // Versão do PCX
                    byte encoding = reader.ReadByte();      // Deve ser 0x01 (RLE)
                    byte bitsPerPixel = reader.ReadByte();  // Bits por pixel por plano

                    ushort xMin = reader.ReadUInt16();
                    ushort yMin = reader.ReadUInt16();
                    ushort xMax = reader.ReadUInt16();
                    ushort yMax = reader.ReadUInt16();

                    reader.ReadUInt16(); // Skip hDpi - Resolução horizontal
                    reader.ReadUInt16(); // Skip vDpi - Resolução vertical

                    reader.ReadBytes(48); // Skip egaPalette - Paleta EGA de 16 cores (não usada para 256 cores)
                    reader.ReadByte(); // Skip reserved
                    byte numColorPlanes = reader.ReadByte(); // Número de planos de cor
                    ushort bytesPerLine = reader.ReadUInt16(); // Bytes por linha de varredura por plano

                    reader.ReadUInt16(); // Skip paletteInfo - Informação da paleta (color/bw)
                    reader.ReadUInt16(); // Skip hScreenSize
                    reader.ReadUInt16(); // Skip vScreenSize
                    reader.ReadBytes(54); // Skip Preenchimento reservado

                    if (manufacturer != 0x0A) // Validação movida para após a leitura completa do cabeçalho mínimo
                    {
                        Debug.LogError("PCX_LOAD: Invalid PCX identifier. Expected 0x0A, got " + manufacturer);
                        return false;
                    }
                    if (encoding != 0x01)
                    {
                        Debug.LogError("PCX_LOAD: Unsupported encoding. Expected 1 (RLE), got " + encoding);
                        return false;
                    }
                     if (bitsPerPixel != 8 || numColorPlanes != 1)
                    {
                        Debug.LogWarningFormat("PCX_LOAD: Unsupported format. BitsPerPixel: {0} (expected 8), NumColorPlanes: {1} (expected 1).", bitsPerPixel, numColorPlanes);
                        // return false; // Might be too strict for some edge cases, but good for typical MUGEN PCX.
                    }


                    Width = (xMax - xMin) + 1;
                    Height = (yMax - yMin) + 1;

                    if (Width <= 0 || Height <= 0 || Width > 4096 || Height > 4096) // Sanity check for dimensions
                    {
                        Debug.LogErrorFormat("PCX_LOAD: Invalid dimensions {0}x{1}. Calculated from xMin={2},yMin={3},xMax={4},yMax={5}", Width, Height, xMin,yMin,xMax,yMax);
                        return false;
                    }


                    DecodedPixels = new byte[Width * Height]; // Simplified for 1 plane

                    // Decodificação RLE
                    int pixelIndex = 0;
                    for (int y = 0; y < Height; y++)
                    {
                        int scanlineByteCounter = 0;
                        while(scanlineByteCounter < bytesPerLine)
                        {
                            if (pixelIndex >= DecodedPixels.Length && scanlineByteCounter < bytesPerLine)
                            {
                                Debug.LogErrorFormat("PCX_LOAD: RLE decoding overflow (pixelIndex: {0} >= DecodedPixels.Length: {1}) while scanlineByteCounter: {2} < bytesPerLine: {3} at line {4}.",
                                    pixelIndex, DecodedPixels.Length, scanlineByteCounter, bytesPerLine, y);
                                return false;
                            }
                            if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                                Debug.LogError("PCX_LOAD: Unexpected EOF during RLE scanline decoding at line " +y);
                                return false;
                            }

                            byte runControlByte = reader.ReadByte();
                            int runLength;
                            byte pixelValue;

                            if ((runControlByte & 0xC0) == 0xC0) // Check if top 2 bits are set
                            {
                                runLength = runControlByte & 0x3F; // Mask out top 2 bits for run length
                                if (reader.BaseStream.Position >= reader.BaseStream.Length) {
                                     Debug.LogError("PCX_LOAD: Unexpected EOF reading RLE pixel value at line " +y);
                                     return false;
                                }
                                pixelValue = reader.ReadByte();
                            }
                            else
                            {
                                runLength = 1;
                                pixelValue = runControlByte;
                            }

                            for (int i = 0; i < runLength; i++)
                            {
                                // Only write if within the image's actual width for this scanline
                                if (scanlineByteCounter < Width)
                                {
                                    if (pixelIndex < DecodedPixels.Length)
                                    {
                                        DecodedPixels[pixelIndex++] = pixelValue;
                                    }
                                    // else: Error already logged by outer check, or this is padding beyond Width*Height
                                }
                                scanlineByteCounter++;
                            }
                        }
                    }

                    // Leitura da Paleta VGA (256 cores)
                    if (bitsPerPixel == 8 && numColorPlanes == 1)
                    {
                        long expectedPaletteOffset = pcxData.Length - 769;
                        if (expectedPaletteOffset > 0 && stream.Length > expectedPaletteOffset) // Check if file is large enough
                        {
                            stream.Seek(expectedPaletteOffset, SeekOrigin.Begin);
                            if (reader.ReadByte() == 0x0C) // Palette marker
                            {
                                Palette = new Color32[256];
                                for (int i = 0; i < 256; i++)
                                {
                                    if (stream.Position > stream.Length - 3) {
                                        Debug.LogWarning("PCX_LOAD: EOF while reading VGA palette. Index: " + i);
                                        for(int j=i; j<256; ++j) Palette[j] = new Color32(0,0,0,255); // Fill rest with black
                                        break;
                                    }
                                    byte r = reader.ReadByte();
                                    byte g = reader.ReadByte();
                                    byte b = reader.ReadByte();
                                    Palette[i] = new Color32(r, g, b, 255);
                                }
                            }
                            else
                            {
                                Debug.LogWarning("PCX_LOAD: VGA palette marker (0x0C) not found at expected position (EOF-769). External palette may be required.");
                            }
                        }
                        else
                        {
                             Debug.LogWarning("PCX_LOAD: File too short for standard VGA palette or invalid offset. External palette may be required.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"PCX_LOAD: Image is not 8bpp/1-plane (BitsPerPixel: {bitsPerPixel}, NumColorPlanes: {numColorPlanes}). Skipping VGA palette search.");
                    }
                }
            }
            return true;
        }
         // Utility method to create a Texture2D from loaded PCX data.
        // This will be called by SffSprite or a similar class.
        public Texture2D CreateTexture()
        {
            if (DecodedPixels == null || Width == 0 || Height == 0)
            {
                Debug.LogError("[Pcx.CreateTexture] Error: Decoded pixel data is missing or dimensions are zero.");
                return null;
            }
            if (Palette == null)
            {
                Debug.LogWarning("[Pcx.CreateTexture] Warning: Palette is missing. Cannot create colored texture. An external .ACT palette is likely required.");
                return null; // For MUGEN, this means an ACT file is needed.
            }
            if (DecodedPixels.Length != Width * Height)
            {
                Debug.LogErrorFormat("[Pcx.CreateTexture] Error: DecodedPixels length ({0}) does not match Width*Height ({1}*{2}={3}).",
                                     DecodedPixels.Length, Width, Height, Width * Height);
                return null;
            }

            Texture2D texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);
            Color32[] colorData = new Color32[Width * Height];

            for (int i = 0; i < DecodedPixels.Length; i++)
            {
                byte paletteIndex = DecodedPixels[i];
                if (paletteIndex < Palette.Length) // Palette should have 256 entries
                {
                    colorData[i] = Palette[paletteIndex];
                }
                else
                {
                    // This case indicates an issue with the PCX data or palette.
                    Debug.LogWarningFormat("[Pcx.CreateTexture] Warning: Palette index {0} is out of bounds for palette length {1} at pixel {2}. Using magenta.",
                                           paletteIndex, Palette.Length, i);
                    colorData[i] = new Color32(255, 0, 255, 255); // Magenta for error
                }
            }
            texture.SetPixels32(colorData);
            texture.Apply();
            return texture;
        }
    }
}
