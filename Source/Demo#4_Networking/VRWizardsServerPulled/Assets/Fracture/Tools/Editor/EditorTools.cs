using UnityEngine;
using System;

public class HorizontalScroll : IDisposable
{
    private readonly Vector2 position;
    public Vector2 Position
    {
        get { return position; }
    }

    public HorizontalScroll(Vector2 position, float height)
    {
        this.position = GUILayout.BeginScrollView(position, false, true, GUILayout.Height(height));
    }

    public HorizontalScroll(Vector2 position, string style, float height)
    {
        this.position = GUILayout.BeginScrollView(position, false, true, style, style, GUILayout.Height(height));
    }

    public void Dispose()
    {
        GUILayout.EndScrollView();
    }
}

public class GUIColor : IDisposable
{
    public GUIColor(Color color)
    {
        GUI.color = color;
    }

    public void Dispose()
    {
        GUI.color = Color.white;
    }
}

public class GUIContentColor : IDisposable
{
    public GUIContentColor(Color color)
    {
        GUI.contentColor = color;
    }

    public void Dispose()
    {
        GUI.contentColor = Color.white;
    }
}

public class GUIBackgroundColor : IDisposable
{
    public GUIBackgroundColor(Color color)
    {
        GUI.backgroundColor = color;
    }

    public void Dispose()
    {
        GUI.backgroundColor = Color.white;
    }
}

public class Horizontal : IDisposable
{
    public Horizontal()
    {
        GUILayout.BeginHorizontal();
    }

    public Horizontal(string style)
    {
        GUILayout.BeginHorizontal(style);
    }

    public void Dispose()
    {
        GUILayout.EndHorizontal();
    }
}

public class Vertical : IDisposable
{
    public Vertical()
    {
        GUILayout.BeginVertical();
    }

    public Vertical(string style)
    {
        GUILayout.BeginVertical(style);
    }

    public void Dispose()
    {
        GUILayout.EndVertical();
    }
}
