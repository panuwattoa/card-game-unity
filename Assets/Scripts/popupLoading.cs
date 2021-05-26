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
		GameObject prefab = Resources.Load<GameObject>("Prefabs/loading");
		GameObject o = Instantiate(prefab);


		return o;
	}


	public void Close()
    {
		m_callBack?.Invoke();
		Destroy(gameObject);
    }


}
