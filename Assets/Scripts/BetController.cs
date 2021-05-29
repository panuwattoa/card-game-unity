using System.Collections;
using System.Collections.Generic;
using Scripts.Session;
using TMPro;
using UnityEngine;
using System;
public class BetController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI m_textBubbleChip;


	[SerializeField] private GameObject[] m_chipObject;
	 private int UserCurrentGold;

	[SerializeField] private TextMeshProUGUI maximumText;

	private int m_chipBet =20 ;
	private int m_maximumBet = 200;
	private Action<int>  m_callBack;
	public void ShowBetConsole(int minimumBet, int maximumBet, Action<int> callBack)
	{
		m_callBack = callBack;
		m_chipBet = minimumBet;
		m_maximumBet = maximumBet;
		OnChangeBetValue(0);
	}

    private void OnEnable()
    {
		//NakamaSessionManager.Instance.SyncAccount();
		//UserCurrentGold = PlayerWallet.GetWallet(NakamaSessionManager.Instance.Account.Wallet).gold;
        OnChangeBetValue(0);
		maximumText.text = m_maximumBet.ToString();
	}
	private void UpdateTextBubbleChip()
	{
		m_textBubbleChip.text = m_chipBet.ToString();
	}

	public void OnClickIncreaseBet()
	{
		m_chipBet += m_maximumBet / 20;
		if (m_chipBet > m_maximumBet)
		{
			m_chipBet = m_maximumBet;
		}

		long chip = m_chipBet * 20 / m_maximumBet;
		OnChangeBetValue(chip / (float)m_chipObject.Length);
	}

	public void OnClickDecreaseBet()
	{
		m_chipBet -= m_maximumBet / 20;
		long chip = m_chipBet * 20 / m_maximumBet;
		OnChangeBetValue(chip / (float)m_chipObject.Length);
	}

	private void UpdateChipTray(int chip)
	{

		for (int i = 0; i < m_chipObject.Length; i++)
		{
			if (i < chip)
			{
				if (!m_chipObject[i].activeSelf)
				{
					m_chipObject[i].SetActive(true);

				}

			}
			else
			{
				if (m_chipObject[i].activeSelf)
				{
					m_chipObject[i].SetActive(false);
				}
			}
		}

		m_chipBet = m_maximumBet / 20 * chip;

        var gold = PlayerWallet.GetWallet(NakamaSessionManager.Instance.Account.Wallet).gold;

        if (m_chipBet > gold)
        {
            m_chipBet = gold;
        }
		EffectManager.Instance.PlaySoundByName("mixkit-money-bag-drop-1989");
		UpdateTextBubbleChip();

	}
	public void OnChangeBetValue(float value)
	{
		Debug.Log("value " + value);
		int chip = Mathf.RoundToInt(value * (float)m_chipObject.Length);
		Debug.Log("chip " + chip);
		if (chip < 2)
			chip = 2;
		UpdateChipTray(chip);
	}


	public void OnClickBet()
    {
		m_callBack?.Invoke(m_chipBet);
		gameObject.SetActive(false);
	}

}
