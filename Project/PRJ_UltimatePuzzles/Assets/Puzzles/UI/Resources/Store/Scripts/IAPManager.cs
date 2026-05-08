using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    public static IAPManager Instance { get; private set; }

    public string coin500 = "coin500";
    public string remove_ads = "remove_ads";

    public static bool IsInitialized { get; private set; } = false;

    private static StoreController storeController;
    [SerializeField] private ShopPanel shopPanel;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitIAP();
    }

    private async Task InitIAP()
    {
        try
        {
            var option = new InitializationOptions().SetEnvironmentName("production");
            await UnityServices.InitializeAsync(option);

            storeController = UnityIAPServices.StoreController();

            storeController.OnStoreDisconnected += OnStoreDisconnected;
            storeController.OnProductsFetched += OnProductsFetched;
            storeController.OnProductsFetchFailed += OnProductsFetchFailed;
            storeController.OnPurchasesFetched += OnPurchasesFetched;
            storeController.OnPurchasesFetchFailed += OnPurchasesFetchFailed;
            storeController.OnPurchasePending += OnPurchasePending;
            storeController.OnPurchaseConfirmed += OnPurchaseConfirmed;
            storeController.OnPurchaseFailed += OnPurchaseFailed;
            storeController.OnPurchaseDeferred += OnPurchaseDeferred;

            RegisterEntitlementCallback();

            await storeController.Connect();

            var initialProductToFetch = BuildProductDefinitions();
            storeController.FetchProducts(initialProductToFetch);
        }
        catch (Exception e)
        {
            Debug.Log($"Initialization failed with: {e}");
        }
    }

    private void RegisterEntitlementCallback()
    {
        storeController.OnCheckEntitlement += (result) =>
        {
            Product product = result.Product;
            var status = result.Status;

            Debug.Log($"Product is {product}, Entitle Status is {status}");

            //Only for NoN Consumable product or subscription. Ex: Ads
            bool isEntitled = status == EntitlementStatus.FullyEntitled;
            if (isEntitled)
            {
                if (product.definition.id == remove_ads)
                {
                    shopPanel.RemoveAdsReward(); // You can destroy Ads or destroy inside ShopPanel
                }
            }
        };
    }

    public void BuyProduct(IAPProductKey productKey)
    {
        if (!IsInitialized)
        {
            Debug.Log("IAP Module is not initialized. Try again some time.");
            return;
        }

        if (productKey == IAPProductKey.Coin500)
        {
            storeController.PurchaseProduct(coin500);
        }
        else if (productKey == IAPProductKey.RemoveAds)
        {
            storeController.PurchaseProduct(remove_ads);
        }
    }

    private List<ProductDefinition> BuildProductDefinitions()
    {
        var initialProductToFetch = new List<ProductDefinition>();

        initialProductToFetch.Add(new ProductDefinition(coin500, ProductType.Consumable));
        initialProductToFetch.Add(new ProductDefinition(remove_ads, ProductType.NonConsumable));

        return initialProductToFetch;
    }

    private void OnProductsFetched(List<Product> products)
    {
        // Products are ready. Now Fetch Purchases
        storeController.FetchPurchases();

        foreach (var product in products)
        {
            string price = product.metadata.localizedPrice + " " + product.metadata.isoCurrencyCode;
            // Pass price in ShopPanel
            shopPanel.UpdateButtonPrice(product.definition.id, price);
        }
    }

    private void OnProductsFetchFailed(ProductFetchFailed reason)
    {
        Debug.Log($"Product Fetch Failed: {reason}");
    }

    private void OnPurchasesFetched(Orders orders)
    {
        IsInitialized = true;
        foreach (var product in storeController.GetProducts())
        {
            storeController.CheckEntitlement(product);
        }
    }

    private void OnPurchasesFetchFailed(PurchasesFetchFailureDescription reason)
    {
        Debug.Log($"Purchases Fetch Failed: {reason}");
    }

    private void OnStoreDisconnected(StoreConnectionFailureDescription description)
    {
        Debug.Log($"Initialization/Connection Failed: {description.message}");
    }

    private void OnPurchasePending(PendingOrder order)
    {
        Debug.Log($"Pending Order: {order}");
        storeController.ConfirmPurchase(order);
    }

    private void OnPurchaseDeferred(DeferredOrder deferredOrder)
    {
        Debug.Log($"Purchase Deferred for Product: {deferredOrder?.Info}");
    }

    private void OnPurchaseConfirmed(Order order)
    {
        Debug.Log($"Purchase Confirmed: {order}");
        //Reward Here...
        if (order?.Info?.PurchasedProductInfo != null && order.Info.PurchasedProductInfo.Count > 0)
        {
            int quantity = GetPurchaseQuantity(order);
            string productId = order.Info.PurchasedProductInfo[0].productId;
            if (productId == coin500)
            {
                shopPanel.AddRewardedCoin(500 * quantity);
            }
            else if (productId == remove_ads)
            {
                shopPanel.RemoveAdsReward();

                Debug.Log("Boton Cambiado");
            }
        }
    }

    private int GetPurchaseQuantity(Order order)
    {
        int quantity = 1;

        string receipt = order.Info.Receipt;
        if (!string.IsNullOrEmpty(receipt))
        {
            var payData = JsonUtility.FromJson<IAPPayData>(receipt);
            if (payData.Store != "fake")
            {
                IAPPayload payload = JsonUtility.FromJson<IAPPayload>(payData.Payload);
                IAPPayloadData payloadData = JsonUtility.FromJson<IAPPayloadData>(payload.json);
                quantity = payloadData.quantity;
            }
        }
        return quantity;
    }

    private void OnPurchaseFailed(FailedOrder failedOrder)
    {
        if (failedOrder?.Info?.PurchasedProductInfo == null || failedOrder.Info.PurchasedProductInfo.Count == 0)
        {
            Debug.Log($"Purchase Failed but no product info available");
            return;
        }
        var productId = failedOrder.Info.PurchasedProductInfo[0].productId;
        var reason = failedOrder.FailureReason;
        var message = failedOrder.Details;

        Debug.Log($"Purchase failed. Product is {productId}. Reason is {reason}. Here is the message: {message}");
    }

    // This method is only for IOS. In Android the restoration is Automatic
    public void RestoreAllPurchases()
    {
        storeController.RestoreTransactions((success, error) =>
        {
            if (success)
            {
                Debug.Log("All Previous purchase Restored");
            }
            else
            {
                Debug.LogWarning($"Restore Failed: " + error);
            }
        });
    }

}
