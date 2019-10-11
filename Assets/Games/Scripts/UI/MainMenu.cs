using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    Button btnShop;

    [SerializeField]
    Button btnBoost;

    [SerializeField]
    Button btnUpgrade;


    private void Awake()
    {
        btnShop.onClick.AddListener(() =>
        {
            DialogManager.Instance.ShowDialog(DialogConfig.ShopDialog);
            GameController.Instance.SetLockCamera(true);
        });
    }
}
