using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class popupMessage : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI title;
	[SerializeField] TextMeshProUGUI mesg;
	private Action m_callBack;

	public static popupMessage Create(string title, string msg, Action callBack = null)
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/popup-mesg");
		GameObject o = Instantiate(prefab);
		popupMessage hand = o.GetComponentInChildren<popupMessage>();
		hand.title.text = title;
		hand.mesg.text = msg;
		hand.m_callBack = callBack;

		return hand;
	}


	public void Close()
    {
		m_callBack?.Invoke();
		Destroy(gameObject);
    }


}
