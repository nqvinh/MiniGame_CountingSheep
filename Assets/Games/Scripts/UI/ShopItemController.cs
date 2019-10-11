using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemController : MonoBehaviour
{
    [SerializeField]
    Button btnBuy;
    [SerializeField]
    Image icon;
    [SerializeField]
    TextMeshProUGUI txtPrice;

    string mId=string.Empty;
    int index = 0;
    System.Action<string> onBuyItem = null;
    double itemPrice;
    float priceIncreasePercent = 1.0f;

    private void Awake()
    {
        btnBuy.onClick.AddListener(() =>
        {
            if (DataManager.Instance.PlayerData.userSheepCoin < itemPrice ||
                GameController.Instance.IsCageFull())
                return;
            DataManager.Instance.PlayerData.userSheepCoin -= itemPrice;
            GameController.Instance.UpdateSheepCoin();
            if (onBuyItem != null)
            {
                onBuyItem(mId);
                itemPrice += itemPrice * priceIncreasePercent;
                DataManager.Instance.PlayerData.sheepPriceCurrent[index-1]=itemPrice;
                txtPrice.text = BigNumbers.BigNumber.ToShortString(itemPrice.ToString("F0"), 4);
            } 
        });
    }

    public void Setup(string id,Sprite iconSprite, double price,System.Action<string> buyCallBack,bool setNativeSize = false)
    {
        mId = id;
        index = int.Parse(id);


        icon.sprite = iconSprite;
        txtPrice.text = BigNumbers.BigNumber.ToShortString(price.ToString("F0"),4);
        onBuyItem = buyCallBack;
        itemPrice = price;
        priceIncreasePercent = ConfigManager.Instance.GetSheepConfigByType(index).Pricebonus;
    }


}
