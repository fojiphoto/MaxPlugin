using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;


public enum InAppMode
{ 
    CONSUMEABLE,
    NONCONSUMEABLE,
    ERROR
}


public enum InAppType
{
    Coins,
    HealthPotions,
    EnergyPotions,
    Kunais,
    NoAds
}

[System.Serializable]
public class InAppID
{
    public string anrdoidID;
    public string appleID;
}


[System.Serializable]
public class InAppObject
{
    [SerializeField]
    private InAppID inappID;

    public string InAppID
    {
        get
        {
            string inAppIDToReturn;

#if UNITY_ANDROID
            inAppIDToReturn = this.inappID.anrdoidID;
#endif

#if UNITY_IOS
            inAppIDToReturn = this.inappID.appleID;
#endif

            return inAppIDToReturn;
        }
    }
    

    public InAppType inappType;
    public InAppMode inappMode;

    public int quantityToGive;
}


public class InAppManager : MonoBehaviour,IStoreListener
{
    public List<InAppObject> consumeAbleInApps;
    public List<InAppObject> nonConsumeAbleInApps;

    private static IStoreController storeController;
    private static IExtensionProvider extensionProvider;

    private static string consumeAbleIdentifier = "consumable";
    private static string nonConsumeAbleIdentifier = "nonconsumable";

    void Start()
    {
        if(storeController == null)
        {
            this.InitializePurchase();
        }
    }

    public void InitializePurchase()
    {
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        for (int i = 0; i < this.consumeAbleInApps.Count; i++)
        {
            builder.AddProduct(this.consumeAbleInApps[i].InAppID, ProductType.Consumable);
        }

        for (int i = 0; i < this.nonConsumeAbleInApps.Count; i++)
        {
            builder.AddProduct(this.nonConsumeAbleInApps[i].InAppID, ProductType.NonConsumable);
        }

        UnityPurchasing.Initialize(this, builder);

    }  

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
        extensionProvider = extensions;
    }

    private bool IsInAppInitialized => storeController != null && extensionProvider != null;


    public void BuyProduct(string productID)
    {
        if (this.IsInAppInitialized)
        {
            Product product = storeController.products.WithID(productID);

            if(product!=null && product.availableToPurchase)
            {
                storeController.InitiatePurchase(product);
            }
        }
        //GALogger.LogGAEvent($"Inapp:{productID}:Tap");
    }

    public void RestorePurchases()
    {
        if(this.IsInAppInitialized)
        {
            if(Application.platform== RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
            {
                var appleStore = extensionProvider.GetExtension<IAppleExtensions>();
                appleStore.RestoreTransactions(result => 
                {
                    Debug.LogError($"restore result =>{result}");
                    //ToastManager.Instance.ShowToastMessage("InApps Restored");
                });

            }
        }
    }

    public void CheckInAppType(string ID,out InAppMode  inappMode,out InAppObject inAppObject)
    {
        for(int i=0;i<this.consumeAbleInApps.Count;i++)
        {
            if(this.consumeAbleInApps[i].InAppID == ID)
            {
                inappMode = InAppMode.CONSUMEABLE;
                inAppObject = this.consumeAbleInApps[i];
                return;
            }
        }

        for (int i = 0; i < this.nonConsumeAbleInApps.Count; i++)
        {
            if (this.nonConsumeAbleInApps[i].InAppID == ID)
            {
                inappMode = InAppMode.NONCONSUMEABLE;
                inAppObject = this.nonConsumeAbleInApps[i];
                return;
            }
        }

        inAppObject = null;
        inappMode = InAppMode.ERROR;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseArgs)
    {
        string purchaseID = purchaseArgs.purchasedProduct.definition.id;

        Debug.LogError($"InApp>> {purchaseID}");

        InAppMode inappMode;
        InAppObject inappObject;

        this.CheckInAppType(purchaseID, out inappMode, out inappObject);

        switch(inappMode)
        {
            case InAppMode.CONSUMEABLE:

                Debug.LogError($"InApp>> Consumeable {purchaseID}");
                this.OnConsumeAbleInAppSucceded(inappObject);
                break;

            case InAppMode.NONCONSUMEABLE:
                Debug.LogError($"InApp>> NonConsumeable {purchaseID}");
                this.OnNonConsumeAbleInAppSucceded(inappObject);
                break;
        }

        return PurchaseProcessingResult.Complete;
    }

    public void PurchaseConsumeAble(int index)
    {
        this.BuyProduct(this.consumeAbleInApps[index].InAppID);
    }

    public void PurchaseNonConsumeAble(int index)
    {
        this.BuyProduct(this.nonConsumeAbleInApps[index].InAppID);
    }


    public void OnConsumeAbleInAppSucceded(InAppObject purchaseObject)
    {
        switch (purchaseObject.inappType)
        {
            case InAppType.Coins:
                //GameplayDataManager.Instance.gameplayData.coinsCount += purchaseObject.quantityToGive;
                break;

            case InAppType.EnergyPotions:
                //GameplayDataManager.Instance.EnergyPotionsCount += purchaseObject.quantityToGive;
                break;

            case InAppType.HealthPotions:
                //GameplayDataManager.Instance.HealthPotionsCount += purchaseObject.quantityToGive;
                break;

            case InAppType.Kunais:
                //GameplayDataManager.Instance.KunaisCount += purchaseObject.quantityToGive;
                break;
        }

        //ApplicationManager.Instance.UpdateCurrentHeader();
        //StoreManager.Instance.UpdateHeader();
        //ToastManager.Instance.ShowToastMessage($"{purchaseObject.quantityToGive} {purchaseObject.inappType} Added");
        //GALogger.LogGAEvent($"Inapp:{purchaseObject.InAppID}:Succeed");
    }

    public void OnNonConsumeAbleInAppSucceded(InAppObject purchaseObject)
    {
        switch (purchaseObject.inappType)
        {
            case InAppType.NoAds:
                //GameplayDataManager.Instance.gameplayData.noAdsPurchased = true;
                //if(MainMenuController.instance)
                //MainMenuController.instance.OnNoAdsPurchased();
                //ToastManager.Instance.ShowToastMessage("Purchased No Ads");
                break;
        }
        //ApplicationManager.Instance.UpdateCurrentHeader();

        //GALogger.LogGAEvent($"Inapp:{purchaseObject.InAppID}:Succeed");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {

    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {

    }

}
