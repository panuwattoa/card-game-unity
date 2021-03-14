using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class popupNetWorkError : MonoBehaviour
{
	public static popupNetWorkError Create()
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/popup-net-error");
		GameObject o = Instantiate(prefab);
		popupNetWorkError hand = o.GetComponentInChildren<popupNetWorkError>();
		return hand;
	}




	public void Close()
	{
		Destroy(gameObject);
		StartCoroutine(LoadLogin());
	}

	private IEnumerator LoadLogin()
	{
		var loadingOperation = SceneManager.LoadSceneAsync(0);
		while (!loadingOperation.isDone)
		{
			yield return null;
		}
	}

}
