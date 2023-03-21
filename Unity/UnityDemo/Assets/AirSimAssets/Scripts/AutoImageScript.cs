using System.IO;
using UnityEngine;

public class AutoImageScript : MonoBehaviour
{

    private float nextPicTime = 0.0f;
    public float period = 1.0f;
    public int FileCounter = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextPicTime) {
            nextPicTime += period;
            CamCapture(); 
        }
    }
 
    void CamCapture()
    {
        Camera Cam = GetComponent<Camera>();
 
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;
 
        Cam.Render();
 
        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
 
        var Bytes = Image.EncodeToPNG();
        Destroy(Image);
 
        File.WriteAllBytes(Application.dataPath + "/ImageSequence/" + FileCounter + ".png", Bytes);
        FileCounter++;
    }
}
