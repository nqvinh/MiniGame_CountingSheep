using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopDialog : BaseDialog
{
    [SerializeField]
    Button btnClose;

    [SerializeField]
    ShopItemController shopItem;

    [SerializeField]
    Transform shopContainer;

    bool hasInitShop = false;
    List<int> hasInitButton = new List<int>();

    private void Awake()
    {
        btnClose.onClick.AddListener(() =>
        {
            DialogManager.Instance.HideDialog(this);
        });
    }

    public override void SetUp(DialogParams param)
    {
        base.SetUp(param);

        for (int i = 1; i <= GameController.numOfSheepType; ++i)
        {
            if (!hasInitButton.Contains(i))
            {
                SheepConfigData sheepConfigData = ConfigManager.Instance.SheepConfigProp.dataArray[i - 1];
                if (sheepConfigData.Unlocklevel <= DataManager.Instance.PlayerData.userLevel)
                {
                    ShopItemController item = Instantiate(shopItem, shopContainer);
                    item.Setup(i.ToString(), SimpleResourcesManager.Instance.GetSprite("Sheep" + i), DataManager.Instance.PlayerData.sheepPriceCurrent[i-1], OnBuySheep);
                    hasInitButton.Add(i);
                }
            }   
        }

        //if (hasInitShop == false)
        //{
        //    hasInitShop = true;
        //    for (int i=1;i<=GameController.numOfSheepType;++i)
        //    {
        //        SheepConfigData sheepConfigData = ConfigManager.Instance.SheepConfigProp.dataArray[i - 1];
        //        if (sheepConfigData.Unlocklevel <= DataManager.Instance.PlayerData.userLevel)
        //        {
        //            ShopItemController item = Instantiate(shopItem, shopContainer);
        //            item.Setup(i.ToString(), SimpleResourcesManager.Instance.GetSprite("Sheep" + i), sheepConfigData.Price, OnBuySheep);
        //        }
        //    }
            
        //}
    }


    void OnBuySheep(string id)
    {
        int sheepType = int.Parse(id);
        SheepController sheepController = SheepFactory.Instance.CreateNewSheep(sheepType);
        GameController.Instance.PutSheepBackToCage(sheepController);
        SimpleResourcesManager.Instance.ShowParticle("MergeFx", sheepController.transform.position, 1);
    }

    public override void DoHide()
    {
        GameController.Instance.SetLockCamera(false);
        OnBeginHide();
        CanvasGroup.DOFade(0, 0.5f).onComplete = () =>
        {
            OnEndHide();
            gameObject.SetActive(false);
        };
    }

    public override void DoShow()
    {
        gameObject.SetActive(true);
        OnBeginShow();
        CanvasGroup.DOFade(0, 0);
        CanvasGroup.DOFade(1, 0.5f).onComplete=()=>
        {
            OnEndShow();
        };
    }

   
    public override string ToString()
    {
        return base.ToString();
    }

    protected override void OnBeginHide()
    {
        base.OnBeginHide();
    }

    protected override void OnBeginShow()
    {
        base.OnBeginShow();
    }

    protected override void OnEndHide()
    {
        base.OnEndHide();
    }

    protected override void OnEndShow()
    {
        base.OnEndShow();
    }
}
