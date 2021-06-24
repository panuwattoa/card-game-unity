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
#if UNITY_IOS
using AppleAuth;
using AppleAuth.Native;
using Unity.Advertisement.IosSupport;
#endif

public class LoginController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textLogin;
    [SerializeField] private GameObject loginPannel;
    [SerializeField] private TextMeshProUGUI uuid;
    [SerializeField] private TextMeshProUGUI version;
    [SerializeField] private GameObject loginWithApple;
    const string emailKey = "pokdeng-email";
    const string passwordKey = "pokdeng-password";
    private void Awake()
    {
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
        try
        {
            MobileAds.Initialize(initStatus => { });
#if !UNITY_EDITOR
            if (!FB.IsInitialized)
            { 
                    FB.Init(() =>
                    {
                        FB.ActivateApp();
                    });
            }
#else
            if (!FB.IsInitialized)
            { 
                FB.Init();
            }
#endif
        }
        catch (Exception e)
        {
            // Not supported on mac
#if !UNITY_OSX_STANDALONE
            Debug.LogWarning("Error initializing facebook: " + e.Message);
#endif
        }
#if UNITY_IOS
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            NakamaSessionManager.Instance.SetAppleAuth();
        }
#endif
    }

    public void OnClickOpenWeb(string url)
    {
        Application.OpenURL(url);
    }

    private void Update()
    {
#if UNITY_IOS

        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (NakamaSessionManager.Instance.appleAuthManager != null)
        {
            NakamaSessionManager.Instance.appleAuthManager.Update();
        }
#endif
    }


    public void OnClickLoginGuest()
    {
        PlayerPrefs.SetInt("logintype", (int)LoginType.Guest);
        loginPannel.SetActive(false);
        textLogin.text = "กำลังร้องขอข้อมูล";
        _ = NakamaSessionManager.Instance.ConnectWithGuest();
    }

    public void OnClickLoginFacebook()
    {
        PlayerPrefs.SetInt("logintype", (int)LoginType.Facebook);
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
        //await NakamaSessionManager.Instance.ConnectAsync();
        StartCoroutine(OnStartConnect());
    }

    private IEnumerator OnStartConnect()
    {
        var i = 0;
        while (i >= 10)
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
#if UNITY_IOS
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            loginWithApple.SetActive(true);
        }
#else
        loginWithApple.SetActive(false);
#endif
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
        textLogin.text = "กำลังเตรียมข้อมูล...";
        OnRequestIAPList();
        ReqestInitLoginData();
    }


    private async void OnCheckVersion()
    {
       var success = await GameApi.CheckVersion(NakamaSessionManager.Instance.GameVersion);
        Debug.Log("success  " + success);
        if (!success.Equals("true"))
        {
            popupMessage.Create("พบเวอร์ชั่นใหม่", "กรุณาอัพเดทเวอร์ชั่นใหม่", OnInviteUpdate);
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

    public void OnInviteUpdate()
    {
        Application.OpenURL("market://details?id=com.rhythmstudio.pokdeng");
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

    private async void ReqestInitLoginData()
    {
        string list = await GameApi.ReqestInitLoginData();
        GameManager.Instance.loginRequestData = LoginRequestData.GetDetail(list);
    }

    private async void OnRequestIAPList()
    {
        string list = await GameApi.ReqestIAPlist();
        var detail = IAPList.GetDetail(list);
        IAPManager.Instance.IapProduct = new List<IapStruct>();
        foreach (var item in detail.iap)
        {
            IapStruct iap = new IapStruct
            {
                productName = item.product_id,
                productText = item.product_name,
                productType = 1,
                value = item.gold
            };
            IAPManager.Instance.IapProduct.Add(iap);
        }
        IAPManager.Instance.InitializePurchasing();
    }

    public void OnSignInWithApple()
    {
        PlayerPrefs.SetInt("logintype", (int)LoginType.Apple);
        loginPannel.SetActive(false);
        textLogin.text = "กำลังร้องขอข้อมูล";
        NakamaSessionManager.Instance.SigninWithApple();
    }
}
