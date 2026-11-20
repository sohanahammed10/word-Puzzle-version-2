using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ছবির আঁকা স্লাইডারের ওপর নিজের স্লাইডার বসায়: টানলে সবুজ/নীল অংশ কমে বাড়ে, নব সরে, % বদলায়।
public class VolumeBar : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Color coverColor = new Color32(24, 72, 178, 255);        // ঘরের রং (আসল স্লাইডার ঢাকতে)
    public Color trackColor = new Color32(10, 32, 96, 255);         // ফাঁকা নালা
    public Color fillColor = new Color32(70, 205, 50, 255);         // ভরা অংশ
    public Color percentBoxColor = new Color32(10, 32, 96, 255);
    public RectTransform percentArea;                               // "70%" লেখার ঘর
    public float trackHeight = 0.55f;                               // নালার উচ্চতা (জায়গার কত ভাগ)
    public float coverGrow = 6f;                                    // ঢাকনা চারদিকে কতটা বড় হবে

    public event Action<float> Changed;
    public event Action Released;

    RectTransform rt;
    Image fill;
    RectTransform knob;
    TMP_Text percentText;
    float value = 0.7f;
    float knobSize;
    bool built;

    void Build()
    {
        built = true;
        rt = (RectTransform)transform;

        Image hit = GetComponent<Image>();
        if (hit == null) hit = gameObject.AddComponent<Image>();
        hit.color = new Color(0f, 0f, 0f, 0f);
        hit.raycastTarget = true;

        float w = Mathf.Max(40f, rt.rect.width);
        float h = Mathf.Max(10f, rt.rect.height);
        knobSize = h * 0.9f;

        Sprite wide = UIShapes.Pill(256, Mathf.RoundToInt(256f * h / w));
        UIShapes.NewImage(transform, "Cover", wide, coverColor, -coverGrow);

        Sprite band = UIShapes.Pill(256, Mathf.RoundToInt(256f * (h * trackHeight) / w));
        Image track = UIShapes.NewImage(transform, "Track", band, trackColor);
        SetBand(track.rectTransform);

        fill = UIShapes.NewImage(transform, "Fill", band, fillColor);
        SetBand(fill.rectTransform);
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = 0;
        fill.fillAmount = 0.7f;

        Sprite knobSprite = UIShapes.Knob(64, new Color32(214, 140, 10, 255), new Color32(255, 222, 90, 255));
        knob = UIShapes.NewImage(transform, "Knob", knobSprite, Color.white).rectTransform;
        knob.anchorMin = new Vector2(0f, 0.5f);
        knob.anchorMax = new Vector2(0f, 0.5f);
        knob.pivot = new Vector2(0.5f, 0.5f);
        knob.sizeDelta = new Vector2(knobSize, knobSize);

        if (percentArea != null)
        {
            float pw = Mathf.Max(20f, percentArea.rect.width);
            float ph = Mathf.Max(10f, percentArea.rect.height);
            Sprite box = UIShapes.Pill(128, Mathf.RoundToInt(128f * ph / pw));
            UIShapes.NewImage(percentArea, "PercentBox", box, percentBoxColor, -coverGrow * 0.5f);
            percentText = UIShapes.NewText(percentArea, "PercentText", "", Color.white);
        }
    }

    void SetBand(RectTransform r)
    {
        r.anchorMin = new Vector2(0f, 0.5f - trackHeight * 0.5f);
        r.anchorMax = new Vector2(1f, 0.5f + trackHeight * 0.5f);
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }

    void Layout()
    {
        float w = Mathf.Max(40f, rt.rect.width);
        float r = knobSize * 0.5f;
        float x = r + value * (w - 2f * r);
        knob.anchoredPosition = new Vector2(x, 0f);
        fill.fillAmount = x / w;
        if (percentText != null) percentText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void SetValue(float v, bool notify)
    {
        if (!built) Build();
        value = Mathf.Clamp01(v);
        Layout();
        if (notify && Changed != null) Changed(value);
    }

    void FromPointer(PointerEventData e)
    {
        if (!built) Build();
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, e.position, e.pressEventCamera, out local)) return;
        float r = knobSize * 0.5f;
        float w = rt.rect.width;
        float x = local.x - rt.rect.xMin;
        SetValue((x - r) / Mathf.Max(1f, w - 2f * r), true);
    }

    public void OnPointerDown(PointerEventData e) { FromPointer(e); }
    public void OnDrag(PointerEventData e) { FromPointer(e); }
    public void OnPointerUp(PointerEventData e) { if (Released != null) Released(); }
}
