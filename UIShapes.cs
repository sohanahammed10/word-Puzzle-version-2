using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// ছবির ওপর একটা জায়গা। সংখ্যা ০ থেকে ১ : বাম, উপর, ডান, নিচ (ছবির উপর-বাম কোণ থেকে মাপা)
[System.Serializable]
public struct Zone
{
    public float left, top, right, bottom;

    public Zone(float left, float top, float right, float bottom)
    {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
    }
}

public static class UIShapes
{
    public static RectTransform Area(Transform parent, string name, Zone z)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = new Vector2(z.left, 1f - z.bottom);
        rt.anchorMax = new Vector2(z.right, 1f - z.top);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return rt;
    }

    public static RectTransform Stretch(Transform parent, string name, float pad = 0f)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(pad, pad);
        rt.offsetMax = new Vector2(-pad, -pad);
        return rt;
    }

    public static Image NewImage(Transform parent, string name, Sprite sprite, Color color, float pad = 0f)
    {
        RectTransform rt = Stretch(parent, name, pad);
        Image im = rt.gameObject.AddComponent<Image>();
        im.sprite = sprite;
        im.color = color;
        im.raycastTarget = false;
        return im;
    }

    public static TMP_Text NewText(Transform parent, string name, string text, Color color)
    {
        RectTransform rt = Stretch(parent, name);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.color = color;
        t.fontStyle = FontStyles.Bold;
        t.alignment = TextAlignmentOptions.Center;
        t.enableAutoSizing = true;
        t.fontSizeMin = 10;
        t.fontSizeMax = 90;
        t.raycastTarget = false;
        return t;
    }

    // অদৃশ্য চাপার জায়গা (ছবির ওপর বসানো বাটন)
    public static void MakeButton(RectTransform area, UnityAction onClick)
    {
        Image im = area.GetComponent<Image>();
        if (im == null) im = area.gameObject.AddComponent<Image>();
        im.color = new Color(0f, 0f, 0f, 0f);
        im.raycastTarget = true;

        Button b = area.GetComponent<Button>();
        if (b == null) b = area.gameObject.AddComponent<Button>();
        b.transition = Selectable.Transition.None;
        b.targetGraphic = im;
        b.onClick.AddListener(onClick);
    }

    // দুই মাথা গোল লম্বা আকার (সাদা রঙের, Image.color দিয়ে রং বদলাও)
    public static Sprite Pill(int w, int h)
    {
        h = Mathf.Max(8, h);
        w = Mathf.Max(h, w);
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = h * 0.5f;
        Color32[] px = new Color32[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float cx = Mathf.Clamp(x + 0.5f, r, w - r);
                float dx = x + 0.5f - cx;
                float dy = y + 0.5f - r;
                float a = Mathf.Clamp01(r - Mathf.Sqrt(dx * dx + dy * dy) + 0.5f);
                px[y * w + x] = new Color32(255, 255, 255, (byte)(a * 255f));
            }
        }
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);
    }

    // গোল নব (বাইরে এক রং, ভেতরে আরেক রং)
    public static Sprite Knob(int s, Color outer, Color inner)
    {
        s = Mathf.Max(16, s);
        Texture2D tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float r = s * 0.5f;
        Color32[] px = new Color32[s * s];
        for (int y = 0; y < s; y++)
        {
            for (int x = 0; x < s; x++)
            {
                float dx = x + 0.5f - r;
                float dy = y + 0.5f - r;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Clamp01(r - d + 0.5f);
                float k = Mathf.Clamp01((d - r * 0.66f) / (r * 0.10f));
                Color32 c = Color.Lerp(inner, outer, k);
                c.a = (byte)(a * 255f);
                px[y * s + x] = c;
            }
        }
        tex.SetPixels32(px);
        tex.Apply(false, true);
        return Sprite.Create(tex, new Rect(0, 0, s, s), new Vector2(0.5f, 0.5f), 100f);
    }
}
