using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField] private Image profile;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private GameObject goldOj;
    [SerializeField] private TextMeshProUGUI textGold;
    [SerializeField] private Image[] card;
    [SerializeField] private GameObject closetKing;
    [SerializeField] private GameObject closetCrown;
    [SerializeField] private GameObject currentActive;
    [SerializeField] private Image currentBet;
    [SerializeField] private TextMeshProUGUI beginName;
    [SerializeField] private CardSlotController cardSlotController;
    [SerializeField] private GameObject juaSim;
    [SerializeField] private GameObject notJuaSim;
    [SerializeField] private GameObject jub;

    public int position { get; private set; }
    public bool m_isMyPlayerSlot { get; private set; }
    public bool m_isHavePlayerSit{ get; private set; }

    public string UUID { get; private set; }

    private string pName;
    public void InitPlayer(int position, string playerName,bool isMyPlayerSlot, string UUID)
    {
        m_isHavePlayerSit = true;
        Debug.Log("isMyPlayerSlot " + isMyPlayerSlot);
        m_isMyPlayerSlot = isMyPlayerSlot;
        this.position = position;
        pName = playerName;
        this.playerName.gameObject.SetActive(true);
        this.playerName.text = pName;
        this.position = position;
        textGold.text = "";
        goldOj.SetActive(true);
       // profile.gameObject.SetActive(true);
        beginName.text = pName.Substring(0, 1);
        this.UUID = UUID;
    } 


    //private void Update()
    //{
    //    if (isSetData)
    //    {
 
    //        isSetData = false;
    //    }
    //}
    public void OnSetTextGold(string gold)
    {
        goldOj.SetActive(true);
        textGold.text = string.Format("{0:n0}", gold);
    }
    public void OnSetActive(bool isActive)
    {
        currentActive.SetActive(isActive);
    }

    public void OnSetCard(HandCard[] card, PokdengCardResult cardResult, int multiply, int point)
    {
        if (m_isMyPlayerSlot)
        {
            Debug.Log("isMyPlayerSlot multiply " + m_isMyPlayerSlot);
            foreach (var c in card)
            {
                cardSlotController.SetCardOnHand(c.CardID);
            }
            cardSlotController.SetCardResultAndMultiply(cardResult, multiply, point);
            cardSlotController.BeginOpenMyCard();
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                cardSlotController.SetCardOnHand(53);
            }
            cardSlotController.BiginInitAnotherCard();

         }
    }


    public void AddCardToHand(int card, PokdengCardResult cardResult, int multiply, int point)
    {
        if (m_isMyPlayerSlot)
        {
            cardSlotController.SetCardOnHand(card);
            cardSlotController.SetCardResultAndMultiply(cardResult, multiply, point);
            cardSlotController.BeginOpenMyCard();
        }
        else
        {
            cardSlotController.SetCardOnHand(53);
            cardSlotController.BiginInitAnotherCardThree();
        }
    }

    public void AddCardToHandTimeOut(int card, PokdengCardResult cardResult, int multiply, int point)
    {
        if (m_isMyPlayerSlot)
        {
            cardSlotController.SetCardOnHand(card);
            cardSlotController.SetCardResultAndMultiply(cardResult, multiply, point);
            cardSlotController.BeginOpenTimeOutMyCard();
        }
        else
        {
            cardSlotController.SetCardOnHand(53);
            cardSlotController.BiginInitAnotherCardThree();
        }
    }
    public void OpenCard(HandCard[] cards, PokdengCardResult cardResult, int multiply, int point)
    {
        if (!m_isMyPlayerSlot)
        {
            cardSlotController.OpenAnotherCardWithResult(cards, cardResult, multiply, point);
        }
    }
    public void OnSetActive()
    {
        currentActive.SetActive(true);
    }

    public void DeActive()
    {
        currentActive.SetActive(false);
    }

    public void OnSetCloset(bool isSet)
    {
        if (closetKing != null)
        {
            closetKing.SetActive(isSet);
            closetCrown.SetActive(isSet);
        }
    }

    public void OnSetJuaSim()
    {
        juaSim.SetActive(true);
    }

    public void OnSetNotJuaSim()
    {
        notJuaSim.SetActive(true);
    }

    public void OnSetJub()
    {
        jub.SetActive(true);
    }
    public void Clear()
    {
        if (juaSim != null)
        {
            juaSim.SetActive(false);
            notJuaSim.SetActive(false);
            jub.SetActive(false);
        }
        cardSlotController.ClearCard();

        if (closetKing != null)
        {
            closetKing.SetActive(false);
            closetCrown.SetActive(false);
        }
        currentActive.SetActive(false);
    }
    public void RemovePlayer()
    {
        m_isHavePlayerSit = false;
        this.playerName.gameObject.SetActive(false);
        profile.gameObject.SetActive(false);
        goldOj.SetActive(false);
    }
}
