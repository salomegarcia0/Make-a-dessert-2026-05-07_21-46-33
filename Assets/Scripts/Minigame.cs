using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillCheckMinigame : MonoBehaviour
{
    [Header("=== UI References ===")]
    public GameObject         panel;
    public TextMeshProUGUI    hitCounterText;
    public TextMeshProUGUI    instructionText;
    public Image              feedbackFlash;
    public RectTransform      ringParent;

    [Header("=== Configuración ===")]
    public float   needleSpeed         = 180f;
    public float   speedIncreasePerHit = 20f;
    public float   successZoneSize     = 50f;
    public float   zoneShrinkPerHit    = 4f;
    public int     requiredHits        = 5;
    public int     maxFails            = 3;
    public KeyCode skillCheckKey       = KeyCode.Space;

    [Header("=== Colores flash ===")]
    public Color successColor = new Color(0.2f, 1f,   0.3f, 0.6f);
    public Color failColor    = new Color(1f,   0.2f, 0.2f, 0.6f);

    private float  _angle, _speed, _zoneStart, _zoneSize;
    private int    _hits, _fails;
    private bool   _active, _pausing;
    private float  _pauseTimer;

    private CookingStation   _station;
    private PlayerPickup     _pickup;
    private SkillCheckDrawer _drawer;

    void Start()
    {
        if (panel) panel.SetActive(false);
        if (feedbackFlash) feedbackFlash.gameObject.SetActive(false);
        EnsureDrawer();
        if (_drawer) { _drawer.Deactivate(); _drawer.gameObject.SetActive(false); }
    }

    void EnsureDrawer()
    {
        if (ringParent == null)
        {
            var go = new GameObject("RingDrawer");
            go.transform.SetParent(panel != null ? panel.transform : transform, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(380f, 380f);
            rt.anchoredPosition = Vector2.zero;
            ringParent = rt;
        }
        _drawer = ringParent.GetComponent<SkillCheckDrawer>();
        if (_drawer == null) _drawer = ringParent.gameObject.AddComponent<SkillCheckDrawer>();
    }

    void Update()
    {
        if (!_active) return;

        if (_pausing)
        {
            _pauseTimer -= Time.deltaTime;
            if (_pauseTimer <= 0f)
            {
                _pausing = false;
                if (feedbackFlash) feedbackFlash.gameObject.SetActive(false);
                RandomizeZone();
            }
            return;
        }

        _angle = (_angle + _speed * Time.deltaTime) % 360f;
        _drawer.SetState(_angle, _zoneStart, _zoneSize);

        if (Input.GetKeyDown(skillCheckKey)) CheckHit();
    }

    public void StartMinigame(CookingStation station, PlayerPickup pickup)
    {
        _station  = station;
        _pickup   = pickup;
        _hits = 0; _fails = 0;
        _angle    = 0f;
        _speed    = needleSpeed;
        _zoneSize = successZoneSize;
        _active   = true;
        _pausing  = false;

        EnsureDrawer();
        _drawer.gameObject.SetActive(true);
        _drawer.Activate();

        if (panel) panel.SetActive(true);
        if (feedbackFlash) feedbackFlash.gameObject.SetActive(false);

        // Configurar textos una sola vez
        SetupTextLayout();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        UpdateUI();
        RandomizeZone();
    }

    void SetupTextLayout()
    {
        // Contador grande dentro del anillo (centro)
        if (hitCounterText != null)
        {
            var rt = hitCounterText.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(200f, 80f);
            rt.anchoredPosition = new Vector2(0f, 20f); // ligeramente arriba del centro
            hitCounterText.fontSize  = 64;
            hitCounterText.fontStyle = TMPro.FontStyles.Bold;
            hitCounterText.alignment = TMPro.TextAlignmentOptions.Center;
            hitCounterText.color     = Color.white;
        }

        // Instrucción pequeña debajo del contador, dentro del anillo
        if (instructionText != null)
        {
            var rt = instructionText.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(220f, 70f);
            rt.anchoredPosition = new Vector2(0f, -35f); // debajo del contador
            instructionText.fontSize    = 22;
            instructionText.fontStyle   = TMPro.FontStyles.Normal;
            instructionText.alignment   = TMPro.TextAlignmentOptions.Center;
            instructionText.color       = new Color(1f, 1f, 1f, 0.85f);
            instructionText.textWrappingMode = TMPro.TextWrappingModes.Normal;
        }
    }

    void CheckHit()
    {
        if (InZone(_angle, _zoneStart, _zoneSize)) HitSuccess(); else HitFail();
    }

    bool InZone(float a, float s, float size)
    {
        float e = (s + size) % 360f;
        return e > s ? a >= s && a <= e : a >= s || a <= e;
    }

    void HitSuccess()
    {
        _hits++;
        _speed    += speedIncreasePerHit;
        _zoneSize  = Mathf.Max(18f, _zoneSize - zoneShrinkPerHit);
        Flash(successColor);
        UpdateUI();
        if (_hits >= requiredHits) { Finish(true); return; }
        Pause(0.4f);
    }

    void HitFail()
    {
        _fails++;
        Flash(failColor);
        UpdateUI();
        if (maxFails > 0 && _fails >= maxFails) { Finish(false); return; }
        Pause(0.65f);
    }

    void Flash(Color c)
    {
        if (!feedbackFlash) return;
        feedbackFlash.color = c;
        feedbackFlash.gameObject.SetActive(true);
    }

    void Pause(float t) { _pausing = true; _pauseTimer = t; }

    void RandomizeZone()
    {
        float gap    = 80f;
        float offset = Random.Range(gap, 360f - _zoneSize - gap);
        _zoneStart   = (_angle + offset) % 360f;
        _drawer.SetState(_angle, _zoneStart, _zoneSize);
    }

    void UpdateUI()
    {
        if (hitCounterText)
            hitCounterText.text = _hits + " / " + requiredHits;

        if (instructionText)
        {
            if (_fails > 0 && maxFails > 0)
                instructionText.text = "¡ESPACIO!\nFallos: " + _fails + "/" + maxFails;
            else
                instructionText.text = "Presiona\nESPACIO";
        }
    }

    void Finish(bool ok)
    {
        _active = false;
        if (hitCounterText)   hitCounterText.text   = ok ? "¡LISTO!" : "¡QUEMADO!";
        if (instructionText)  instructionText.text  = "";
        Invoke(ok ? nameof(DoSuccess) : nameof(DoFail), ok ? 0.8f : 1.2f);
    }

    void DoSuccess() { ClosePanel(); _station?.OnCookingSuccess(_pickup); }
    void DoFail()    { ClosePanel(); _station?.OnCookingFailed(); }

    void ClosePanel()
    {
        if (_drawer)       _drawer.gameObject.SetActive(false);
        if (panel)         panel.SetActive(false);
        if (feedbackFlash) feedbackFlash.gameObject.SetActive(false);
    }
}