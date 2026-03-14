using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Add as component to any empty object
// Builds and updates the scrolling note-track UI at the top of the screen.
// Put this on the same GameObject as InputIndicatorManager.
[RequireComponent(typeof(InputIndicatorManager))]
public class InputIndicatorTrackUI : MonoBehaviour
{
    [Header("Bar Layout")]
    [SerializeField] private float _barPadding = 12f;
    [SerializeField] private float _trackHeight = 52f;
    [SerializeField] private float _topMargin = 10f;
    [SerializeField] private Sprite _barSprite;
    [SerializeField] private Sprite _trackSprite;

    [Header("Scroll")]
    [SerializeField] private float _travelTime = 2f;

    [Header("Hit Zone & Notes")]
    [SerializeField] private float _hitZoneXPercent = 0.07f;
    [SerializeField] private float _noteSize = 42f;
    [SerializeField] private Sprite _hitZoneSprite;
    [SerializeField] private Sprite _specialSprite;
    [SerializeField] private float _specialIconSize = 16f;

    private InputIndicatorManager _manager;
    private RectTransform _trackRect;
    private Image _hitZoneImage;
    private readonly Dictionary<BeatNote, RectTransform> _noteVisuals = new();

    private static readonly Color[] _indicatorColors =
    {
        new(0.95f, 0.30f, 0.40f),
        new(0.30f, 0.65f, 1.00f),
        new(0.30f, 0.95f, 0.50f),
        new(1.00f, 0.85f, 0.30f),
        new(1.00f, 0.50f, 0.15f),
        new(0.70f, 0.35f, 1.00f),
        new(0.20f, 0.90f, 0.90f),
        new(1.00f, 0.45f, 0.70f),
    };

    private void Awake()
    {
        _manager = GetComponent<InputIndicatorManager>();
    }

    private void Start()
    {
        var canvasRect = BuildCanvas();
        BuildBar(canvasRect);
        SpawnAllNotes();

        _manager.OnBeatConsumed += HandleBeatConsumed;
        _manager.OnHitResult += HandleHitResult;
        _manager.OnBeatAdded += HandleBeatAdded;
    }

    private void OnDestroy()
    {
        if (_manager != null)
        {
            _manager.OnBeatConsumed -= HandleBeatConsumed;
            _manager.OnHitResult -= HandleHitResult;
            _manager.OnBeatAdded -= HandleBeatAdded;
        }
    }

    private void Update()
    {
        UpdateNotePositions();
    }

    // Creates a screen-space overlay canvas for the track.
    private RectTransform BuildCanvas()
    {
        var go = new GameObject("IndicatorTrackCanvas");
        go.transform.SetParent(transform, false);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go.GetComponent<RectTransform>();
    }

    // Builds the dark bar, inner track, glow, and hit-zone line.
    private void BuildBar(RectTransform canvasRect)
    {
        float barHeight = _trackHeight + _barPadding * 2f;

        var bar = CreateImage("IndicatorBar", canvasRect, new Color(0.06f, 0.06f, 0.09f, 0.92f), _barSprite);
        var barRect = bar.rectTransform;
        barRect.anchorMin = new Vector2(0f, 1f);
        barRect.anchorMax = new Vector2(1f, 1f);
        barRect.pivot = new Vector2(0.5f, 1f);
        barRect.sizeDelta = new Vector2(-40f, barHeight);
        barRect.anchoredPosition = new Vector2(0f, -_topMargin);

        var track = CreateImage("Track", barRect, new Color(0.10f, 0.10f, 0.13f, 0.90f), _trackSprite);
        _trackRect = track.rectTransform;
        _trackRect.anchorMin = Vector2.zero;
        _trackRect.anchorMax = Vector2.one;
        _trackRect.offsetMin = new Vector2(_barPadding, _barPadding);
        _trackRect.offsetMax = new Vector2(-_barPadding, -_barPadding);

        var glow = CreateImage("HitGlow", _trackRect, new Color(1f, 1f, 1f, 0.06f));
        var glowRect = glow.rectTransform;
        glowRect.anchorMin = new Vector2(_hitZoneXPercent, 0f);
        glowRect.anchorMax = new Vector2(_hitZoneXPercent, 1f);
        glowRect.pivot = new Vector2(0.5f, 0.5f);
        glowRect.sizeDelta = new Vector2(32f, 0f);
        glowRect.anchoredPosition = Vector2.zero;

        var hitLine = CreateImage("HitZone", _trackRect, new Color(1f, 1f, 1f, 0.50f));
        var hitRect = hitLine.rectTransform;
        hitRect.anchorMin = new Vector2(_hitZoneXPercent, 0f);
        hitRect.anchorMax = new Vector2(_hitZoneXPercent, 1f);
        hitRect.pivot = new Vector2(0.5f, 0.5f);

        if (_hitZoneSprite != null)
        {
            hitLine.sprite = _hitZoneSprite;
            hitLine.preserveAspect = true;
            hitRect.sizeDelta = new Vector2(_noteSize, 0f);
        }
        else
        {
            hitRect.sizeDelta = new Vector2(3f, 0f);
        }

        hitRect.anchoredPosition = Vector2.zero;
        _hitZoneImage = hitLine;
    }

