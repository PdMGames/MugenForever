using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MugenForever.SFF
{
    public class SFFInfo
    {
        
        /// <summary>
        /// Nome do distribuidor
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Versão do SFF
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// Numero de imagens
        /// </summary>
        public int totalImage { get; set; }

        /// <summary>
        /// Posição inicial do subfile
        /// </summary>
        public int offsetSubFile { get; set; }

        /// <summary>
        /// Tamanho do SubfileHeader
        /// </summary>
        public int sizeSubFileHeader { get; set; }

        /// <summary>
        /// Posição inicial do inicio das palettas
        /// </summary>
        public int offsetPallete { get; set; }

        /// <summary>
        /// Paletta 
        /// 1 - SPRPALTYPE_SHARED
        /// 2 - SPRPALTYPE_INDIV
        /// </summary>
        public byte paletteType { get; set; }

        /// <summary>
        /// Comentarios
        /// </summary>
        public string comments { get; set; }

        public Dictionary<int, Dictionary<int, SFFImageInfo>> groupsImage { get; set; }

        public class SFFImageInfo
        {

            /// <summary>
            /// Tamanho do Subfile
            /// 0 - Informa que é um link
            /// </summary>
            public int lengthSubfile { get; set; }

            /// <summary>
            /// Cordenada axis X
            /// </summary>
            public int positionX { get; set; }

            /// <summary>
            /// Cordenada axis Y
            /// </summary>
            public int positionY { get; set; }

            /// <summary>
            /// Cordenada axis Z
            /// </summary>
            public int positionZ { get; set; }

            /// <summary>
            /// Numero da imagem anterior caso a lengthSubfile seja 0 
            /// </summary>
            public int indexLinkedImage { get; set; }

            /// <summary>
            /// Verifica se irá utilizar a mesma paleta da imagem anterior
            /// </summary>
            public bool isPalettePreviusImage { get; set; }

            /// <summary>
            /// Comentários
            /// </summary>
            public string comments { get; set; }

            /// <summary>
            /// Conteúdo da imagem
            /// </summary>
            public MemoryStream data { get; set; }

        }

        [ContextMenu("Load From File")]
        public void LoadInEditor()
        {
            string file = EditorUtility.OpenFilePanel("Select Mugen Def File", "", "def");

            if (file.Length != 0)
            {
                //LoadFromFile(file);
            }
        }
    }
}