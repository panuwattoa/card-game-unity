using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogRule : MonoBehaviour
{
    [SerializeField] private GameObject main;
    [SerializeField] private GameObject rule;
    [SerializeField] private GameObject result;
    [SerializeField] private GameObject point;
    [SerializeField] private GameObject pay;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnCloseMain()
    {
        gameObject.SetActive(false);
    }

    public void OnCloseRule()
    {
        rule.SetActive(false);

    }

    public void OnCloseResult()
    {
        result.SetActive(false);
    }

    public void OnClosePoint()
    {
        point.SetActive(false);
    }

    public void OnClosePay()
    {
        pay.SetActive(false);
    }


    public void OnOpenRule()
    {
        rule.SetActive(true);
    }

    public void OnOpenResult()
    {
        result.SetActive(true);
    }

    public void OnOpenPoint()
    {
        point.SetActive(true);
    }

    public void OnOpenPay()
    {
        pay.SetActive(true);
    }
}
