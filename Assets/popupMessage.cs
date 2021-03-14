using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class popupMessage : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI title;
	[SerializeField] TextMeshProUGUI mesg;
	public static popupMessage Create(string title, string msg)
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/popup/popup-mesg");
		GameObject o = Instantiate(prefab);
		popupMessage hand = o.GetComponentInChildren<popupMessage>();
		hand.title.text = title;
		hand.mesg.text = msg;
		return hand;
	}


	public void Close()
    {
		Destroy(gameObject);
    }


}
