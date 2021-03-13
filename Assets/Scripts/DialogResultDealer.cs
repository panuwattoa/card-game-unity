using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogResultDealer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalChip;
    [SerializeField] private TextMeshProUGUI winChip;
    [SerializeField] private TextMeshProUGUI lostChip;

    [SerializeField] private TextMeshProUGUI winnerName;
    [SerializeField] private TextMeshProUGUI loserName;

    public void SetData(int total, int win , int lost, string[] winName,string[] lostName)
    {
        winnerName.text = "";
        loserName.text = "";
        totalChip.text = total.ToString();
        winChip.text = win.ToString();
        lostChip.text = lost.ToString();
        if (winName != null)
        {
            foreach (string item in winName)
            {
                if (item != null)
                {
                    winnerName.text += item + "\n";

                }
            }


        }

        if (lostName != null)
        {
            foreach (string item in lostName)
            {
                if (item != null)
                {
                    loserName.text += item + "\n";
                }
            }
        }
    }
}
