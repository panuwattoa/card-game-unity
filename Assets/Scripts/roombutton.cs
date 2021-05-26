using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.Session;
using System;

public class roombutton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] private GameObject invitBuy;

    private int roomValue;
    private  void Start()
    {
        if (int.TryParse(valueText.text, out int valurInt))
        {
            roomValue = valurInt;
        }

    }
    public async void OnClickJoinAsync()
    {
        var gold = PlayerWallet.GetWallet(NakamaSessionManager.Instance.Account.Wallet).gold;
 
        if (gold < roomValue*5)
        {
            invitBuy.SetActive(true);
            // invite
            return;
        }
        var roomID = await GameApi.GetMatchWithValue(valueText.text, NakamaSessionManager.Instance.Account.User.Id);
        GameManager.Instance.JoinMatchRoomID(roomID);
    }
}
