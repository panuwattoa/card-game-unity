using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardJakScript : MonoBehaviour
{
    [SerializeField] private GameObject[] position1;
    [SerializeField] private GameObject[] position2;
    [SerializeField] private GameObject[] position3;
    [SerializeField] private GameObject[] position4;
    [SerializeField] private GameObject[] position5;
    [SerializeField] private GameObject[] position6;
    [SerializeField] private GameObject[] position7;

    public void OnSetCardJak(PlayerSlot[] seatPosition)
    {
        foreach (var c in position1)
        {
            c.SetActive(false);
        }
        foreach (var c in position2)
        {
            c.SetActive(false);
        }
        foreach (var c in position3)
        {
            c.SetActive(false);
        }
        foreach (var c in position4)
        {
            c.SetActive(false);
        }

        foreach (var c in position5)
        {
            c.SetActive(false);
        }
        foreach (var c in position6)
        {
            c.SetActive(false);
        }
        foreach (var c in position7)
        {
            c.SetActive(false);
        }
        for (int i = 0; i < seatPosition.Length; i++)
        {
            if (seatPosition[i].m_isHavePlayerSit)
            {
                if (i+1 == 1)
                {

                    foreach (var c in position1)
                    {
                        c.SetActive(true);
                    }
                }
                if (i + 1 == 2)
                {
                    foreach (var c in position2)
                    {
                        c.SetActive(true);
                    }
                }

                if (i + 1 == 3)
                {
                    foreach (var c in position3)
                    {
                        c.SetActive(true);
                    }
                }
                if (i + 1 == 4)
                {
                    foreach (var c in position4)
                    {
                        c.SetActive(true);
                    }
                }
                if (i + 1 == 5)
                {
                    foreach (var c in position5)
                    {
                        c.SetActive(true);
                    }
                }
                if (i + 1 == 6)
                {
                    foreach (var c in position6)
                    {
                        c.SetActive(true);
                    }
                }
                if (i + 1 == 7)
                {
                    foreach (var c in position7)
                    {
                        c.SetActive(true);
                    }
                }

            }
        }
       
    }
}
