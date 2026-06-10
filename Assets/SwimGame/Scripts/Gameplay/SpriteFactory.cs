using UnityEngine;

public static class SpriteFactory
{
    private static Sprite square;
    private static Sprite circle;
    private static Sprite triangle;
    private static Texture2D circleTexture;

    public static Sprite Square
    {
        get
        {
            if (square == null) square = CreateSquare();
            return square;
        }
    }

    public static Sprite Circle
    {
        get
        {
            if (circle == null) circle = CreateCircle();
            return circle;
        }
    }

    public static Sprite Triangle
    {
        get
        {
            if (triangle == null) triangle = CreateTriangle();
            return triangle;
        }
    }

    public static Texture2D CircleTexture
    {
        get
        {
            if (circleTexture == null) CreateCircle();
            return circleTexture;
        }
    }

    private static Sprite CreateSquare()
    {
        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < 16; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }

    private static Sprite CreateCircle()
    {
        int size = 128;
        var tex = new Texture2D(size, size);
        float radius = size / 2f - 1f;
        var center = new Vector2(size / 2f, size / 2f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float a = Mathf.Clamp01(radius - d + 0.5f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        circleTexture = tex;
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    private static Sprite CreateTriangle()
    {
        int size = 64;
        var tex = new Texture2D(size, size);
        float centerX = size / 2f;
        for (int y = 0; y < size; y++)
        {
            float t = (float)y / size;
            float halfWidth = (1f - t) * size / 2f;
            for (int x = 0; x < size; x++)
            {
                float dist = Mathf.Abs(x + 0.5f - centerX);
                float a = Mathf.Clamp01(halfWidth - dist + 0.5f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
