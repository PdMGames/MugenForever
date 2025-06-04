using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Read SFF V1
/// The file specification
/// <include file='xml_include_tag.doc' path='Docs/SFFV1_SPEC.md' />
/// </summary>
namespace MugenForever.Sff
{
    // SffV1 não herda mais de Sff (que agora é estático) nem de MonoBehaviour.
    public class SffV1
    {
        // Campos que eram da classe Sff base ou que são específicos do SFFv1
        // Estes serão lidos e depois transferidos para SffInfo.
        private string signature;
        private byte verLo3, verLo2, verLo1, verHi; // Componentes da versão
        private int totalGroups;
        private int totalImage;
        private int offsetSubFile;
        // private int sizeSubFileHeader; // Não parece ser usado diretamente em SffInfo
        // private int paletteType; // Não parece ser usado diretamente em SffInfo
        private string comments;

        // A lista de sprites será construída e adicionada diretamente ao sffInfo.sprites
        // private List<SffSprite> localSprites = new List<SffSprite>(); // Lista local temporária
        // private Dictionary<int, Dictionary<int, SffSprite>> localSpriteList = new Dictionary<int, Dictionary<int, SffSprite>>();


        // Removido LoadInEditor pois EditorUtility não funciona em build
        // e a classe não é mais um MonoBehaviour.

        public void ReadFromFile(string pathFile, SffInfo sffInfo)
        {
            if (sffInfo == null) {
                Debug.LogError("[SffV1.ReadFromFile] Error: sffInfo object is null.");
                return;
            }

            using (FileStream fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read))
            {
                using (System.IO.BinaryReader binaryReader = new System.IO.BinaryReader(fileStream))
                {
                    signature = new string(binaryReader.ReadChars(12));
                    verLo3 = binaryReader.ReadByte();
                    verLo2 = binaryReader.ReadByte();
                    verLo1 = binaryReader.ReadByte();
                    verHi = binaryReader.ReadByte();

                    sffInfo.verLo3 = verLo3.ToString();
                    sffInfo.verLo2 = verLo2.ToString();
                    sffInfo.verLo1 = verLo1.ToString();
                    sffInfo.verHi = verHi.ToString();

                    totalGroups = binaryReader.ReadInt32();
                    totalImage = binaryReader.ReadInt32();
                    offsetSubFile = binaryReader.ReadInt32();
                    /* sizeSubFileHeader = */ binaryReader.ReadInt32();
                    /* paletteType = */ binaryReader.ReadByte();
                    binaryReader.BaseStream.Seek(3, SeekOrigin.Current); // Skip blank space

                    int currentPosForComments = (int)binaryReader.BaseStream.Position;
                    int lengthComments = 512 - currentPosForComments;
                    if (lengthComments < 0) lengthComments = 0; // Sanity check
                    comments = new string(binaryReader.ReadChars(lengthComments));
                    // sffInfo.comments = comments; // SffInfo não tem campo comments atualmente

                    // Ir para o início dos dados dos sprites
                    binaryReader.BaseStream.Seek(offsetSubFile, SeekOrigin.Begin);

                    // sffInfo.sprites já foi inicializado em SffInfo constructor
                    // Dictionary local para ajudar a resolver sprites vinculados
                    List<SffSprite> tempSpriteListForLinking = new List<SffSprite>();


                    for (int i = 0; i < totalImage; i++)
                    {
                        SffSprite spr = new SffSprite();
                        // Posição atual é o início do cabeçalho do subarquivo do sprite
                        long currentSpriteNodeStartOffset = binaryReader.BaseStream.Position;

                        spr.nextFileOffset = binaryReader.ReadInt32();
                spr.subfileLength = binaryReader.ReadInt32();

                spr.axisX = binaryReader.ReadInt16();
                spr.axisY = binaryReader.ReadInt16();
                spr.groupNumber = binaryReader.ReadInt16();
                spr.imageNumber = binaryReader.ReadInt16();
                spr.indexPreviousLinked = binaryReader.ReadInt16();
                spr.index = i + 1;

                spr.samePaletteOfPreviousImage = binaryReader.ReadByte() == 1;
                spr.comments = new string(binaryReader.ReadChars(13));

                if (spr.subfileLength == 0 && spr.indexPreviousLinked != 0 && sprites.Exists(sff => sff.index == spr.indexPreviousLinked))
                {
                    //spr.subfileLength = sprites[spr.indexPreviousLinked].subfileLength;
                    spr.pcx = sprites[spr.indexPreviousLinked].pcx;
                }
                else
                {
                    byte[] imageBytes = binaryReader.ReadBytes(spr.subfileLength);
                    spr.pcx.load(new MemoryStream(imageBytes));

                    //spr.image.LoadRawTextureData(spr.pcx.image.GetRawTextureData());

                    /*if (spr.pcx.image != null)
                    {
                        BinaryWriter w = new BinaryWriter(File.OpenWrite(String.Format("image.g-{0}.i-{1}.png", spr.groupNumber, spr.imageNumber)));

                        // Writer raw data                
                        w.Write(spr.pcx.image.GetRawTextureData());
                        w.Flush();
                        w.Close();
                    }*/
                }
                spr.image = spr.pcx.image;

                spr.name = String.Format("[{0}][{1}][{2}]", spr.index.ToString("000"), spr.groupNumber, spr.imageNumber);

                spr.sprite = new Sprite();
                

                
                 

                //create dictionary for group
                if (spriteList.ContainsKey(spr.groupNumber))
                {
                    Dictionary<int, SffSprite> dicSPR = spriteList[spr.groupNumber];
                    sprites.Add(spr);
                    dicSPR.Add(spr.imageNumber, spr);
                }
                else
                {
                    sprites.Add(spr);
                    Dictionary<int, SffSprite> dicSpr = new Dictionary<int, SffSprite>();
                    dicSpr.Add(spr.imageNumber, spr);
                    spriteList.Add(spr.groupNumber, dicSpr);
                }

                // Jump for next image
                binaryReader.BaseStream.Seek(spr.nextFileOffset, SeekOrigin.Begin);
            }
        }
    }
}
