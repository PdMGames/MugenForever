using UnityEngine;

namespace MugenForever.Sff
{
    /**
00-03 File offset where next subfile in the "linked list" is	[04]
located. Null if last subfile
04-07 Subfile length (not including header)	[04]
Length is 0 if it is a linked sprite
08-09 Image axis X coordinate	[02]
10-11 Image axis Y coordinate	[02]
12-13 Group number	[02]
14-15 Image number (in the group)	[02]
16-17 Index of previous copy of sprite (linked sprites only)	[02]
This is the actual
18 True if palette is same as previous image	[01]
19-31 Blank; can be used for comments	[14]
32- PCX graphic data. If palette data is available, it is the last
768 bytes
    */
    [System.Serializable]
    public class SffSprite
    {
        /// <summary>
        /// Name of Sprite
        /// </summary>
        public string name;

        /// <summary>
        /// Index position
        /// </summary>
        public long index;

        /// <summary>
        /// Next image offset
        /// </summary>
        public int nextFileOffset;

        /// <summary>
        /// size of image: 0 is linked
        /// </summary>
        public int subfileLength;

        /// <summary>
        /// Position X
        /// </summary>
        public int axisX;

        /// <summary>
        /// Position Y
        /// </summary>
        public int axisY;

        /// <summary>
        /// Number of group
        /// </summary>
        public int groupNumber;

        /// <summary>
        /// Number of image
        /// </summary>
        public int imageNumber;

        /// <summary>
        /// Position of image linked sprite only
        /// </summary>
        public int indexPreviousLinked;

        /// <summary>
        /// Same palette of image previous
        /// </summary>
        public bool samePaletteOfPreviousImage;

        /// <summary>
        /// Comments
        /// </summary>
        public string comments;
        
        /// <summary>
        /// Image raw
        /// </summary>
        public Texture2D texture { get; set; } // Renomeado de image para texture e adicionado setter

        /// <summary>
        /// Sprite source
        /// </summary>
        public Sprite sprite { get; set; } // Adicionado setter

        public Pcx pcx = new Pcx();

        // version sffv2 <<

        /// <summary>
        /// Width of image
        /// </summary>
        public int width;

        /// <summary>
        /// Height of image
        /// </summary>
        public int height;

        /// <summary>
        /// ????
        /// </summary>
        public int coldepth;

        /// <summary>
        /// Format
        /// 0 raw
        /// 1 invalid (no use)
        /// 2 RLE8
        /// 3 RLE5
        /// 4 LZ5
        /// </summary>
        public int fmt;

        /// <summary>
        ///  offset ldata or tdata
        /// </summary>
        public int offsetData;

        /// <summary>
        /// Index of palette
        /// </summary>
        public int paletteIndex;

        /// <summary>
        /// 0    unset: literal (use ldata); set: translate (use tdata; decompress on load)
        /// 1-15 unused
        /// </summary>
        public int flag;

        // >> version sffv2

        public void GenerateUnitySprite()
        {
            if (pcx == null || pcx.DecodedPixels == null)
            {
                Debug.LogError("SFFSPRITE_GENERATE: PCX data or DecodedPixels are null. Cannot create texture.");
                return;
            }

            if (pcx.Width <= 0 || pcx.Height <= 0)
            {
                Debug.LogError($"SFFSPRITE_GENERATE: Invalid PCX dimensions: {pcx.Width}x{pcx.Height}");
                return;
            }

            // Cria a Texture2D
            // As texturas em MUGEN geralmente têm o eixo Y invertido em comparação com o Unity.
            // Os pixels são aplicados de baixo para cima na textura do Unity.
            texture = new Texture2D(pcx.Width, pcx.Height, TextureFormat.RGBA32, false);
            Color32[] pixels = new Color32[pcx.Width * pcx.Height];

            if (pcx.Palette != null && pcx.Palette.Length > 0)
            {
                for (int y = 0; y < pcx.Height; y++)
                {
                    for (int x = 0; x < pcx.Width; x++)
                    {
                        int decodedIndex = y * pcx.Width + x;
                        if (decodedIndex >= pcx.DecodedPixels.Length) continue; // Safety check

                        byte paletteIndex = pcx.DecodedPixels[decodedIndex];
                        if (paletteIndex >= pcx.Palette.Length)
                        {
                            // Fallback para magenta se o índice da paleta estiver fora dos limites
                            pixels[((pcx.Height - 1 - y) * pcx.Width) + x] = new Color32(255, 0, 255, 255);
                        }
                        else
                        {
                            pixels[((pcx.Height - 1 - y) * pcx.Width) + x] = pcx.Palette[paletteIndex];
                        }
                    }
                }
            }
            else // Sem paleta (ou paleta inválida), criar textura em tons de cinza ou com cor de erro
            {
                Debug.LogWarning("SFFSPRITE_GENERATE: PCX Palette is null or empty. Creating grayscale or error texture.");
                for (int y = 0; y < pcx.Height; y++)
                {
                    for (int x = 0; x < pcx.Width; x++)
                    {
                        int decodedIndex = y * pcx.Width + x;
                        if (decodedIndex >= pcx.DecodedPixels.Length) continue;

                        byte grayValue = pcx.DecodedPixels[decodedIndex];
                        // Invertendo y para o sistema de coordenadas do Unity
                        pixels[((pcx.Height - 1 - y) * pcx.Width) + x] = new Color32(grayValue, grayValue, grayValue, 255);
                    }
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply();

            // Cria o Sprite
            // O pivô (pivot) em MUGEN é geralmente o eixo (axisX, axisY) do SFFSprite.
            // O Unity espera o pivô como uma fração da largura/altura (0-1).
            // axisX e axisY são as coordenadas do eixo a partir do canto superior esquerdo da imagem.
            // O pivô do Unity é a partir do canto inferior esquerdo.
            // Então, pivot.x = axisX / width  e  pivot.y = 1 - (axisY / height)
            float pivotX = (pcx.Width > 0) ? ((float)this.axisX / pcx.Width) : 0.5f;
            float pivotY = (pcx.Height > 0) ? (1.0f - ((float)this.axisY / pcx.Height)) : 0.5f;

            sprite = Sprite.Create(texture, new Rect(0, 0, pcx.Width, pcx.Height), new Vector2(pivotX, pivotY), 100.0f); // 100 pixels per unit é um valor comum

            // Debug.Log($"SFFSPRITE_GENERATE: Sprite '{groupNumber}-{imageNumber}' created. Size: {pcx.Width}x{pcx.Height}, Axis: ({axisX},{axisY}), Pivot: ({pivotX},{pivotY})");
        }
    }
}
