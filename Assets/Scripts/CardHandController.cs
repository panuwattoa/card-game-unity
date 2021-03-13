using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardHandController : MonoBehaviour
{
    [SerializeField] private Image[] card;
	[SerializeField] private Animator cardd;
	// Start is called before the first frame update
	private int card1;
	private int card2;
	private int card3;
	private Action m_callBack;
	bool isOpenCard3;
	public static CardHandController CreateHandCard(int card1, int card2,int card3, bool _isOpenCard3, Action callBack)
	{
		GameObject prefab = Resources.Load<GameObject>("Prefabs/hand-card3");
		GameObject o = Instantiate(prefab);
		CardHandController hand = o.GetComponentInChildren<CardHandController>();
		hand.card1 = card1;
		hand.card2 = card2;
		hand.card3 = card3;
		hand.isOpenCard3 = _isOpenCard3;
		hand.m_callBack = callBack;
		hand.cardd.ResetTrigger("is2Card");
		hand.cardd.ResetTrigger("is3Card");

		return hand;
	}

    private void Start()
    {
		var cardImg1 = Helper.getCardSpriteByValue(card1);
		if (cardImg1 != null)
        {
			card[0].sprite = cardImg1;
		}

		var cardImg2 = Helper.getCardSpriteByValue(card2);
		if (cardImg2 != null)
		{
			card[1].sprite = cardImg2;
		}
		var cardImg3 = Helper.getCardSpriteByValue(card3);
		if (cardImg3!= null)
		{
			card[2].sprite = cardImg3;
		}

		if (isOpenCard3)
		{
			cardd.SetTrigger("is3Card");
        }
        else
        {
			cardd.SetTrigger("is2Card");
		}
	}

	public void AnimationDone()
    {
		StartCoroutine(WaitClose());
	}

	IEnumerator WaitClose()
    {
		yield return new WaitForSeconds(1.2f);
        try
        {
			if (m_callBack != null)
			{
				m_callBack?.Invoke();
			}
			Destroy(gameObject.transform.parent.gameObject);
		}
        catch 
        {
			Destroy(gameObject.transform.parent.gameObject);

		}

	}
}
