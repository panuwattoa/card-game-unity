using System.Collections;
using System.Collections.Generic;
using Scripts.Session;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class settingmanager : MonoBehaviour
{

    [SerializeField] private Slider soundSlider;
    [SerializeField] private Button b_logout;

    // Start is called before the first frame update

    public static void Create()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/setting");
        Instantiate(prefab);
        return;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("gamebgm"))
        {
            soundSlider.value = PlayerPrefs.GetFloat("gamebgm");
        }

        if (SceneManager.GetActiveScene().name == "TableScene")
        {
            b_logout.interactable = false;
        }
    }


    public void OnValueChangeBGM(System.Single value)
    {
        soundSlider.value = value;
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("gamebgm", value);
    }
    
    public void OnClickOnOpenWeb(string url)
    {
        Application.OpenURL(url);
    }

    public void OnClose()
    {
        Destroy(gameObject);
    }

    public void OnClickLogout()
    {
        GameManager.Instance.IsFirstOpen = true;
        PlayerPrefs.SetString("nakama.authToken", null);
        PlayerPrefs.SetInt("logintype", (int)LoginType.None);
         _ =  NakamaSessionManager.Instance.DisconnectWithOutPopupAsync();
        StartCoroutine(LoadLogin());
    }


    private IEnumerator LoadLogin()
    {
        var loadingOperation = SceneManager.LoadSceneAsync(0);
        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }
}
