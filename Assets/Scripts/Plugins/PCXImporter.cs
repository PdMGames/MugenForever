using MugenForever.IO.PCX;
using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1,"pcx")]
public class PcxImporter : ScriptedImporter
{

    public bool alphaIsTransparency = false;

    public override void OnImportAsset(AssetImportContext ctx)
    {
        FileStream fileStream = new(ctx.assetPath, FileMode.Open, FileAccess.Read);

        IPCXImage readPCXImage = new PCXImageImpl(fileStream);
        Texture2D texture = readPCXImage.Texture2D;
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.name = Path.GetFileName(ctx.assetPath);
        texture.alphaIsTransparency = alphaIsTransparency;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), 100f);
        sprite.name = texture.name;

        ctx.AddObjectToAsset("sprite", sprite);
        ctx.AddObjectToAsset("main", texture);
        ctx.SetMainObject(texture);
    }

}