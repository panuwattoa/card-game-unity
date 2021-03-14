using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;
using Nakama;
using Scripts.Session;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : Singleton<GameManager>
{
    public IMatch Match { get; set; }
    public GameObject loading;
    public GameObject Invite;
    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

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
            popupMessage.Create("ผิดพลาด", e.Message);
           // Debug.LogError(ex);
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




    public void InviteBuy()
    {
        Invite.SetActive(true);
    }

}
