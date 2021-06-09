using System.Collections;
using System.Collections.Generic;
using Scripts.Session;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class LobbyController : MonoBehaviour,Observer
{
    [SerializeField] private TextMeshProUGUI m_userName;
    [SerializeField] private TextMeshProUGUI m_currentGold;
    [SerializeField] private TextMeshProUGUI m_currentShopGold;
    
    [SerializeField] private TextMeshProUGUI m_currentGem;
    [SerializeField] private TextMeshProUGUI m_uuid;
    [SerializeField] private GameObject popUpSetName;
    [SerializeField]  private GameObject howto;
    [SerializeField] private GameObject shopPannel;
    [SerializeField] private GameObject chatPannel;
    [SerializeField] private GameObject invitBuy;
    [SerializeField] private GameObject playReward;
    [SerializeField] private GameObject dailyReward;

    // Start is called before the first frame update
    NakamaSessionManager nakama;
    async void Start()
    {
    
        nakama = NakamaSessionManager.Instance;

        if (!NakamaSessionManager.Instance.IsConnected)
        {
            SceneManager.LoadScene("LodingScene");
            return;
        }
        m_currentGold.text = "loading..";
      var wallet = await NakamaSessionManager.Instance.SyncAccount();
        if (string.IsNullOrEmpty(nakama.Account.User.DisplayName))
        {
            popUpSetName.SetActive(true);
        }
        else
        {
            m_userName.text = nakama.Account.User.DisplayName;
        }
        m_currentGold.text = string.Format("{0:n0}", PlayerWallet.GetWallet(wallet).gold);
        m_currentGem.text = "0";
        m_currentShopGold.text = m_currentGold.text;
        m_uuid.text = nakama.Account.User.Username;
        nakama.AddObserver(this);
        Debug.LogFormat("User wallet: '{0}'", nakama.Account.Wallet);

        var data = await GameApi.ReqestGetUserData();
        GameManager.Instance.SetUserData(UserData.GetDetail(data));
        if (GameManager.Instance.IsFirstOpen)
        {
            var dataReward = GameManager.Instance.loginRequestData.dailyRewardData;
            if (dataReward != null)
            {
                if (dataReward.reward.Length > 0 && !GameManager.Instance.UserData.is_recived)
                {
                    dailyReward.SetActive(true);
                }
            }
            var p = GameManager.Instance.loginRequestData.playRewardData;
            if (p != null)
            {
                if (p.reward != null)
                {
                    playReward.SetActive(true);
                }
            }
            GameManager.Instance.IsFirstOpen = false;
        }
        else
        {
            if (PlayerWallet.GetWallet(wallet).gold <= 250)
            {
                invitBuy.SetActive(true);
            }
        }
       
    }



    public void OnClickPopupBuyClose(GameObject go)
    {
        go.SetActive(false);
    }

    public async void OnSetName(TMP_InputField input)
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            popUpSetName.SetActive(false);
            await nakama.Client.UpdateAccountAsync(nakama.Session, null, input.text);
            m_userName.text = input.text;
        }
    }
    private void OnDestroy()
    {
        if (NakamaSessionManager.Instance != null)
        {
            NakamaSessionManager.Instance.RemoveObserver(this);
        }
    }

    public void OnClickMuteAllSound()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    public void OnClickOpenWeb(string url)
    {
#if UNITY_WEBGL
        Application.ExternalEval("window.open('" + url + "', '_blank')");
#else
        Application.OpenURL(url);
#endif
    }


    public void OnClickDialogHowto()
    {
        howto.SetActive(true);
    }

    private async void RefreshWallet()
    {
        var wallet = await NakamaSessionManager.Instance.SyncAccount();
        m_currentGold.text = string.Format("{0:n0}", PlayerWallet.GetWallet(wallet).gold);
        m_currentShopGold.text = m_currentGold.text;
    }

    public void Notify(Subject o)
    {
        RefreshWallet();
    }

    public void OnClickCloseShopPannel()
    {
        shopPannel.SetActive(false);
    }

    public void OnClickOpenShopPannel()
    {
        shopPannel.SetActive(true);
    }

    public void OnClickOpenSetting()
    {
        settingmanager.Create();
    }

    public void OnClickBubbleChat()
    {
        chatPannel.SetActive(true);
    }


    public void OnClickDailyReward()
    {
        var data = GameManager.Instance.loginRequestData.dailyRewardData;
        if (data != null)
        {
            if (data.reward.Length > 0)
            {
                dailyReward.SetActive(true);
                return;
            }
        }

        popupMessage.Create("ระบบ", "หมดเวลากิจกรรม");

    }

    public void OnClickMission()
    {
        var data = GameManager.Instance.loginRequestData.playRewardData;
        if (data != null)
        {
            if (data.reward.Length > 0)
            {
                playReward.SetActive(true);
                return;
            }
        }
        popupMessage.Create("ระบบ", "หมดเวลากิจกรรม");
    }

}
