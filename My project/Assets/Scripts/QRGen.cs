using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using ZXing;
using ZXing.QrCode;
using UnityEngine.Rendering.Universal;
using System.IO;
using NativeGalleryNamespace;
public class QRGen : MonoBehaviour

{
    public TMP_InputField inputContent;
    public RawImage qrImage;
    private BarcodeWriter writer;
    private Texture2D qrTexture;
    public GameObject InputQR;
    public GameObject QrCode;
    public Texture2D TemplateTexture;

    void Awake()
    {

        writer = new BarcodeWriter
        {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions
            {
                Width = 1024,
                Height = 1024,
                Margin = 2,
                CharacterSet = "UTF-8"
            }
        };
        qrTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        visibleGenQr();
        Debug.Log(qrTexture.width + "x" + qrTexture.height);
    }
    void visibleGenQr()
    {
        InputQR.SetActive(true);
        QrCode.SetActive(false);
    }
    void visibleQR()
    {
        InputQR.SetActive(false);
        QrCode.SetActive(true);
    }
    public void GenQr()
    {
        if (string.IsNullOrEmpty(inputContent.text))
            return;

        Color32[] pixelData = writer.Write(inputContent.text);

        qrTexture.SetPixels32(pixelData);
        qrTexture.Apply();

        qrImage.texture = qrTexture;

        visibleQR();
    }
    public void SaveQRWithBackground()
    {
        Texture2D bgTex = TemplateTexture;
        Texture2D qrTex = (Texture2D)qrImage.texture;
        Texture2D result = new Texture2D(
            bgTex.width,
            bgTex.height,
            TextureFormat.RGBA32,
            false
        );
        result.SetPixels(bgTex.GetPixels());
        int qrSize = qrTex.width;
        int posX = (bgTex.width - qrSize) / 2;
        int posY = (bgTex.height - qrSize) / 2;
        result.SetPixels(
            posX,
            posY,
            qrSize,
            qrSize,
            qrTex.GetPixels()
        );
        result.Apply();
        byte[] png = result.EncodeToPNG();

        NativeGallery.SaveImageToGallery(
            png,
            "QR App",
            "QR_" + System.DateTime.Now.Ticks + ".png"
        );
        Debug.Log("Saved QR to gallery!");
    }

    ///////////////

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
