using Assets.Scripts.IO.SFF;
using MugenForever.IO.PAL;
using MugenForever.IO.SFF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

        private IEnumerator AnimationTest()
        {

            Dictionary<int, SFFSprite> sprite;
            SFFSprite sffSprite;
            var transform = image.GetComponent<RectTransform>();

            while (true){
                isKeyPress = Input.anyKey;
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    sprite = sffImpl.Spriters[20];
                    for (int i = 0; i < sprite.Count; i++)
                    {
                        if (!isKeyPress) break;
                        sffSprite = sprite[i];
                        Texture2D texture = sffSprite.PCX.Texture2D;
                        transform.sizeDelta = new Vector2(texture.width, texture.height);
                        transform.localPosition = new Vector2(sffSprite.AxisX, sffSprite.AxisY);
                        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.0f, 0.0f), 100f);
                        yield return new WaitForSecondsRealtime(speed);
                        isKeyPress = Input.anyKey;
                    }
                }

                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    sprite = sffImpl.Spriters[21];
                    for (int i = 0; i < sprite.Count; i++)
                    {
                        if (!isKeyPress) break;
                        sffSprite = sprite[i];
                        Texture2D texture = sffSprite.PCX.Texture2D;
                        transform.sizeDelta = new Vector2(texture.width, texture.height);
                        transform.localPosition = new Vector2(sffSprite.AxisX, sffSprite.AxisY);
                        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.0f, 0.0f), 100f);
                        yield return new WaitForSecondsRealtime(speed);
                        isKeyPress = Input.anyKey;
                    }
                }

                if (!isKeyPress)
                {
                    sprite = sffImpl.Spriters[0];
                    for (int i = 0; i < sprite.Count; i++)
                    {
                        if (isKeyPress) break;
                        sffSprite = sprite[i];
                        Texture2D texture = sffSprite.PCX.Texture2D;                        
                        transform.sizeDelta = new Vector2(texture.width, texture.height);
                        transform.localPosition = new Vector2(sffSprite.AxisX, sffSprite.AxisY);
                        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.0f, 0.0f), 100f);
                        yield return new WaitForSecondsRealtime(speed);
                        isKeyPress = Input.anyKey;
                    }
                }
            }
        }

        private void Update()
        {
           
        }
    }
}
