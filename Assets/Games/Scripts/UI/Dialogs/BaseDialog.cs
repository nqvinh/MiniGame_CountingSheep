using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDialog : MonoBehaviour
{
    CanvasGroup canvasGroup = null;

    public CanvasGroup CanvasGroup {
        get
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            return canvasGroup;
        }
    }

    public virtual void SetUp(DialogParams param)
    {
        
    }

    public virtual void DoShow()
    {

    }

    public virtual void DoHide()
    {

    }

    protected virtual void OnBeginShow()
    {

    }

    protected virtual void OnEndShow()
    {

    }

    protected virtual void OnBeginHide()
    {

    }

    protected virtual void OnEndHide()
    {

    }

}


public class DialogParams
{

}