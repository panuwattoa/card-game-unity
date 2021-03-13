using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.Session;
using System;

public class roombutton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] GameObject popupNotMoeny;
    [SerializeField] TextMeshProUGUI online;

    private int roomValue;
    private async void Start()
    {
        if (int.TryParse(valueText.text, out int valurInt))
        {
            roomValue = valurInt;
        }
        var value = await GameApi.GetRoomOnline(valueText.text);
        online.text = value;
    }
    public async void OnClickJoinAsync()
    {
        var gold = PlayerWallet.GetWallet(NakamaSessionManager.Instance.Account.Wallet).gold;
 
        if (gold < roomValue*5)
        {
            popupNotMoeny.SetActive(true);
            // invite
            return;
        }
        var roomID = await GameApi.GetMatchWithValue(valueText.text, NakamaSessionManager.Instance.Account.User.Id);
        GameManager.Instance.JoinMatchRoomID(roomID);
    }
}
