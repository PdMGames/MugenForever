using MugenForever.IO.SFF;
using UnityEngine;
using UnityEngine.UI;

namespace MugenForever.Scripts
{
    internal class LoadSff : MonoBehaviour
    {
        [SerializeField]
        public SFFImpl sffImpl;
        public Image image;
        public bool isKeyPress = false;
        public float speed = 0.5f;
        private void Start()
        {
            // SFF1.0
            /* using FileStream sff = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\kfm.sff");
             using FileStream pal = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\kfm.act");

             IPalette palette = new PaletteImpl(pal);
             // transparent color
             palette.PalleteColor[0] = new Color32(0, 0, 0, 0);
             Array.Reverse(palette.PalleteColor);

             sffImpl = new SFFImpl(sff, palette);
             image = GameObject.FindObjectsByType<Image>(FindObjectsSortMode.InstanceID)[0];

             StartCoroutine(AnimationTest());

             Debug.Log(sffImpl);*/
        }


        private void Update()
        {

        }
    }
}
