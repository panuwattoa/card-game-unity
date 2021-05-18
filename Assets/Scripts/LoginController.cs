using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.Session;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System;
using GoogleMobileAds.Api;
using Facebook.Unity;

public class LoginController : MonoBehaviour
{
    [SerializeField] private string regisUrl;
    [SerializeField] private TextMeshProUGUI textLogin;
    [SerializeField] private GameObject loginPannel;
    [SerializeField] private TMP_InputField email;
    [SerializeField] private TMP_InputField password;
    [SerializeField] private TextMeshProUGUI uuid;
    [SerializeField] private TextMeshProUGUI version;
    [SerializeField] private GameObject dialogUpgrade;

    const string emailKey = "pokdeng-email";
    const string passwordKey = "pokdeng-password";
    private void Awake()
    {
        try
        {
            MobileAds.Initialize(initStatus => { });
#if !UNITY_EDITOR
                    FB.Init(() =>
                    {
                        FB.ActivateApp();
                    });
#else
        FB.Init();
#endif
        }
        catch (Exception e)
        {
            // Not supported on mac
#if !UNITY_OSX_STANDALONE
            Debug.LogWarning("Error initializing facebook: " + e.Message);
#endif
        }
    }
    public void OnClickRegis()
    {
        Application.OpenURL(regisUrl);
    }

    public void OnClickOpenWeb(string url)
    {
        Application.OpenURL(url);
    }


    public void OnClickLoginWithEmailAsync()
    {
        if (!string.IsNullOrEmpty(email.text) && !string.IsNullOrEmpty(password.text))
        {
            textLogin.text = "กำลังเข้าสู่ระบบ..";
            PlayerPrefs.SetString(emailKey, email.text);
            PlayerPrefs.SetString(passwordKey, password.text);
            loginPannel.SetActive(false);
            _ = NakamaSessionManager.Instance.ConnectWithEmailAsync(email.text, password.text);
        }
    }


    public void OnClickLoginGuest()
    {
        loginPannel.SetActive(false);
        textLogin.text = "กำลังร้องขอข้อมูล";
        _ = NakamaSessionManager.Instance.ConnectWithGuest();
    }

    public void OnClickLoginFacebook()
    {
        loginPannel.SetActive(false);
        textLogin.text = "กำลังร้องขอข้อมูล facebook";
        NakamaSessionManager.Instance.LinkFacebook();

    }

    // Start is called before the first frame update
    void Start()
    {
        version.text = NakamaSessionManager.Instance.GameVersion.ToString();
        textLogin.text = "กำลังเชื่อมต่อ..";
        Application.runInBackground = true;
        NakamaSessionManager.Instance.OnLoginFail += OnLoginFail;
        NakamaSessionManager.Instance.OnConnectionSuccess += OnLoginSucess;
        NakamaSessionManager.Instance.OnDisconnected += OnDisconnect;
        NakamaSessionManager.Instance.OnConnectionFailure += OnConnectionFail;
        StartCoroutine(OnStartConnect());
    }

    private IEnumerator OnStartConnect()
    {
        var i = 0;
        while (i>= 10)
        {
            if (FB.IsInitialized)
            {
                break;
            }
            yield return new WaitForSeconds(1f);
            i++;
        }
       
        _ = NakamaSessionManager.Instance.ConnectAsync();
    }

    private void OnLoginFail()
    {
        loginPannel.SetActive(true);
        textLogin.text = "ไม่พบบัญชีผู้ใช้.";

        Debug.Log("Login fail");
    }

    private void OnLoginSucess()
    {
        textLogin.text = "สำเร็จ..";
        uuid.text = NakamaSessionManager.Instance.Account.User.Id;
        textLogin.text = "กำลังตรวจสอบเวอร์ชั่น...";
        OnCheckVersion();
    }


    private async void OnCheckVersion()
    {
       var success = await GameApi.CheckVersion(NakamaSessionManager.Instance.GameVersion);
        Debug.Log("success  " + success);
        if (!success.Equals("true"))
        {
            dialogUpgrade.SetActive(true);
            popupMessage.Create("พบเวอร์ชั่นใหม่", "กรุณาอัพเดทเวอร์ชั่นใหม่");
            return;
        }
        textLogin.text = "กำลังพาท่านข้าสู่ห้องโถง..";
        SceneManager.LoadScene("LobbyScene");
    }

    private void OnDisconnect()
    {
        textLogin.text = "ขาดการเชื่อมต่อ..";
        popupMessage.Create("ไม่สามารถเชื่อมต่อ", "ไม่สามารถเชื่อมต่อเซิฟเวอร์ได้\nเราอาจจะกำลังปิดปรับปรุงระบบ", OnReconection);

    }


    private void OnConnectionFail()
    {
        textLogin.text = "ไม่สามารถเชื่อมต่อเซิฟเวอร์ได้..";
        popupMessage.Create("ไม่สามารถเชื่อมต่อ", "ไม่สามารถเชื่อมต่อเซิฟเวอร์ได้\nเราอาจจะกำลังปิดปรับปรุงระบบ", OnReconection);
        // loginPannel.SetActive(true);
    }


    private void OnDestroy()
    {
        if (NakamaSessionManager.Instance != null)
        {
            NakamaSessionManager.Instance.OnLoginFail -= OnLoginFail;
            NakamaSessionManager.Instance.OnConnectionSuccess -= OnLoginSucess;
            NakamaSessionManager.Instance.OnDisconnected -= OnDisconnect;
            NakamaSessionManager.Instance.OnConnectionFailure -= OnConnectionFail;
        }
    }


    private async void OnReconection()
    {
        _ = await NakamaSessionManager.Instance.ConnectAsync();

    }

}
