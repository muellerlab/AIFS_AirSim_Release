using System.IO;
using UnityEngine;

public class AutoImageScript_Front : MonoBehaviour
{

    private float nextPicTime = 10.0f;
    public float period = 1.0f;
    public int FileCounter = 0;

    public int imgWidth = 1202;
    public int imgHeight = 908;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextPicTime) {
            CamCapture(); 
            nextPicTime = Time.time + period;
        }
    }
 
    void CamCapture()
    {
        // Gets the camera object
        Camera Cam = GetComponent<Camera>();

        // Specify a render texture
        RenderTexture currentRT = RenderTexture.GetTemporary(imgWidth, imgHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        // What the camera sees will be rendered using this texture
        Cam.targetTexture = currentRT;
        // The texture is also set to be the current rendering target
        RenderTexture.active = Cam.targetTexture;

        // The camera renders whatever is set to its target texture
        Cam.Render();

        // This creates another texture
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        // Reads from screen to the new texture, makes a copy of the rendering
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        // Applies changes to the texture
        Image.Apply();
        RenderTexture.active = currentRT; // I think this is actually already done
 
        var Bytes = Image.EncodeToPNG(); //Now the image is stored in byte array
        Destroy(Image);
 
        File.WriteAllBytes(Application.dataPath + "/ImageSequence/" + "Scene_" + FileCounter + "_P_" + transform.position.x.ToString("0.00") + 
        "_" + transform.position.y.ToString("0.00")+"_" + transform.position.z.ToString("0.00") + "_R_" + transform.rotation.eulerAngles.x.ToString("0.00") + "_" +
        transform.rotation.eulerAngles.y.ToString("0.00") + "_" + transform.rotation.eulerAngles.z.ToString("0.00") + ".png", Bytes);
        FileCounter++;
    }
}
