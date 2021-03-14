using System.Collections;
using System.Collections.Generic;
using Scripts.Session;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class LobbyController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_userName;
    [SerializeField] private TextMeshProUGUI m_currentGold;
    [SerializeField] private TextMeshProUGUI m_currentGem;
    [SerializeField] private TextMeshProUGUI m_uuid;
    [SerializeField] private GameObject popUpSetName;
    [SerializeField]
    private GameObject howto;
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
        m_currentGold.text = string.Format("{0:n0}", PlayerWallet.GetWallet(wallet).gold.ToString());
        m_currentGem.text = "0";
        m_uuid.text = nakama.Account.User.Username;
        Debug.LogFormat("User wallet: '{0}'", nakama.Account.Wallet);
    }

    public void OnClickPopupBuyMoney(GameObject go)
    {
        Application.OpenURL("https://p8p9.app/#/deposit");
        go.SetActive(false);
    }

    public void OnClickShop()
    {
        Application.OpenURL("https://p8p9.app/#/deposit");
    }

    public void OnClickPopupBuyClose(GameObject go)
    {
        go.SetActive(false);
    }

    public async void OnSetName(TMP_InputField input)
    {
        popUpSetName.SetActive(false);
        if (!string.IsNullOrEmpty(input.text))
        {
            await nakama.Client.UpdateAccountAsync(nakama.Session, null, input.text);
            m_userName.text = input.text;
        }
    }
    private void OnDestroy()
    {

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
}
