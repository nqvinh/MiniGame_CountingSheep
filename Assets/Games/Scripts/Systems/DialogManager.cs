using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DialogConfig
{
    ShopDialog,
    COUNT
}

public class DialogManager : Singleton<DialogManager>
{
    Dictionary<DialogConfig, BaseDialog> dialogMap = new Dictionary<DialogConfig, BaseDialog>();
    List<BaseDialog> visibleDialog = new List<BaseDialog>();

    public void LoadDialog()
    {
        int dialogCount = (int)DialogConfig.COUNT;
        for (int i=0;i<dialogCount;++i)
        {
            DialogConfig dialogConfig = (DialogConfig)i;
            BaseDialog dialog = Resources.Load<BaseDialog>("Dialogs/" + dialogConfig.ToString());
            BaseDialog dialogInstance = Instantiate(dialog, this.transform);
            dialogInstance.gameObject.SetActive(false);
            dialogMap.Add(dialogConfig, dialogInstance);
            //Resources.UnloadAsset(dialog);
        }
    }

    public void ShowDialog(DialogConfig dialogConfig,DialogParams dialogParams = null)
    {
        BaseDialog dialog = dialogMap[dialogConfig];
        if (visibleDialog.Contains(dialog))
            return;
       
        visibleDialog.Add(dialog);
        dialog.SetUp(dialogParams);
        dialog.DoShow();
    }

    public void HideDialog(DialogConfig dialogConfig)
    {
        BaseDialog dialog = dialogMap[dialogConfig];
        if (!visibleDialog.Contains(dialog))
            return;
        dialog.DoHide();
    }

    public void HideDialog(BaseDialog dialog)
    {
        dialog.DoHide();
        visibleDialog.Remove(dialog);
    }


}
