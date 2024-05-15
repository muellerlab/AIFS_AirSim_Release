using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class AutoImageScript_Depth : MonoBehaviour
{
    private float nextPicTime = 10.0f;
    public float period = 1.0f;
    public int FileCounter = 0;
    public Shader effectsShader;
    public int imgHeight = 4800;
    public int imgWidth = 7200;
    public Shader blurShader;
    private Material blurMaterial;

    // Storing default camera settings
    private Color defaultBackgroundColor;
    private CameraClearFlags defaultClearFlags;

    // Configuration for blur

    void Start()
    {
        blurMaterial = new Material(blurShader);

        Camera Cam = GetComponent<Camera>();
        defaultBackgroundColor = Cam.backgroundColor;
        defaultClearFlags = Cam.clearFlags;
    }

    void Update()
    {
        if (Time.time > nextPicTime) {
            nextPicTime += period;
            Camera Cam = GetComponent<Camera>();
            RenderTexture renderTexture = RenderTexture.GetTemporary(imgWidth, imgHeight, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

            // Capture original image
            CaptureAndSaveImage(Cam, renderTexture, "Front");

            // Apply effects and capture again
            Cam.SetReplacementShader(effectsShader, "");
            Cam.backgroundColor = Color.white;
            Cam.clearFlags = CameraClearFlags.SolidColor;
            CaptureAndSaveImage(Cam, renderTexture, "Depth");

            // Reset camera settings to ensure next capture is without effects
            Cam.ResetReplacementShader();
            Cam.backgroundColor = defaultBackgroundColor;
            Cam.clearFlags = defaultClearFlags;

            // Clean up
            RenderTexture.ReleaseTemporary(renderTexture);
            FileCounter++;
        }
    }

    void CaptureAndSaveImage(Camera Cam, RenderTexture renderTexture, string suffix)
    {
        Cam.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;
        Cam.Render();

        if (suffix.Contains("Depth"))
        {
            ApplyBlur(renderTexture);
        }

        SaveImage(renderTexture, suffix);
        Cam.targetTexture = null;
        RenderTexture.active = null;
    }

    void ApplyBlur(RenderTexture source)
    {
        RenderTexture tempRT = RenderTexture.GetTemporary(source.width, source.height, 24, RenderTextureFormat.RGB565);
        Graphics.Blit(source, tempRT, blurMaterial);
        RenderTexture.active = source;
        GL.Clear(true, true, Color.clear);  // Clear to transparent
        Graphics.Blit(tempRT, source);
        RenderTexture.ReleaseTemporary(tempRT);
    }

    void SaveImage(RenderTexture renderTexture, string suffix)
    {
        Texture2D Image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        Image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        Image.Apply();
        byte[] Bytes = Image.EncodeToPNG();
        Destroy(Image);

        string fileName = $"{suffix}_{FileCounter}_{transform.position.x}_{transform.position.y}_{transform.position.z}.png";
        File.WriteAllBytes(Path.Combine(Application.dataPath, "ImageSequence", fileName), Bytes);
    }
}

