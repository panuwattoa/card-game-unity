using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CountdownTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    private int m_time;
    public void SetTime(int time)
    {
        m_time = time;
    }

    private void OnEnable()
    {
        timeText.text = m_time.ToString();
        StartCoroutine("count");
    }

    IEnumerator count()
    {
        while (m_time > 0)
        {
            yield return new WaitForSeconds(1);
            m_time--;
            timeText.text = m_time.ToString();
        }
    }

    private void OnDisable()
    {
        StopCoroutine("count");
    }



}
