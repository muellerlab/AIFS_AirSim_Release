using System.IO;
using UnityEngine;

public class AutoImageScript_Left : MonoBehaviour
{

    private float nextPicTime = 0.0f;
    public float period = 1.0f;
    private int FileCounter = 0;

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
        Camera Cam = GetComponent<Camera>();

        RenderTexture currentRT = RenderTexture.GetTemporary(imgWidth, imgHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        Cam.targetTexture = currentRT;
        RenderTexture.active = Cam.targetTexture;

 
        Cam.Render();
 
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToPNG();
        Destroy(Image);
 
        File.WriteAllBytes(Application.dataPath + "/ImageSequence/" + "Left_" + FileCounter + "_P_" + transform.position.x.ToString("0.00") + 
        "_" + transform.position.y.ToString("0.00")+"_" + transform.position.z.ToString("0.00") + "_R_" + transform.rotation.eulerAngles.x.ToString("0.00") + "_" +
        transform.rotation.eulerAngles.y.ToString("0.00") + "_" + transform.rotation.eulerAngles.z.ToString("0.00") + ".png", Bytes);
        FileCounter++;
    }
}
