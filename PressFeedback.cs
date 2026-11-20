using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PressFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Color pressedColor = new Color(0f, 0f, 0f, 0.3f);
    private Image img;
    private Color hidden = new Color(1f, 1f, 1f, 0f);

    void Awake()
    {
        img = GetComponent<Image>();
        img.color = hidden;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (img != null) img.color = pressedColor;
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (img != null) img.color = hidden;
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (img != null) img.color = hidden;
    }
}