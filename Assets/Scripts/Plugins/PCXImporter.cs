using MugenForever.IO.PAL;
using MugenForever.IO.PCX;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace MugenForever.Plugins
{

    [ScriptedImporter(version: 2, ext: "pcx", AllowCaching = true)]
    [RequireComponent(typeof(RawImage))]
    public class PcxImporter : ScriptedImporter
    {
        [Header("Texture")]
        [Tooltip("Alpha is transparency")]
        public bool alphaIsTransparency = false;
        [Tooltip("Filter mode")]
        public FilterMode filterMode = FilterMode.Point;
        [Tooltip("Wrap mode")]
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        [Tooltip("AnisoLevel")]
        [Range(1, 16)]
        public int anisoLevel = 1;

        [Header("Sprite")]
        public bool createSprite = true;
        public Vector2 pivot;
        public float pixelSize = 100f;
        public PaletteImpl pallete;
        


        public override void OnImportAsset(AssetImportContext ctx)
        {
            FileStream fileStream = new(ctx.assetPath, FileMode.Open, FileAccess.Read);
            IPCXImage readPCXImage;

            if (pallete)
            {
                readPCXImage = new PCXImageImpl(fileStream, pallete);
            }
            else
            {
                readPCXImage = new PCXImageImpl(fileStream);
            }

            Texture2D texture = readPCXImage.Texture2D;
            texture.name = Path.GetFileName(ctx.assetPath);
            texture.filterMode = filterMode;
            texture.wrapMode = wrapMode;
            texture.alphaIsTransparency = alphaIsTransparency;
            texture.anisoLevel = anisoLevel;

            ctx.AddObjectToAsset("texture", texture);
            ctx.SetMainObject(texture);

           

            if (createSprite)
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelSize);
                sprite.name = texture.name;
                
                //image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), pivot, pixelSize);

                //ctx.AddObjectToAsset("sprite", sprite);
            }
        }
    }

    [CustomEditor(typeof(PcxImporter), true)]
    public class PCXImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            base.ApplyRevertGUI();
        }
    }
}