using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ছবিতে সবুজ ON টগল আঁকা আছে। OFF করলে তার ওপর ঘরের রং + OFF লেখা + বাঁ দিকে নব বসে যায়।
// ON করলে ওটা সরে যায়, নিচের আসল ছবি দেখা যায়।
public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    public Color offFill = new Color32(24, 72, 178, 255);   // ঘরের রং
    public float inset = 0.07f;                             // সোনালি পাড় বাঁচাতে ভেতরে কতটা ঢুকবে

    public event Action<bool> Changed;
    public bool IsOn { get; private set; } = true;

    GameObject offLook;

    void Build()
    {
        RectTransform rt = (RectTransform)transform;

        Image hit = GetComponent<Image>();
        if (hit == null) hit = gameObject.AddComponent<Image>();
        hit.color = new Color(0f, 0f, 0f, 0f);
        hit.raycastTarget = true;

        float w = Mathf.Max(20f, rt.rect.width);
        float h = Mathf.Max(10f, rt.rect.height);
        float pad = h * inset;
        float innerW = Mathf.Max(10f, w - 2f * pad);
        float innerH = Mathf.Max(10f, h - 2f * pad);

        RectTransform look = UIShapes.Stretch(transform, "OffLook", pad);
        offLook = look.gameObject;

        Sprite pill = UIShapes.Pill(256, Mathf.RoundToInt(256f * innerH / innerW));
        UIShapes.NewImage(look, "Track", pill, offFill);

        float k = innerH * 0.84f;
        Sprite knobSprite = UIShapes.Knob(64, new Color32(110, 165, 235, 255), new Color32(240, 248, 255, 255));
        RectTransform knob = UIShapes.NewImage(look, "Knob", knobSprite, Color.white).rectTransform;
        knob.anchorMin = new Vector2(0f, 0.5f);
        knob.anchorMax = new Vector2(0f, 0.5f);
        knob.pivot = new Vector2(0f, 0.5f);
        knob.sizeDelta = new Vector2(k, k);
        knob.anchoredPosition = new Vector2(innerH * 0.08f, 0f);

        RectTransform label = UIShapes.NewText(look, "OffText", "OFF", Color.white).rectTransform;
        label.anchorMin = new Vector2(0.40f, 0.12f);
        label.anchorMax = new Vector2(0.94f, 0.88f);
        label.offsetMin = Vector2.zero;
        label.offsetMax = Vector2.zero;

        offLook.SetActive(!IsOn);
    }

    public void Set(bool on, bool notify)
    {
        if (offLook == null) Build();
        IsOn = on;
        offLook.SetActive(!on);
        if (notify && Changed != null) Changed(on);
    }

    public void OnPointerClick(PointerEventData e)
    {
        Set(!IsOn, true);
        AudioManager.PlayClick();
    }
}
