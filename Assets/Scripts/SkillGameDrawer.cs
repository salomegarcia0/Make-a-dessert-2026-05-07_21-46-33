using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class SkillCheckDrawer : MaskableGraphic
{
    private float _needleAngle;
    private float _zoneStart;
    private float _zoneSize;
    private bool  _ready = false;  // no dibuja nada hasta que el minijuego arranque

    private const float OUTER_R  = 170f;
    private const float INNER_R  = 130f;
    private const int   SEGMENTS = 80;

    private static readonly Color RING_COLOR   = new Color(0.82f, 0.82f, 0.82f, 0.5f);
    private static readonly Color ZONE_COLOR   = new Color(0.15f, 0.95f, 0.35f, 0.80f);
    private static readonly Color NEEDLE_COLOR = new Color(1f,    1f,    1f,    1f);
    private static readonly Color NEEDLE_TIP   = new Color(1f,    0.25f, 0.25f, 1f);

    // Llamado por SkillCheckMinigame al arrancar
    public void Activate()
    {
        _ready = true;
        SetVerticesDirty();
    }

    // Llamado al cerrar el minijuego
    public void Deactivate()
    {
        _ready = false;
        SetVerticesDirty();
    }

    public void SetState(float needleAngle, float zoneStart, float zoneSize)
    {
        _needleAngle = needleAngle;
        _zoneStart   = zoneStart;
        _zoneSize    = zoneSize;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (!_ready) return;  // dibuja nada si no está activo

        DrawArc(vh, 0f,         360f,                   INNER_R, OUTER_R, RING_COLOR, SEGMENTS);
        DrawArc(vh, _zoneStart, _zoneStart + _zoneSize, INNER_R, OUTER_R, ZONE_COLOR, 48);
        DrawNeedle(vh, _needleAngle);
    }

    void DrawArc(VertexHelper vh, float fromDeg, float toDeg,
                 float innerR, float outerR, Color col, int segs)
    {
        float step    = (toDeg - fromDeg) / segs;
        int   baseIdx = vh.currentVertCount;

        for (int i = 0; i <= segs; i++)
        {
            float rad = Mathf.Deg2Rad * (fromDeg + step * i - 90f);
            float c   = Mathf.Cos(rad);
            float s   = Mathf.Sin(rad);
            AddVert(vh, new Vector2(c * innerR, s * innerR), col);
            AddVert(vh, new Vector2(c * outerR, s * outerR), col);
        }

        for (int i = 0; i < segs; i++)
        {
            int b = baseIdx + i * 2;
            vh.AddTriangle(b,     b + 1, b + 2);
            vh.AddTriangle(b + 1, b + 3, b + 2);
        }
    }

    void DrawNeedle(VertexHelper vh, float deg)
    {
        float rad   = Mathf.Deg2Rad * (deg - 90f);
        float cos   = Mathf.Cos(rad);
        float sinR  = Mathf.Sin(rad);
        float perpX = -sinR;
        float perpY =  cos;
        float halfW  = 4f;
        float startR = INNER_R - 12f;
        float endR   = OUTER_R + 20f;

        int b = vh.currentVertCount;
        AddVert(vh, new Vector2(cos * startR + perpX * halfW, sinR * startR + perpY * halfW), NEEDLE_COLOR);
        AddVert(vh, new Vector2(cos * startR - perpX * halfW, sinR * startR - perpY * halfW), NEEDLE_COLOR);
        AddVert(vh, new Vector2(cos * endR   + perpX * halfW, sinR * endR   + perpY * halfW), NEEDLE_COLOR);
        AddVert(vh, new Vector2(cos * endR   - perpX * halfW, sinR * endR   - perpY * halfW), NEEDLE_COLOR);
        vh.AddTriangle(b, b+1, b+2);
        vh.AddTriangle(b+1, b+3, b+2);

        float tipR = endR + 14f;
        int t = vh.currentVertCount;
        AddVert(vh, new Vector2(cos*(endR-2f) + perpX*(halfW+3f), sinR*(endR-2f) + perpY*(halfW+3f)), NEEDLE_TIP);
        AddVert(vh, new Vector2(cos*(endR-2f) - perpX*(halfW+3f), sinR*(endR-2f) - perpY*(halfW+3f)), NEEDLE_TIP);
        AddVert(vh, new Vector2(cos * tipR, sinR * tipR), NEEDLE_TIP);
        vh.AddTriangle(t, t+1, t+2);
    }

    void AddVert(VertexHelper vh, Vector2 pos, Color col)
    {
        var v      = UIVertex.simpleVert;
        v.position = pos;
        v.color    = col;
        vh.AddVert(v);
    }

    public override Texture mainTexture => null;
}