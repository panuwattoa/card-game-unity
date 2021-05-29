using System.Collections;
using System.Collections.Generic;
using Nakama;
using Scripts.Session;
using UnityEngine;
using TMPro;
using Nakama.TinyJson;

public class ChatManager : MonoBehaviour
{
    [SerializeField] GameObject pannel;
    [SerializeField] TMP_InputField textInput;
    // Start is called before the first frame update
    private string roomname = "public-lobby";
    IChannel channel;
    [SerializeField]
    TextMeshProUGUI count;
    private int countChat;

    async void Start()
    {
        var persistence = true;
        var hidden = false;
         channel = await NakamaSessionManager.Instance.Socket.JoinChatAsync(roomname, ChannelType.Room, persistence, hidden);
        Debug.LogFormat("Now connected to channel id: '{0}'", channel.Id);


        NakamaSessionManager.Instance.Socket.ReceivedChannelMessage += OnRecieveChat;
    }

    private void OnRecieveChat(IApiChannelMessage message)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Chatbubble go;
            ChatRoom msg = ChatRoom.GetDetail(message.Content);
            if (message.SenderId == NakamaSessionManager.Instance.Account.User.Id)
            {

                go = Chatbubble.Create(msg.name, msg.msg, true,false);
            }
            else
            {
                go = Chatbubble.Create(msg.name, msg.msg, false,false);
            }
            countChat++;
            count.text = countChat.ToString();
            go.transform.SetParent(pannel.transform);
            go.transform.localScale = new Vector3(1, 1, 1);
            CheckChat();
        });
    }
    private void CheckChat()
    {
        if (pannel.transform.childCount > 10 )
        {
             Destroy(pannel.transform.GetChild(0).gameObject);  
        }
    }

    private void OnDisable()
    {
        countChat = 0;
        count.text = countChat.ToString();
    }

    public void OnClickSendMsg()
    {
        if (!string.IsNullOrEmpty(textInput.text))
        {
            OnSend(textInput.text);
            textInput.text = "";
        }

    }

    private async void OnSend(string text)
    {
        try
        {
            var content = new Dictionary<string, string> { { "name",NakamaSessionManager.Instance.Account.User.DisplayName },{ "msg", text } }.ToJson();
            var sendAck = await NakamaSessionManager.Instance.Socket.WriteChatMessageAsync(channel.Id, content);
            Debug.Log(sendAck);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);

        }
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (NakamaSessionManager.Instance != null)
        {
            NakamaSessionManager.Instance.Socket.ReceivedChannelMessage -= OnRecieveChat;

            _ = NakamaSessionManager.Instance.Socket.LeaveChatAsync(channel.Id);
        }
    }
}
