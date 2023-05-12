using MugenForever.IO.PAL;
using MugenForever.IO.PCX;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace MugenForever.Plugins
{
    [ScriptedImporter(version: 2, ext: "act", AllowCaching = true)]
    public class ActImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            FileStream fileStream = new(ctx.assetPath, FileMode.Open, FileAccess.Read);

            PaletteImpl pallete = new PaletteImpl(fileStream);
            pallete.name = Path.GetFileName(ctx.assetPath);

            ctx.AddObjectToAsset("pallete", pallete);
            ctx.SetMainObject(pallete);
        }
    }

    [CustomEditor(typeof(ActImporter), true)]
    public class ActImporterEditor : ScriptedImporterEditor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            base.ApplyRevertGUI();
        }
    }
}
