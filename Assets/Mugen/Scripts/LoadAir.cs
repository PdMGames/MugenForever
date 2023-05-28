using MugenForever.AIR;
using MugenForever.Util;
using System.IO;
using UnityEngine;

namespace MugenForever.Scripts
{
    public class LoadAir : MonoBehaviour
    {

        void Start()
        {
            FileStream fs = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\kfm.air");

            ReaderConfig config = new(fs);

            new AIRImpl(config);
        }

    }
}
