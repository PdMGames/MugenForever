using MugenForever.IO.PAL;
using MugenForever.IO.PCX;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LogadImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\sample_1280_853.pcx"); // RGB
        //using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\0-3-rgb-transp.pcx"); // RGB
        using FileStream st = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\0-3.pcx"); // INDEXED

        //using FileStream pal = File.OpenRead("C:\\erick.leao\\desenv\\unity3d\\MugenForeverOld\\Resources\\winmugen_2003\\chars\\kfm\\kfm2.act"); // PALLETE EXTERNAL

        //IPCXImage readPCXImage = new PCXImageImpl(st, new PaletteImpl(pal)); // com pal external
        IPCXImage readPCXImage = new PCXImageImpl(st);

        Texture2D texture = readPCXImage.Texture2D;
        texture.filterMode = FilterMode.Point;

        //RawImage image = gameObject.GetComponent<RawImage>();
        Image image = gameObject.GetComponent<Image>();
        image.GetComponent<RectTransform>().sizeDelta = new Vector2(texture.width, texture.height);
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 50f);

        //flip
        //image.transform.localScale = new Vector3(-1, 1, 1);



        Debug.Log(readPCXImage);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
