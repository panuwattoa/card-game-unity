using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Session;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPManager : Singleton<IAPManager>, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
    public List<IapStruct> IapProduct = new List<IapStruct>();
    private Action m_callBack;

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    private GameObject m_loading;
        public bool initializeSuccess;

    void Start()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public void InitializePurchasing() 
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            
            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            foreach (var item in IapProduct)
            {
                Debug.Log("Init IAp "+ item.productName);
                builder.AddProduct(item.productName, ProductType.Consumable);
            }
            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // // must only be referenced here. 
            // builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            //     { kProductNameAppleSubscription, AppleAppStore.Name },
            //     { kProductNameGooglePlaySubscription, GooglePlay.Name },
            // });

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }


        public bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        


        // public void BuyNonConsumable()
        // {
        //     // Buy the non-consumable product using its general identifier. Expect a response either 
        //     // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //     BuyProductID(kProductIDNonConsumable);
        // }
        //
        //
        // public void BuySubscription()
        // {
        //     // Buy the subscription product using its the general identifier. Expect a response either 
        //     // through ProcessPurchase or OnPurchaseFailed asynchronously.
        //     // Notice how we use the general product identifier in spite of this ID being mapped to
        //     // custom store-specific identifiers above.
        //     BuyProductID(kProductIDSubscription);
        // }


        public void BuyProductID(string productId, Action callBack = null)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                m_callBack = callBack;
                if (m_loading == null)
                {
                    m_loading = popupLoading.Create();
                }
                else
                {
                    m_loading.SetActive(true);
                }
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
             Product product = m_StoreController.products.WithID(productId);
                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                    if (m_loading != null) m_loading.SetActive(false);
                }
            // Otherwise ...
            else
                {
                    if(m_loading!= null) m_loading.SetActive(false);
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer || 
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {

        if (m_loading != null) {
            m_loading.SetActive(false);
        }
        CheckIAP(args);
        m_callBack?.Invoke();
        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed.
        return PurchaseProcessingResult.Complete;
    }

        private async void CheckIAP(PurchaseEventArgs args)
        {
            try
            {

                string validate;
                validate = await GameApi.CheckIAPPayload( args.purchasedProduct.receipt);
                popupMessage.Create("ระบบ", validate);  

             }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                popupMessage.Create("ระบบ", "เกิดข้อผิดพลาด code:297 \n" );
            }

            NakamaSessionManager.Instance.Notify();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            if(m_loading!= null) m_loading.SetActive(false);
            string reason =
                $"PurchaseFailureReason: {failureReason}";
            popupMessage.Create("PurchaseFailed",reason);
            Debug.Log(
                $"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
        }

        public string GetProductPriceFromStore(string id)
        {
            if (m_StoreController?.products != null)
            {
                return m_StoreController.products.WithID(id).metadata.localizedPriceString;
            }
            else
            {
                return "N/A";
            }
        }
}

