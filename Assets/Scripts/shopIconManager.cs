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
    private bool isOut;
    // Start is called before the first frame update
    private void OnEnable()
    {
        productPrice.text = IAPManager.Instance.GetProductPriceFromStore(productID);
    }
    void Start()
    {
       var product = IAPManager.Instance.IapProduct.Find(x => x.productName.Equals(productID));
        if (product != null)
        {
            productNameText.text = product.productText;
            if (isSpecial)
            {
                CheckSpecial();
            }
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
            isOut = true;
        }
    }

    public void OnClickBuy()
    {
        if (!isOut)
        {
            IAPManager.Instance.BuyProductID(productID, OnBuySuccess);
        }
    }


    private void OnBuySuccess()
    {
        if (isSpecial)
        {
           _ = GameApi.BuySpecail();
        }
    }
}
