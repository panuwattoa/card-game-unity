using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScore : MonoBehaviour
{
#pragma warning disable CS0649
    [SerializeField] private GameObject m_twoDeng;
    [SerializeField] private GameObject m_threeDeng;
    [SerializeField] private GameObject m_fiveDeng;
#pragma warning restore CS0649

    public void SetDeng(int deng)
	{
        this.gameObject.SetActive(true);

        m_twoDeng?.SetActive(false);
        m_threeDeng?.SetActive(false);
        m_fiveDeng?.SetActive(false);


        if (deng == 2)
		{
            m_twoDeng?.SetActive(true);
		}
        else if(deng == 3)
		{
            m_threeDeng?.SetActive(true);
		}
        else if(deng == 5)
		{
            m_fiveDeng?.SetActive(true);
		}
	}
}
