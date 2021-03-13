using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper 
{
	public static Sprite getCardSpriteByValue(int cardValue)
	{
		// The media class look like "1_diamond".
		int num = getNumFromValue(cardValue);
		int suit = getSuitFromValue(cardValue);
		string sSuit = "";
		switch (suit)
		{
			case 1:
				sSuit = "club";
				break;
			case 2:
				sSuit = "diamond";
				break;
			case 3:
				sSuit = "heart";
				break;
			case 4:
				sSuit = "spade";
				break;
		}
		//Debug.Log("carddd " + "card/" + sSuit + "_" + num);
		Sprite image = Resources.Load<Sprite>("card/"+sSuit + "_" + num );
		if (image == null) Debug.LogWarning("null");
		return image;
	}

	public static Sprite getBackCard()
    {
		return  Resources.Load<Sprite>("card/card_b");

	}

	public static int getSuitFromValue(int cardValue)
	{
		return cardValue - (getNumFromValue(cardValue) - 1) * 4;
	}

	public static int getNumFromValue(int cardValue)
	{
		double ret = Math.Ceiling(Convert.ToDouble(cardValue) / 4);
		//Debug.LogWarning(" cardValue " + cardValue + " ret " + ret + " Convert.ToInt16(ret) " + Convert.ToInt16(ret));

		return Convert.ToInt16(ret);
	}


}
