using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;
using Nakama;
using Scripts.Session;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Threading.Tasks;

public class GameManager : Singleton<GameManager>
{
    public UserData UserData { get; private set; }
    public IMatch Match { get; set; }
    public GameObject loading;
    public LoginRequestData loginRequestData;
    public bool IsFirstOpen = true;
    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (PlayerPrefs.HasKey("gamebgm"))
        {
            AudioListener.volume = PlayerPrefs.GetFloat("gamebgm");
        }

    }
    public async void JoinMatchRoomID(string roomID)
    {
        try
        {
            loading.SetActive(true);
            Match = await NakamaSessionManager.Instance.Socket.JoinMatchAsync(roomID);
            if (Match != null)
            {
                StartCoroutine(LoadTableSceneJoinRoom(roomID));
            }
            //Match = null;
        }
        catch (System.Exception e )
        {
            loading.SetActive(false);
            // Debug.LogError(ex);
            if (e is TaskCanceledException)
            {
        
                    StartCoroutine(LoadLogin());
               
            }
            else
            {
                popupMessage.Create("ผิดพลาด", e.Message);
            }
        }
        
    }

    private IEnumerator LoadTableSceneJoinRoom(string roomID)
    {
        var loadingOperation = SceneManager.LoadSceneAsync(2);
        while (!loadingOperation.isDone)
        {
            yield return null;
        }
        loading.SetActive(false);
        TableScene.Instance.SetRoom(roomID);
    }


    private IEnumerator LoadLogin()
    {
        var loadingOperation = SceneManager.LoadSceneAsync(0);
        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }

    public void InviteBuy()
    {
        popupShopInvite.Create();
    }

    public void SetUserData(UserData u)
    {
        UserData = u;
    }

}
[Serializable]
public class IapStruct
{
    public string productName;
    public string productText;
    [Header("--- 1 = Diamond 2 = Money 3 = Animal(TODO)")]
    public int productType;
    public int value;
}
