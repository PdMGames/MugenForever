using MugenForever.IO.PCX;
using System.IO;
using UnityEngine;

public class LogadImage : MonoBehaviour
{
    public string PathPxc;

    // Start is called before the first frame update
    void Start()
    {

        //using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\sample_1280_853.pcx"); // RGB
        //using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\0-3-rgb.pcx"); // RGB
        //using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\0-3.pcx"); // INDEXED
        using FileStream st = File.OpenRead(PathPxc);
        //using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\commons-imaging\\src\\test\\data\\images\\pcx\\1\\3plane8bppCompressed.pcx");


        //using FileStream pal = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\kfm4.act"); // PALLETE EXTERNAL

        //IPCXImage readPCXImage = new PCXImageImpl(st, true, new PaletteImpl(pal)); // com pal external
        IPCXImage readPCXImage = new PCXImageImpl(st);

        Texture2D texture = readPCXImage.Texture2D;
        texture.filterMode = FilterMode.Point;
        //RawImage image = gameObject.GetComponent<RawImage>();
        /*Image image = gameObject.GetComponent<Image>();
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
        image.GetComponentInParent<Canvas>().pixelPerfect = true;
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.0f, 0.0f), 100f);*/
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.0f, 0.0f), 100f); ;



        //flip
        //image.transform.localScale = new Vector3(-1, 1, 1);



        Debug.Log(readPCXImage);


    }

    // Update is called once per frame
    void Update()
    {

    }

}
