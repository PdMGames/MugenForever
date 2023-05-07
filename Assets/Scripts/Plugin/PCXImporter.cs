using UnityEngine;
using UnityEditor.AssetImporters;
using System.IO;
using MugenForever.IO.PCX;
using UnityEditor;

[ScriptedImporter(1,"pcx")]
public class PcxImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        FileStream fileStream = new FileStream(ctx.assetPath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);

        IPCXImage readPCXImage = new PCXImageImpl(fileStream);
        Texture2D texture = readPCXImage.Texture2D;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100f);

        ctx.AddObjectToAsset("sprite", sprite);
        ctx.AddObjectToAsset("main", texture);
        ctx.SetMainObject(texture);
    }
}