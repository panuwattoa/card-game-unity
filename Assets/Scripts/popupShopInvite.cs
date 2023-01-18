using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class popupShopInvite : MonoBehaviour
{

	public TMP_InputField ssoid;
	public TMP_InputField purchase_id;
	private Action<string,string> m_callBack;

	public static popupShopInvite Create(Action<string,string> callBack = null)
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/Lobby_Popup_Name");
		GameObject o = Instantiate(prefab);
		popupShopInvite hand = o.GetComponent<popupShopInvite>();
		hand.m_callBack = callBack;
		return hand;
	}

	public void OnClickShop()
    {

		m_callBack?.Invoke(ssoid.text, purchase_id.text);
		Close();

	}

	public void OnClickAds()
	{

	}
	public void Close()
	{
		Destroy(gameObject);
	}

}
