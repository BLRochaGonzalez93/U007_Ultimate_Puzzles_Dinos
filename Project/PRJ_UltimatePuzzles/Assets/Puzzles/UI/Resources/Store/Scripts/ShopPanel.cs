using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    [Header("Coin Text")]
    [SerializeField] private TMP_Text coinText;

    [Header("Remove Ads")]
    [SerializeField] private GameObject adsBuyButton;

    [Header("Buy Button Text")]
    [SerializeField] private TMP_Text coin500PriceText;
    [SerializeField] private TMP_Text removeAdsPriceText;

    public void UpdateButtonPrice(string productId, string price)
    {
        if (productId == IAPManager.Instance.coin500)
        {
            coin500PriceText.text = price;
        }
        else if (productId == IAPManager.Instance.remove_ads)
        {
            removeAdsPriceText.text = price;
        }
    }

    public void Coin500Button()
    {
        IAPManager.Instance.BuyProduct(IAPProductKey.Coin500);
        // int coinAmount = 500;
        // AddRewardedCoin(coinAmount);
    }

    public void RemoveAdsButton()
    {
        IAPManager.Instance.BuyProduct(IAPProductKey.RemoveAds);
        // RemoveAdsReward();
    }

    public void AddRewardedCoin(int coinAmount)
    {
        int currentValue;
        if (int.TryParse(coinText.text, out currentValue))
        {
            currentValue += coinAmount;
            coinText.text = currentValue.ToString();
            PlayerPrefs.SetString("coins", coinText.text);
        }
    }

    public void RemoveAdsReward()
    {
        Destroy(adsBuyButton);

        //Destroy Ads Here
        AdsManager.Instance.StopShowBannerAd();
    }
}
