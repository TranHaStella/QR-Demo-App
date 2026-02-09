using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Globalization;
using UnityEngine.InputSystem;
using System;
using System.Collections;


public class OpenLink : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text link;
    string pendingURL;
    float startTime;
    bool waittingResult = false;
    bool isValidURL(string url)
    {
        return url.Contains("://");
    }
    bool isDeepLink(string url)
    {
        return !url.StartsWith("http") || !url.StartsWith("https");
    }
    public void SetLink(string content)
    {
        if (isValidURL(content))
        { link.text = $"Liên kết website: <link={content}><u>{content}</u></link>"; }
        else
        {
            link.text = "Nội dung: " + content;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(">>> OnPointerClick CALLED <<<");
        link.ForceMeshUpdate();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(link, eventData.position, eventData.pressEventCamera);

        if (linkIndex == 1) return;
        if (linkIndex >= link.textInfo.linkInfo.Length) return;
        TMP_LinkInfo LinkInfo = link.textInfo.linkInfo[linkIndex];
        string url = LinkInfo.GetLinkID();
        Debug.Log("Đã Click:" + url);
        OpenWLink(url);
    }
    void OpenWLink(string url)
    {
        Application.OpenURL(url);
        pendingURL = url;
        startTime = Time.time;
        waittingResult = true;
    }
    void OpenWithStore()
    {
#if UNITY_ANDROID
        if (!Application.isFocused)
        {
            waittingResult = true;
            return;
        }

        if (Time.time - startTime > 1.2f)
        {
            waittingResult = true;
            if (isDeepLink(pendingURL))
            {
                Debug.Log(" App chưa cài --> Play Store");
                Application.OpenURL("market://details?id=" + Uri.EscapeDataString(pendingURL));
            }
        }
#endif
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //OpenWithStore();
    }
}
