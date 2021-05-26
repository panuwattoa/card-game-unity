using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Session;
using Nakama;
using Scripts.Utils;
using Nakama.TinyJson;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public enum OpCode
{
    OpCodeTerminate = 0,
    OpCodeJoinRequest ,
    OpCodeOtherJoin,
    OpCodeLeave,
    OpCodeStanUp,
    OpCodeFightForDealer,
    OpCodeRequestDealer,
    OpCodeUserBet,
    OpCodeUserPok,
    OpCodeJua,
    OpCodeJub,
    OpCodeSetDealer,
    OpCodePok,
    OpCodeChnageTurn,
    OpCodeBotJua,
    OpCodeDealCard,
    OpCodeGameStartBet,
    OpCodeOpeningTable,
    OpCodeNotJua,
    OpCodeClearTable,
    OpBotDealerCard,
    OpCodeGameResult,
    OpCodeSelfLeave,
    OpCodeOpenRequestDealer,
    OpCodeDealerResult,
    OpCodeCancelDealer,
    OpCodeJuaWithTimeOut,
    InviteBuy,
    OpCodePlayerJub,
}
public class TableScene : Singleton<TableScene>
{

    [SerializeField]
    private PlayerSlot[] playerSlot;
    [SerializeField]
    private PlayerSlot seatKing;
    [SerializeField] private CountdownTime countdownTime;
    [SerializeField] private BetController betBar;
    [SerializeField] private GameObject toast;
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private GameObject buttonJua;
    [SerializeField] private GameObject buttonNotJua;

    [SerializeField] private GameObject resultWin;
    [SerializeField] private GameObject resultLose;
    [SerializeField] private GameObject resultWinWin;
    [SerializeField] private GameObject chipWinOj;
    [SerializeField] private GameObject chipLoseOj;
    [SerializeField] private TextMeshProUGUI chipWinText;
    [SerializeField] private TextMeshProUGUI chipLoseText;
    [SerializeField] private GameObject ObtionPanel;
    [SerializeField] private Button requestKing;

    [SerializeField] private GameObject buttonDealerJua;
    [SerializeField] private GameObject buttonDealerJubList;
    [SerializeField] private DialogResultDealer dialogDealerResult;
    [SerializeField] private GameObject howto;
    [SerializeField] private CardJakScript cardDek;
    [SerializeField] private Animator cardDekAnimator;
    [SerializeField] private  Button buttonCancelDealer;
    [SerializeField] private Button leaveBtn;
    [SerializeField] private ChatRoomManager roomChat;
    public string RoomID { get; private set; }
    private IMatch match;
    NakamaSessionManager nakama;
    private List<IUserPresence> connectedOpponents;
    private Dictionary<int, PlayerSlot> seatPosition;
    private int activePostion;
    private int beginPos;
    private bool isPlaying;
    private bool isDealerBot;
    public void SetRoom(string roomID)
    {
        RoomID = roomID;
        nakama = NakamaSessionManager.Instance;
        connectedOpponents = new List<IUserPresence>(8);
        nakama.Socket.ReceivedMatchPresence += PresenceEvent;
        nakama.Socket.ReceivedMatchState += OnReceivedMatchState;
        nakama.Socket.ReceivedError += OnDisconnectedFromMasterServer;
        SendRequestRoomData();
    }

