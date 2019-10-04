using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyGamesUtils
{
    public static class Utils
    {
        public static Vector2 WorldToCanvas(Vector3 worldPos,Canvas canvas)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            Vector2 uiOffset = new Vector2((float)canvasRect.sizeDelta.x / 2f, (float)canvasRect.sizeDelta.y / 2f);

            // Get the position on the canvas
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(worldPos);
            Vector2 proportionalPosition = new Vector2(ViewportPosition.x * canvasRect.sizeDelta.x, ViewportPosition.y * canvasRect.sizeDelta.y);

           return (proportionalPosition - uiOffset);
        }
    }

}
