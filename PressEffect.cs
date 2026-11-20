using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Color pressedColor = new Color(0f, 0f, 0f, 0.35f);
    private Image overlay;
    private Color clear = new Color(0f, 0f, 0f, 0f);

    void Awake()
    {
        Image baseImage = GetComponent<Image>();

        GameObject go = new GameObject("PressOverlay", typeof(RectTransform), typeof(Image));
        go.transform.SetParent(transform, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        overlay = go.GetComponent<Image>();
        overlay.raycastTarget = false;
        if (baseImage != null)
        {
            overlay.sprite = baseImage.sprite;
            overlay.type = baseImage.type;
            overlay.preserveAspect = baseImage.preserveAspect;
        }
        overlay.color = clear;
    }

    public void OnPointerDown(PointerEventData e) { overlay.color = pressedColor; }
    public void OnPointerUp(PointerEventData e) { overlay.color = clear; }
    public void OnPointerExit(PointerEventData e) { overlay.color = clear; }
}