    private void SendRequestRoomData()
    {
        var newState = new Dictionary<string, string> {}.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeJoinRequest, newState);
    }


    private void PresenceEvent(IMatchPresenceEvent presenceEvent)
    {
        foreach (var presence in presenceEvent.Leaves)
        {
            connectedOpponents.Remove(presence);
            // todo: do something no leave room
        }
        connectedOpponents.AddRange(presenceEvent.Joins);
        Debug.LogFormat("Connected opponents: [{0}]", string.Join(",\n  ", connectedOpponents));
    }

    private void OnReceivedMatchState(IMatchState newState)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            var enc = System.Text.Encoding.UTF8;
            var content = enc.GetString(newState.State);
            //Debug.Log("Incom op " + (OpCode)newState.OpCode );
            //Debug.Log("Content " + content);

            switch (newState.OpCode)
            {
                case (int)OpCode.OpCodeJoinRequest:
                    {
                        isPlaying = false;
                        //Debug.Log("A custom opcode. " + content);
                        var join = JoinDetail.GetJoinDetail(content);
                        OnSetJoinUser(join);
                        Debug.Log("join Detail " + join.BetRate);
                        break;
                    }
                case (int)OpCode.OpCodeOtherJoin:
                    {
                        //Debug.Log("OpCodeOtherJoin " + content);
                        SetNewJoin(UserSit.GetUserSit(content));
                        break;
                    }
                case (int)OpCode.OpCodeLeave:
                    {
                        UserLeave(PlayerLeave.GetUserSit(content));
                        break;
                    }
                case (int)OpCode.OpCodeGameStartBet:
                    {
                        requestKing.gameObject.SetActive(false);
                        isPlaying = true;
                        countdownTime.SetTime(10);
                        countdownTime.gameObject.SetActive(true);
                        betBar.gameObject.SetActive(true);
                        //cardDek.OnSetCardJak(playerSlot);
                        //cardDek.gameObject.SetActive(true);
                        toastText.text = "กำลังลงเดิมพัน";
                        _ = StartCoroutine(nameof(WaitCloseToast));
                        break;
                    }
                case (int)OpCode.OpCodeUserBet:
                    {
                        //countdownTime.gameObject.SetActive(false);
                        //betBar.gameObject.SetActive(false);
                        break;
                    }
                case (int)OpCode.OpCodeOpeningTable:
                    {
                        toast.SetActive(true);
                        toastText.text = "กำลังรอเจ้ามือ ขอเป็นเจ้ามือสิ..";
                        break;
                    }
                case (int)OpCode.OpCodeSetDealer:
                    {
                       
                        toast.SetActive(true);
                        requestKing.gameObject.SetActive(false);
                        var dealer = SetDealer.GetDealer(content);
                        isDealerBot = dealer.IsDealerBot;
                        foreach (KeyValuePair<int, PlayerSlot> item in seatPosition)
                        {
                            item.Value.OnSetCloset(false);
                        }

                        if (dealer.IsDealerBot)
                        {
                            toastText.text = "เดี๋ยวข้าจะเป็นเจ้ามือให้เอง ฮ่า ๆๆ";
                        }
                        else
                        {
                            toastText.text = "["+ dealer.DealerName + "] เป็นเจ้ามือ";
                            seatPosition[dealer.Position].OnSetCloset(true);
                            if (seatPosition[dealer.Position].UUID.Equals(nakama.Account.User.Id))
                            {
                                buttonCancelDealer.gameObject.SetActive(true);
                                buttonCancelDealer.interactable = true;
                                isPlaying = true;
                            }
                            else
                            {
                                buttonCancelDealer.gameObject.SetActive(false);
                                buttonCancelDealer.interactable = false;
                            }
                            _ = StartCoroutine(nameof(WaitBetToast));
                        }
                        break;
                    }
                case (int)OpCode.OpCodeChnageTurn:
                    {
                        if (!isPlaying) return;
                        toast.SetActive(false);
                      //  betBar.gameObject.SetActive(false);
                        var turn = OnChangeTurn.GetUserTurn(content);
                        if (turn.UID.Equals(nakama.Account.User.Id))
                        {
                            if (turn.IsDealerTurn)
                            {
                                buttonDealerJua.SetActive(true);
                                buttonNotJua.SetActive(true);
                            }
                            else
                            {
                                buttonJua.SetActive(true);
                                buttonNotJua.SetActive(true);
                                buttonDealerJubList.SetActive(false);
                                buttonDealerJua.SetActive(false);
                            }
                        }
                        else
                        {
                            buttonJua.SetActive(false);
                            buttonNotJua.SetActive(false);
                            buttonDealerJua.SetActive(false);
                            buttonDealerJubList.SetActive(false);
                        }
                        //seatPosition[turn.Position]
                        foreach (KeyValuePair<int, PlayerSlot> item in seatPosition)
                        {
                            item.Value.DeActive();
                        }
                        countdownTime.gameObject.SetActive(false);
                        countdownTime.SetTime(turn.TurnTime);
                        countdownTime.gameObject.SetActive(true);
                        seatPosition[turn.Position].OnSetActive();
                        break;
                    }
                case (int)OpCode.OpCodeDealCard:
                    {
                        Debug.Log("Content " + content);
                        if (!isPlaying) return;
                        toast.SetActive(false);
                        var card = DealCard.GetHandCard(content);
                        betBar.gameObject.SetActive(false);
                        cardDek.gameObject.SetActive(false);
                        cardDekAnimator.Play("Card", -1, 0f);
                        //set open card
                        playerSlot[0].OnSetCard(card.HandCard, (PokdengCardResult)card.CardResult, card.PointMultiply, card.Point);

                        for (int i = 1; i < playerSlot.Length; i++)
                        {
                            if (playerSlot[i].m_isHavePlayerSit)
                            {
                                bool have = false;
                                foreach (var item in card.PlayerList)
                                {
                                    if (item.Equals(playerSlot[i].UUID))
                                    {
                                        have = true;
                                    }
                                }
                                if (have)
                                {
                                    playerSlot[i].OnSetCard(card.HandCard, (PokdengCardResult)card.CardResult, card.PointMultiply, card.Point);
                                }
                                // set back card
                            }
                        }
                        // set back card
                        //seatKing
                        if (isDealerBot)
                        {
                            seatKing.OnSetCard(card.HandCard, (PokdengCardResult)card.CardResult, card.PointMultiply, card.Point);
                        }

                        break;
                    }
                case (int)OpCode.OpCodeBotJua:
                    {
                        if (!isPlaying) return;
                        seatKing.AddCardToHand(0, 0, 0, 0);
                        break;
                    }
                case (int)OpCode.OpCodeClearTable:
                    {
                        buttonCancelDealer.gameObject.SetActive(false);
                        buttonCancelDealer.interactable = false;
                        dialogDealerResult.gameObject.SetActive(false);
                        requestKing.gameObject.SetActive(false);
                        betBar.gameObject.SetActive(false);
                        buttonJua.SetActive(false);
                        buttonNotJua.SetActive(false);
                        resultWin.SetActive(false);
                        chipWinOj.gameObject.SetActive(false);
                        resultLose.SetActive(false);
                        chipLoseOj.gameObject.SetActive(false);
                        resultWinWin.SetActive(false);
                        countdownTime.gameObject.SetActive(false);
                        buttonDealerJua.SetActive(false);
                        buttonDealerJubList.SetActive(false);

                        for (int i = 0; i < playerSlot.Length; i++)
                        {
                            if (playerSlot[i].m_isHavePlayerSit)
                            {
                                // set back card
                                playerSlot[i].Clear();
                            }
                        }

                        seatKing.Clear();
                        break;
                    }
                case (int)OpCode.OpCodePok:
                    {
                        if (!isPlaying) return;
                        var card = DealCard.GetHandCard(content);
                        seatPosition[card.Position].OpenCard(card.HandCard, (PokdengCardResult)card.CardResult, card.PointMultiply, card.Point);

                        break;
                    }
                case (int)OpCode.OpCodeJua:
                    {
                        if (!isPlaying) return;
                        betBar.gameObject.SetActive(false);
                        buttonJua.SetActive(false);
                        buttonNotJua.SetActive(false);
                        buttonDealerJua.SetActive(false);
                        buttonDealerJubList.SetActive(false);
                        var jua = PlayerJua.GetPlayerJua(content);
                        //seatPosition[jua.Position].AddCardToHand(jua.CardID, (PokdengCardResult)jua.CardResult, jua.PointMultiply, jua.Point);
                        seatPosition[jua.Position].AddCardToHandTimeOut(jua.CardID, (PokdengCardResult)jua.CardResult, jua.PointMultiply, jua.Point);
                        seatPosition[jua.Position].OnSetJuaSim();
                        break;
                    }
                case (int)OpCode.OpCodeJuaWithTimeOut:
                    {
                        if (!isPlaying) return;
                        betBar.gameObject.SetActive(false);
                        buttonJua.SetActive(false);
                        buttonNotJua.SetActive(false);
                        buttonDealerJua.SetActive(false);
                        buttonDealerJubList.SetActive(false);
                        var jua = PlayerJua.GetPlayerJua(content);
                        seatPosition[jua.Position].AddCardToHandTimeOut(jua.CardID, (PokdengCardResult)jua.CardResult, jua.PointMultiply, jua.Point);
                        seatPosition[jua.Position].OnSetJuaSim();
                        break;
                    }
                case (int)OpCode.OpCodeGameResult:
                    {
                        if (!isPlaying) return;
                        requestKing.gameObject.SetActive(false);
                        betBar.gameObject.SetActive(false);
                        buttonJua.SetActive(false);
                        buttonNotJua.SetActive(false);
                        buttonDealerJua.SetActive(false);
                        buttonDealerJubList.SetActive(false);
                        var re = PlayerResult.GetResult(content);
                        if (re.Result == 1)
                        {
                            resultWin.SetActive(true);
                            chipWinText.text = re.Chip.ToString();
                            chipWinOj.gameObject.SetActive(true);
                        }
                        else if (re.Result == 2) 
                        {
                            resultLose.SetActive(true);
                            chipLoseText.text = re.Chip.ToString();
                            chipLoseOj.gameObject.SetActive(true);

                        }
                        else
                        {
                            resultWinWin.SetActive(true);
                            chipWinText.text = re.Chip.ToString();
                            chipWinOj.gameObject.SetActive(true);
                        }
                        //NakamaSessionManager.Instance.SyncAccount();
                        playerSlot[0].OnSetTextGold(re.CurrentChip.ToString());
                        break;
                    }
                case (int)OpCode.OpBotDealerCard:
                    {
                        if (!isPlaying) return;
                        var card = DealCard.GetHandCard(content);
                        seatKing.OpenCard(card.HandCard, (PokdengCardResult)card.CardResult, card.PointMultiply, card.Point);
                        break;
                    }
                case (int)OpCode.OpCodeSelfLeave:
                    {
                        nakama.Socket.ReceivedMatchPresence -= PresenceEvent;
                        nakama.Socket.ReceivedMatchState -= OnReceivedMatchState;
                        GameManager.Instance.Match = null;
                        nakama.Socket.LeaveMatchAsync(RoomID);
                        StartCoroutine(LoadLobbySceneJoinRoom());
                        break;
                    }
                case (int)OpCode.OpCodeOpenRequestDealer:
                    {
                        requestKing.gameObject.SetActive(true);
                        requestKing.interactable = true;
                        break;
                    }
                case (int)OpCode.OpCodeDealerResult:
                    {
                        toast.SetActive(false);
                        var result = DealerResult.GetResult(content);
                        betBar.gameObject.SetActive(false);
                        buttonJua.SetActive(false);
                        buttonNotJua.SetActive(false);
                        buttonDealerJua.SetActive(false);
                        buttonDealerJubList.SetActive(false);
                        dialogDealerResult.SetData(result.DealerChipGenTotal, result.DealerWinTotal, result.DealerLostTotal, result.DealerWinName, result.DealerLostName);
                        dialogDealerResult.gameObject.SetActive(true);
                        playerSlot[0].OnSetTextGold(result.CurrentChip.ToString());
                        break;
                    }
                case (int)OpCode.OpCodeJub:
                    {
                        toast.SetActive(true);
                        var num = DealerJub.GetJub(content);
                        toastText.text = "เจ้ามือกำลังจับ [" + num.NumJub + "] ใบ";
                         
                        break;
                    }
                case (int)OpCode.InviteBuy:
                    {
                       if( GameManager.Instance != null)
                        {
                            GameManager.Instance.InviteBuy();
                        }
                        break;
                    }
                case (int)OpCode.OpCodeNotJua:
                    {
                        var jua = PlayerNotJua.GetPlayerNotJua(content);
                        seatPosition[jua.Position].OnSetNotJuaSim();
                        break;
                    }
                case (int)OpCode.OpCodePlayerJub:
                    {
                       var jub = PlayerJub.GetPlayerJub(content);
                        seatPosition[jub.Position].OnSetJub();
                        break;
                    }
                default:
                    {
                        Debug.LogFormat("opCode '{0}' sent '{1}'", newState.OpCode, content);
                        break;
                    }
            }
        });
    }

    IEnumerator WaitCloseToast()
    {
        yield return new WaitForSeconds(1);
        toast.SetActive(false);
    }

    IEnumerator WaitBetToast()
    {
        yield return new WaitForSeconds(1);
        toast.SetActive(true);
        toastText.text = "ผู้เล่นกำลัง ลงเดิมพัน...";
    }
    public void OnClickRequestDealer()
    {
        requestKing.interactable = false;
        var newState = new Dictionary<string, int> {}.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeRequestDealer, newState);
    }

    public void OnClickJub(int numJub)
    {
        buttonDealerJua.SetActive(false);
        buttonDealerJubList.SetActive(false);
        requestKing.interactable = false;
        var newState = new Dictionary<string, int> { { "jub", numJub} }.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeJub, newState);
    }

    private void UserLeave(PlayerLeave leave)
    {
        //if(leave.Position ==0)
        //{
        //    nakama.Socket.ReceivedMatchPresence -= PresenceEvent;
        //    nakama.Socket.ReceivedMatchState -= OnReceivedMatchState;
        //    GameManager.Instance.Match = null;
        //    StartCoroutine(LoadLobbySceneJoinRoom());
        //    return;
        //}
        if (leave.Position == 0) return;
        bool isMe = false;

        if (seatPosition.ContainsKey(leave.Position))
        {
            if (seatPosition[leave.Position].UUID.Equals(nakama.Account.User.Id))
            {
                isMe = true;
            }
            seatPosition[leave.Position].RemovePlayer();
            seatPosition.Remove(leave.Position);
            if (isMe)
            {
                nakama.Socket.ReceivedMatchPresence -= PresenceEvent;
                nakama.Socket.ReceivedMatchState -= OnReceivedMatchState;
                GameManager.Instance.Match = null;
                nakama.Socket.LeaveMatchAsync(RoomID);
                StartCoroutine(LoadLobbySceneJoinRoom());
            }
        }
    }


    private IEnumerator LoadLobbySceneJoinRoom()
    {
        var loadingOperation = SceneManager.LoadSceneAsync(1);
        while (!loadingOperation.isDone)
        {
            yield return null;
        }
    }


    private void SetNewJoin(UserSit user)
    {
        var index = user.Position - beginPos;
        if (index < 0)
        {
            index = 7 + index;
        }

        playerSlot[index].InitPlayer(user.Position, user.Name,false, user.UID);
        if (!seatPosition.ContainsKey(user.Position))
        {
            seatPosition.Add(user.Position, playerSlot[index]);
        }
        else
        {
            seatPosition[user.Position] = playerSlot[index];
        }
    }

    private void OnSetJoinUser(JoinDetail join)
    {
        foreach (var player in playerSlot)
        {
            player.RemovePlayer();
        }
        if (join.RoomPlaying)
        {
            toast.SetActive(true);
            toastText.text = "กำลังเล่นอยู่..กรุณารอสักครู่";
        }
        else
        {
            toast.SetActive(true);
            toastText.text = "เกมกำลังจะเริ่ม...";
        }
        betBar.ShowBetConsole(join.BetRate, join.MaxBetRate, BetCallBack);
        seatPosition = new Dictionary<int, PlayerSlot>();
        var self = join.UserSit.Where(x => x.UID == nakama.Session.UserId).FirstOrDefault();
        if (self != null)
        {
            beginPos = self.Position;
        }


        foreach (var u in join.UserSit)
        {
            var index = u.Position - beginPos;
            if (index < 0)
            {
                index = 7 + index;
            }
            var isMyPlayerSlot = true;
            if (index != 0 )
            {
                isMyPlayerSlot = false;
            }
            playerSlot[index].InitPlayer(u.Position, u.Name, isMyPlayerSlot,u.UID);
            if (seatPosition.ContainsKey(u.Position))
            {
                seatPosition[u.Position] = playerSlot[index];
            }
            else
            {
                seatPosition.Add(u.Position, playerSlot[index]);
            }
        }
        playerSlot[0].OnSetTextGold(PlayerWallet.GetWallet(nakama.Account.Wallet).gold.ToString());
        leaveBtn.interactable = true;
        roomChat.InitChat(RoomID);
    }

    private void BetCallBack(int chip)
    {
        var newState = new Dictionary<string, int> { { "bet", chip } }.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeUserBet, newState);
    }

    public void OnClickLeave()
    {
        ObtionPanel.gameObject.SetActive(false);
        toast.gameObject.SetActive(true);
        toastText.text = "ระบบจะออกจากห้องหลังจบเกม";
        var newState = new Dictionary<string, string> { { "UID", nakama.Account.User.Id } }.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeLeave, newState);
        //leaveBtn.interactable = false;
        StartCoroutine(WaitCloseToast());  
    }


    public void OnClickOption()
    {
        ObtionPanel.SetActive(!ObtionPanel.activeSelf);
    }
    public void OnClickJua()
    {
        buttonJua.SetActive(false);
        buttonNotJua.SetActive(false);
        buttonDealerJua.SetActive(false);
        buttonDealerJubList.SetActive(false);

        var newState = new Dictionary<string, int> {}.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeJua, newState);
    }

    public void OnClicNotJua()
    {
        buttonJua.SetActive(false);
        buttonNotJua.SetActive(false);
        buttonDealerJua.SetActive(false);
        var newState = new Dictionary<string, int> { }.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeNotJua, newState);
    }

    public void OnClickDealerJua()
    {
        buttonJua.SetActive(false);
        buttonNotJua.SetActive(false);
        buttonDealerJua.SetActive(false);
        buttonDealerJubList.SetActive(true);
    }

    protected override void OnDestroy()
    {
        nakama.Socket.ReceivedMatchPresence -= PresenceEvent;
        nakama.Socket.ReceivedMatchState -= OnReceivedMatchState;
        base.OnDestroy();
 
    }

    void OnApplicationQuit()
    {
        var newState = new Dictionary<string, string> { { "UID", nakama.Account.User.Id } }.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeLeave, newState);
    }

    private void OnDisconnectedFromMasterServer(Exception ee)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            popupNetWorkError.Create();
        });
      //  Debug.LogError(ee);
    }

    public void OnClickHowTo()
    {
        settingmanager.Create();
    }

    public void OnClickMuteAllSound()
    {
        AudioListener.pause = !AudioListener.pause;
    }

    public void OnClickCanCelDealer()
    {

        buttonCancelDealer.gameObject.SetActive(false);
        buttonCancelDealer.interactable = false;
        var newState = new Dictionary<string, int> { }.ToJson();
        nakama.Socket.SendMatchStateAsync(RoomID, (long)OpCode.OpCodeCancelDealer, newState);
    }

    public void OnClickChat()
    {
        roomChat.gameObject.SetActive(true);
    }

    
}
