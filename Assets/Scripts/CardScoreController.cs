using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PokdengCardResult
{
	NONE = 0,
	POKNINE = 1,
	POKEIGHT = 2,
	TONG = 3,
	STRAIGHT_FLUSH = 4,
	STRAIGHT = 5,
	SIAN = 6,
	POINT = 7,
}
public class CardScoreController : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private CardScore[] cardScores;
#pragma warning restore CS0649

    public void SetCardScore(PokdengCardResult cardResult, int multiply, int point)
	{
		this.gameObject.SetActive(true);
		foreach (var item in cardScores)
		{
			item.gameObject.SetActive(false);
		}
		switch (cardResult)
		{
			case PokdengCardResult.POINT:
				{
					cardScores[point].SetDeng(multiply);
					break;
				}
			case PokdengCardResult.SIAN:
				{
					cardScores[10].SetDeng(multiply);
					break;
				}
			case PokdengCardResult.STRAIGHT:
				{
					cardScores[11].SetDeng(multiply);
					break;
				}
			case PokdengCardResult.TONG:
				{
					cardScores[12].SetDeng(multiply);
					break;
				}
			case PokdengCardResult.POKEIGHT:
				{
					cardScores[13].SetDeng(multiply);
					break;
				}
			case PokdengCardResult.POKNINE:
				{
					cardScores[14].SetDeng(multiply);
					break;
				}
			case PokdengCardResult.STRAIGHT_FLUSH:
				{
					cardScores[15].SetDeng(multiply);
					break;
				}
			default:
				break;
		}
	}

	public void HideCardScore()
	{
		this.gameObject.SetActive(false);

		foreach (var item in cardScores)
		{
			item.gameObject.SetActive(false);
		}
	}
}