    private void SpawnAllNotes()
    {
        foreach (var beat in _manager.BeatMap)
        {
            if (beat.IsConsumed || beat.IndicatorIndex >= _manager.Indicators.Count) continue;
            SpawnNote(beat);
        }
    }

    // Creates the colored square (or sprite) for one beat note on the track.
    private void SpawnNote(BeatNote beat)
    {
        int idx = beat.IndicatorIndex;
        InputIndicator indicator = _manager.Indicators[idx];
        Color color = _indicatorColors[idx % _indicatorColors.Length];

        var note = CreateImage("Note", _trackRect, color);
        var rt = note.rectTransform;
        rt.anchorMin = new Vector2(_hitZoneXPercent, 0.5f);
        rt.anchorMax = new Vector2(_hitZoneXPercent, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(_noteSize, _noteSize);
        rt.anchoredPosition = Vector2.zero;

        if (indicator.NoteSprite != null)
        {
            note.sprite = indicator.NoteSprite;
            note.preserveAspect = true;
            note.color = Color.white;
        }
        else
        {
            var label = CreateLabel("Key", rt, indicator.InputKey.ToString(), 20,
                new Color(0f, 0f, 0f, 0.85f));
            var lr = label.rectTransform;
            lr.anchorMin = Vector2.zero;
            lr.anchorMax = Vector2.one;
            lr.offsetMin = Vector2.zero;
            lr.offsetMax = Vector2.zero;
            label.alignment = TextAlignmentOptions.Center;
            label.fontStyle = FontStyles.Bold;
        }

        if (beat.IsSpecial && _specialSprite != null)
        {
            var icon = CreateImage("Special", rt, Color.white, _specialSprite);
            var ir = icon.rectTransform;
            ir.anchorMin = new Vector2(1f, 1f);
            ir.anchorMax = new Vector2(1f, 1f);
            ir.pivot = new Vector2(1f, 1f);
            ir.sizeDelta = new Vector2(_specialIconSize, _specialIconSize);
            ir.anchoredPosition = new Vector2(2f, 2f);
            icon.preserveAspect = true;
        }

        _noteVisuals[beat] = rt;
    }

    // Slides each note toward the hit zone and destroys it once it scrolls off.
    private void UpdateNotePositions()
    {
        if (_trackRect == null) return;

        float trackWidth = _trackRect.rect.width;
        float travelWidth = trackWidth * (1f - _hitZoneXPercent);

        List<BeatNote> cleanup = null;

        foreach (var kvp in _noteVisuals)
        {
            BeatNote beat = kvp.Key;
            RectTransform rt = kvp.Value;

            if (rt == null)
            {
                cleanup ??= new List<BeatNote>();
                cleanup.Add(beat);
                continue;
            }

            float timeLeft = beat.HitTime - _manager.SongTime;
            float t = timeLeft / _travelTime;
            float x = t * travelWidth;

            rt.anchoredPosition = new Vector2(x, 0f);
            rt.gameObject.SetActive(t <= 1.05f);

            if (t < -0.3f)
            {
                Destroy(rt.gameObject);
                cleanup ??= new List<BeatNote>();
                cleanup.Add(beat);
            }
        }

        if (cleanup != null)
        {
            foreach (var b in cleanup)
                _noteVisuals.Remove(b);
        }
    }

    private void HandleBeatAdded(BeatNote beat)
    {
        if (beat.IsConsumed || beat.IndicatorIndex >= _manager.Indicators.Count) return;
        SpawnNote(beat);
    }

    private void HandleBeatConsumed(BeatNote beat, HitResult result)
    {
        if (!_noteVisuals.TryGetValue(beat, out var rt)) return;
        if (rt != null) Destroy(rt.gameObject);
        _noteVisuals.Remove(beat);
    }

    // Flashes the hit zone green (perfect) or red (fail).
    private void HandleHitResult(int indicatorIndex, HitResult result)
    {
        if (_hitZoneImage == null) return;

        Color flash = result == HitResult.Perfect
            ? new Color(0f, 1f, 0.4f, 0.90f)
            : new Color(1f, 0.1f, 0.1f, 0.90f);

        StartCoroutine(FlashHitZone(flash));
    }

    private IEnumerator FlashHitZone(Color flashColor)
    {
        Color original = _hitZoneImage.color;
        _hitZoneImage.color = flashColor;

        const float duration = 0.18f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            _hitZoneImage.color = Color.Lerp(flashColor, original, elapsed / duration);
            yield return null;
        }

        _hitZoneImage.color = original;
    }

    private static Image CreateImage(string name, RectTransform parent, Color color,
        Sprite sprite = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();

        if (sprite != null)
        {
            img.sprite = sprite;
            img.type = Image.Type.Simple;
            img.color = Color.white;
        }
        else
        {
            img.color = color;
        }

        return img;
    }

    private static TextMeshProUGUI CreateLabel(string name, RectTransform parent,
        string text, float fontSize, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        return tmp;
    }
}
