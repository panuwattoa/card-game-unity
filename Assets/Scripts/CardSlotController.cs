using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSlotController : MonoBehaviour
{
    [SerializeField] private Image[] card;

    private int m_pokdengCardMultiply;
    private int m_pokdengCardPoint;
    private PokdengCardResult m_pokdengCardResult;
    [SerializeField] private CardScoreController cardScoreController;

    private List<int> m_cards;

    public void SetCardOnHand(int cardNum)
    {
        if (m_cards == null)
            m_cards = new List<int>();

        m_cards.Add(cardNum);
    }

    public void BeginOpenMyCard()
    {
        cardScoreController.HideCardScore();
        if(m_cards.Count == 2)
        {
            CardHandController.CreateHandCard(m_cards[0], m_cards[1],0, false,OpenCardTableCallBack);
        }else if (m_cards.Count == 3)
        {
            CardHandController.CreateHandCard(m_cards[0], m_cards[1], m_cards[2], true, OpenCardTableCallBack);

        }
        else
        {
            OpenCardTableCallBack();
        }
    }


    public void BeginOpenTimeOutMyCard()
    {
        cardScoreController.HideCardScore();
        OpenCardTableCallBack();
      
    }


    private void OpenCardTableCallBack()
    {
        for (int i = 0; i < m_cards.Count; i++)
        {
            card[i].gameObject.SetActive(true);
            card[i].sprite = Helper.getCardSpriteByValue(m_cards[i]);
        }
        OpenCardFinish();
    }

    public void SetCardResultAndMultiply(PokdengCardResult cardResult, int multiply, int point)
    {
        m_pokdengCardResult = cardResult;
        m_pokdengCardMultiply = multiply;
        m_pokdengCardPoint = point;
    }

    public void OpenCardFinish()
    {
        cardScoreController.SetCardScore(m_pokdengCardResult, m_pokdengCardMultiply, m_pokdengCardPoint);

    }


    public void BiginInitAnotherCard()
    {
        cardScoreController.HideCardScore();
        for (int i = 0; i < m_cards.Count; i++)
        {
            card[i].gameObject.SetActive(true);
            card[i].sprite = Helper.getBackCard();
        }
    }

    public void BiginInitAnotherCardThree()
    {
        cardScoreController.HideCardScore();
        if (card.Length > 2)
        {
            card[2].gameObject.SetActive(true);
            card[2].sprite = Helper.getBackCard();
        }
    }

    public void OpenAnotherCardWithResult(HandCard[] openCard, PokdengCardResult cardResult, int multiply, int point)
    {
        m_cards?.Clear();
        m_cards = new List<int>();
        foreach (var c in openCard)
        {
            m_cards.Add(c.CardID);
        }
        cardScoreController.SetCardScore(m_pokdengCardResult, m_pokdengCardMultiply, m_pokdengCardPoint);
        m_pokdengCardResult = cardResult;
        m_pokdengCardMultiply = multiply;
        m_pokdengCardPoint = point;
        OpenCardTableCallBack();
    }


    public void ClearCard()
    {
        for (int i = 0; i < card.Length; i++)
        {
            card[i].gameObject.SetActive(false);
            card[i].sprite = Helper.getBackCard();
        }
        cardScoreController.HideCardScore();
        m_cards?.Clear();
    }


}
