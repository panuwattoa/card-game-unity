using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class shopIconManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI productNameText;
    [SerializeField] private TextMeshProUGUI productPrice;
    [SerializeField] private string productID;
    [SerializeField] private bool isSpecial;
    [SerializeField] private GameObject outOfStock;
    // Start is called before the first frame update
    private void Awake()
    {
        productPrice.text = IAPManager.instance.GetProductPriceFromStore(productID);
    }
    void Start()
    {
       var product = IAPManager.instance.IapProduct.Find(x => x.productName.Equals(productID));
        productNameText.text = product.productText;
        if(isSpecial)
        {
            CheckSpecial();
        }
    }

    private async void CheckSpecial()
    {
      string resp = await GameApi.CheckSepcialIAP();
        if (resp.Equals("true"))
        {
            outOfStock.SetActive(false);
        }
        else
        {
            outOfStock.SetActive(true);
        }
    }

    public void OnClickBuy()
    {
        IAPManager.instance.BuyProductID(productID, OnBuySuccess);
    }


    private void OnBuySuccess()
    {
        if (isSpecial)
        {
           _ = GameApi.BuySpecail();
        }
    }
}
