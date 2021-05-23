using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Utils;
using Nakama;
using Scripts.Session;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : Singleton<GameManager>
{
    public List<IapStruct> IAPProductList = new List<IapStruct>();

    public IMatch Match { get; set; }
    public GameObject loading;
    // Start is called before the first frame update

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
        popupShopInvite.Create();
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
