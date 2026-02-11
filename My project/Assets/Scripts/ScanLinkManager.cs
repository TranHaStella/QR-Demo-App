using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using UnityEngine.SceneManagement;
using NativeGalleryNamespace;

public class ScanLinkManager : MonoBehaviour
{
    [SerializeField]
    private RawImage cameraView;
    private WebCamTexture webcam;
    public TMP_Text linkScan;
    public OpenLink openLink;
    public AspectRatioFitter ratioFitter;
    [Header("UI")]
    public GameObject scanQr;
    public GameObject scanSucces;
    public GameObject choiceGenQR;
    private float scanTimer = 0f;
    private float scanDelay = 0.5f;
    private IBarcodeReader reader;
    bool isScaning = true;
    [Header("information")]
    public TMP_Text info;
    public GameObject infoPanel;

    void Awake()
    {
        infoPanel.SetActive(false);
        reader = new BarcodeReader();
        Application.deepLinkActivated += OpenAppWithLink;
        if (!string.IsNullOrEmpty(Application.absoluteURL))
        {
            OpenAppWithLink(Application.absoluteURL);
        }
    }
    public void Hide()
    {
        infoPanel.SetActive(false);
    }
    void OpenWebCam()
    {
        if (webcam == null)
        {
            webcam = new WebCamTexture(1920, 1080, 30);
            cameraView.texture = webcam;
            reader = new BarcodeReader();
        }

        if (!webcam.isPlaying)
            webcam.Play();
    }
    void ScanDelay()
    {
        if (!isScaning) return;
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanDelay)
        {
            scanTimer = 0f;
            ScanQrCode();
        }
    }
    void visiblescanQR()
    {
        scanQr.SetActive(true);
        scanSucces.SetActive(false);
        choiceGenQR.SetActive(false);

    }
    void visiblescanSucces()
    {
        scanQr.SetActive(false);
        scanSucces.SetActive(true);
        choiceGenQR.SetActive(false);
    }
    void visibleChoiceGenQR()
    {
        scanQr.SetActive(false);
        scanSucces.SetActive(false);
        choiceGenQR.SetActive(true);
    }
    void ScanQrCode()
    {
        if (webcam.width < 100) return;
        Color32[] pixels = webcam.GetPixels32();
        int width = webcam.width;
        int height = webcam.height;
        DecodeQr(pixels, width, height);
    }
    public void DecodeQr(Color32[] pixels, int width, int height)
    {
        var result = reader.Decode(pixels, width, height);
        if (result != null)
        {
            visiblescanSucces();
            openLink.SetLink(result.Text);
            Debug.Log("Qr Link: " + result.Text);
            webcam?.Stop();
            isScaning = false;
        }
    }
    void UpdateCameraView()
    {
        if (webcam == null || !webcam.isPlaying)
            return;

        cameraView.rectTransform.localEulerAngles =
            new Vector3(0, 0, -webcam.videoRotationAngle);

        cameraView.rectTransform.localScale =
            new Vector3(1, webcam.videoVerticallyMirrored ? -1 : 1, 1);

        ratioFitter.aspectRatio =
            (float)webcam.width / webcam.height;
    }

    public void ReturnCam()
    {
        visiblescanQR();
        OpenWebCam();
        isScaning = true;
    }
    public void OpenGenCode()
    {
        visibleChoiceGenQR();
    }
    public void loadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

    }
    public void UploadImage()
    {
        NativeGallery.GetImageFromGallery((path)
       =>
        {
            if (path == null)
            {
                Debug.Log("User is canceled");
                return;
            }
            if (path != null)
            {
                Texture2D texture2D = NativeGallery.LoadImageAtPath(path, 1024, false);
                if (texture2D == null)
                {
                    Debug.Log("Couldn't load texture form" + path);
                    return;
                }
                Color32[] pixels = texture2D.GetPixels32();
                DecodeQr(pixels, texture2D.width, texture2D.height);
            }
        });
    }
    public void OpenAppWithLink(string url)
    {
        infoPanel.SetActive(true);
        info.text = "Ban da vao app bang lien ket " + url;
    }
    //////////////////////////////////////////
    void OnEnable()
    {
        OpenWebCam();
        visiblescanQR();
        enabled = true;
    }
    void OnDisable()
    {

    }
    // Start is called  before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OpenWebCam();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCameraView();
        ScanDelay();
    }

}
