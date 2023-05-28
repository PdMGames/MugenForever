using Assets.Scripts.IO.SFF;
using MugenForever.AIR;
using MugenForever.IO.PAL;
using MugenForever.IO.SFF;
using MugenForever.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MugenForever
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    internal class ExecuteAnimationTest : MonoBehaviour
    {

        public string ActionStateNumber;

        public string SffLocal;
        public string PaletteLocal;

        public string AirLocal;

        private Animator animator;
        private SpriteRenderer spriteRenderer;

        private void Start()
        {

            FileStream sffStream = File.OpenRead(SffLocal);
            FileStream palStream = File.OpenRead(PaletteLocal);
            FileStream airStream = File.OpenRead(AirLocal);

            animator = gameObject.GetComponent<Animator>();
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

            ISFF sff = new SFFImpl(sffStream, new PaletteImpl(palStream));

            Dictionary<int, SFFSprite> spriteList = sff.Spriters[0];
            SFFSprite sprite = spriteList[0];
            Texture2D texture2D = sprite.PCX.Texture2D;
            spriteRenderer.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.0f, 0.0f), 100f);
            spriteRenderer.transform.localPosition = new Vector2(2, -2.09f);

            AIRImpl air = new(new ReaderConfig(airStream));

            AnimationClip clip = new();
            clip.frameRate = 60;


        }

        private void Update()
        {
            
        }

    }
}
