using MugenForever.IO.PAL;
using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace MugenForever.Plugins
{
    [ScriptedImporter(version: 1, ext: "act", AllowCaching = true)]
    public class ActImporter : ScriptedImporter
    {
        public bool editable = false;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            FileStream fileStream = new(ctx.assetPath, FileMode.Open, FileAccess.Read);

            //PaletteImpl pallete = new PaletteImpl(fileStream);
            PaletteImpl palette = ScriptableObject.CreateInstance<PaletteImpl>();
            palette.Load(fileStream);
            //palette.name = Path.GetFileNameWithoutExtension(ctx.assetPath);
            palette.name = Path.GetFileName(ctx.assetPath);
            ctx.AddObjectToAsset("palette", palette);
            ctx.SetMainObject(palette);

            if (editable)
            {
                PaletteImpl paletteEditable = ScriptableObject.CreateInstance<PaletteImpl>();
                paletteEditable.Load(fileStream);
                paletteEditable.name = Path.GetFileNameWithoutExtension(ctx.assetPath);

                AssetDatabase.CreateAsset(palette, ctx.assetPath + ".asset");
                AssetDatabase.SaveAssets();
            }
        }
    }

    [CustomEditor(typeof(ActImporter), true)]
    public class ActImporterEditor : ScriptedImporterEditor
    {
        ActImporter importer;

        public override void OnEnable()
        {
            base.OnEnable();
            importer = (ActImporter)target;
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            base.ApplyRevertGUI();
        }
    }
}
