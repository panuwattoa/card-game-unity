using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class settingmanager : MonoBehaviour
{

    [SerializeField] private Slider soundSlider;
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
}
