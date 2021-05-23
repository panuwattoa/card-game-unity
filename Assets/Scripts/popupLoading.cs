using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class popupLoading : MonoBehaviour
{
	private Action m_callBack;

	public static GameObject Create( Action callBack = null)
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/Prefabs/popup/loading");
		GameObject o = Instantiate(prefab);
		popupLoading hand = o.GetComponentInChildren<popupLoading>();

		hand.m_callBack = callBack;

		return o;
	}


	public void Close()
    {
		m_callBack?.Invoke();
		Destroy(gameObject);
    }


}
