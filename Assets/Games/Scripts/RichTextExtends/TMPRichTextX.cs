using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

[RequireComponent(typeof(TextMeshProUGUI))]
[ExecuteInEditMode]
public class TMPRichTextX : MonoBehaviour {

    private TextMeshProUGUI m_textMeshProUGUIComp = null;
    public TextMeshProUGUI TextMeshProUGUIComp
    {
        get
        {
            if (m_textMeshProUGUIComp == null)
            {
                m_textMeshProUGUIComp = this.GetComponent<TextMeshProUGUI>();
            }
            return m_textMeshProUGUIComp;
        }
    }

    public TMP_SpriteAsset SpriteAsset
    {
        get
        {
            return spriteAsset;
        }

        set
        {
            spriteAsset = value;
            TextMeshProUGUIComp.spriteAsset = spriteAsset;
        }
    }

    [SerializeField]
    TMP_SpriteAsset spriteAsset;

    [SerializeField]
    string text="";

    string orginalText = "";

    private void Awake()
    {
        
    }

    public void DoRenderRichText()
    {
        StringBuilder sb = new StringBuilder(text);
        if (spriteAsset == null)
        {
            TextMeshProUGUIComp.text = sb.ToString();
            return;
        }
          
        string orginalText = text;
      
        if (text.Contains(@"<sprite>") && text.Contains(@"</sprite>"))
        {
            string currentText = "";
            do
            {
                currentText = sb.ToString();
            
                int beginBracket = currentText.IndexOf(@"<sprite>", 0);
                int endBracket = currentText.IndexOf(@"</sprite>", 0);

                bool notFoundTag = beginBracket == -1 || endBracket == -1;
                if (notFoundTag == false)
                {
                    string frameName = currentText.Substring(beginBracket + 8, endBracket - beginBracket - 8);
                    int index = GetFrameIndex(frameName);
                    ////hide squiggle tags
                    sb.Remove(beginBracket, endBracket - beginBracket + 9);
                    sb.Insert(beginBracket, "<sprite=" + index.ToString() + ">");
                }
                else
                {
                    break;
                }

            } while (true);
            
        }
        TextMeshProUGUIComp.text = sb.ToString();
    }


    public void SetText(string content)
    {
        text = content;
        DoRenderRichText();
    }
  

    void OnValidate()
    {
        TextMeshProUGUIComp.spriteAsset=spriteAsset;
    }


    public int GetFrameIndex(string frameName)
    {
        return spriteAsset.spriteCharacterTable.FindIndex(x => x.name == frameName);
    }
}
