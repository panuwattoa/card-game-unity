using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class popupShopInvite : MonoBehaviour
{


	public static popupShopInvite Create()
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/Lobby_Panel_Shop_Popup_invite");
		GameObject o = Instantiate(prefab);
		popupShopInvite hand = o.GetComponent<popupShopInvite>();
		return hand;
	}

	public void OnClickShop()
    {

    }

	public void OnClickAds()
	{

	}
	public void Close()
	{
		Destroy(gameObject);
	}

}
