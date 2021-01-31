using UnityEngine;
using DG.Tweening;

public static class Helpers {

    public static Vector2 xy(this Vector3 v) {
		return new Vector2(v.x, v.y);
	}

    public static Vector3 xyz(this Vector2 v) {
        return new Vector3(v.x, v.y, 0);
    }

	public static Vector3 WithX(this Vector3 v, float x) {
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y) {
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z) {
		return new Vector3(v.x, v.y, z);
	}

	public static Vector2 WithX(this Vector2 v, float x) {
		return new Vector2(x, v.y);
	}
	
	public static Vector2 WithY(this Vector2 v, float y) {
		return new Vector2(v.x, y);
	}
	
	public static Vector3 WithZ(this Vector2 v, float z) {
		return new Vector3(v.x, v.y, z);
    }

    public static float GetH(this Color c) {
        Color.RGBToHSV(c, out float h, out _, out _);
        return h;
    }

    public static float GetS(this Color c) {
        Color.RGBToHSV(c, out _, out float s, out _);
        return s;
    }

    public static float GetV(this Color c) {
        Color.RGBToHSV(c, out _, out _, out float v);
        return v;
    }

    public static Color WithH(this Color c, float h) {
        float S, V;
        Color.RGBToHSV(c, out _, out S, out V);
        return Color.HSVToRGB(h, S, V);
    }

    public static Color WithS(this Color c, float s) {
        float H, V;
        Color.RGBToHSV(c, out H, out _, out V);
        return Color.HSVToRGB(H, s, V);
    }

    public static Color WithV(this Color c, float v) {
        float H, S;
        Color.RGBToHSV(c, out H, out S, out _);
        return Color.HSVToRGB(H, S, v);
    }

    public static Color WithA(this Color c, float a) {
        return new Color(c.r, c.g, c.b, a);
    }

    public static Color2 ab(this Color c) {
        return new Color2(c, c);
    }

    public static void SetWidth(this LineRenderer line, float w) {
        line.startWidth = w;
        line.endWidth = w;
    }

    public static void SetColor(this LineRenderer line, Color c) {
        line.startColor = c;
        line.endColor = c;
    }
}