using UnityEngine;
using ZXing;
public class ZXingTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var reader = new BarcodeReader();
        Debug.Log("ZXING IS OK!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
