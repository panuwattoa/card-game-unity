using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Chatbubble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Name;
    [SerializeField] TextMeshProUGUI textChat;

	public static Chatbubble Create(string Name, string textChat, bool isMe, bool isRoom)
	{
		GameObject prefab;
		if (isMe)
        {
			if (isRoom)
            {
				prefab = Resources.Load<GameObject>("Prefabs/chat/chatbubble-room-me");

			}
			else
            {
				prefab = Resources.Load<GameObject>("Prefabs/chat/chatbubble-me");
			}

		}
		else
        {
			if (isRoom)
			{
				prefab = Resources.Load<GameObject>("Prefabs/chat/chatbubble-room");

			}
			else
			{
				prefab = Resources.Load<GameObject>("Prefabs/chat/chatbubble");
			}
		}
		GameObject o = Instantiate(prefab);
		Chatbubble hand = o.GetComponent<Chatbubble>();
		hand.Name.text = Name;
		hand.textChat.text = textChat;

		return hand;
	}
}
