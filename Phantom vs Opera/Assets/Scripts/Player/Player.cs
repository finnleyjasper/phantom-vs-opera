using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    // Private Variables
        private bool _isAlive;
        private bool _hasWon;
        private int _healthBar;
        private float _successBar;
        [SerializeField] private int _maxHealth = 10;
        [SerializeField] private float _maxSuccessMeter = 10f;
        private readonly List<Renderer> _renderers = new();
        private readonly List<Color> _baseColors = new();
        private Coroutine _hitFlashRoutine;

    [Header("Hit Flash")]
    [SerializeField] private Color _hitFlashColor = Color.red;
    [SerializeField] private float _hitFlashDuration = 0.08f;
    [SerializeField] private int _hitFlashCount = 2;

    // Reference to PlayerBarUI Script 
    [Header("Player Health Bar UI")]
    [SerializeField] private PlayerBarUI playerHealthBarUI;

    [Header("Player Success Bar UI")]
    [SerializeField] private PlayerBarUI playerSuccessBarUI;

    // Set up Initial health/success levels in Start
    void Start()
    {
        _healthBar = _maxHealth;
        _successBar = 0;
        _isAlive = true;
        _hasWon = false;
        CacheRenderers();

        Debug.Log("Initial health: " + _healthBar);
        Debug.Log("Initial success: " + _successBar);

        playerHealthBarUI.UpdatePlayerHealthUI();
        playerSuccessBarUI.UpdatePlayerSuccessUI();
    }

    // Properties

    public int HealthBar
    {
        get { return _healthBar; }
    }

    public float SuccessBar
    {
        get { return _successBar; }
    }

    public bool IsAlive
    {
        get { return _isAlive; }
    }

    public bool HasWon
    {
        get { return _hasWon; }
    }

    public int MaxHealth => _maxHealth;

    public float MaxSuccessMeter => _maxSuccessMeter;

    // Method to Manage Health Bar - sets initial health bar level, sets results for losing all health (i.e. losing game)
    public void ManagePlayerLose()
    {
        if (_healthBar <= 0)
        {
            _isAlive = false;
            Debug.Log("isAlive status: " + _isAlive);
            Debug.Log("health bar: " + _healthBar + " Game over !");
            GameManager.Instance.GameOver(GameManager.GameState.Lose);
        }
    }

    // Method to Manage Success Bar - sets initial success bar level, sets results for reaching certain success level (i.e. winning game)
    public void ManagePlayerWin()
    {
        if (_successBar >= _maxSuccessMeter)
        {
            _hasWon = true;
            Debug.Log("hasWon status: " + _hasWon + " You won !");
            Debug.Log("success bar: " + _successBar);
            GameManager.Instance.GameOver(GameManager.GameState.Win);
        }
    }

    // Method for when Falling Object hits Player = health decreases - method is called by falling objects

    public void IsHit(int damage)
    {
        _healthBar -= damage; 
        _healthBar = Mathf.Clamp(_healthBar, 0, _maxHealth); // Clamp - health bars cannot go below 0 or above max
        Debug.Log("health bar: " + _healthBar);
        TriggerHitFlash();
     
        if (_healthBar <= 0)
        {
            ManagePlayerLose(); 
        }

        playerHealthBarUI.UpdatePlayerHealthUI(); 
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        _maxHealth = Mathf.Max(1, newMaxHealth);
        _healthBar = Mathf.Clamp(_healthBar, 0, _maxHealth);
        playerHealthBarUI.UpdatePlayerHealthUI();
    }

    public void SetMaxSuccessMeter(float newMaxSuccessMeter)
    {
        _maxSuccessMeter = Mathf.Max(1f, newMaxSuccessMeter);
        _successBar = Mathf.Clamp(_successBar, 0f, _maxSuccessMeter);
        playerSuccessBarUI.UpdatePlayerSuccessUI();
    }

    private void CacheRenderers()
    {
        _renderers.Clear();
        _baseColors.Clear();

        var childRenderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer rendererComponent in childRenderers)
        {
            if (rendererComponent == null || rendererComponent.material == null) continue;

            bool hasBaseColor = rendererComponent.material.HasProperty("_BaseColor");
            bool hasColor = rendererComponent.material.HasProperty("_Color");
            if (!hasBaseColor && !hasColor) continue;

            _renderers.Add(rendererComponent);
            _baseColors.Add(hasBaseColor
                ? rendererComponent.material.GetColor("_BaseColor")
                : rendererComponent.material.color);
        }
    }

    private void TriggerHitFlash()
    {
        if (_renderers.Count == 0) return;

        if (_hitFlashRoutine != null)
            StopCoroutine(_hitFlashRoutine);

        _hitFlashRoutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        for (int i = 0; i < _hitFlashCount; i++)
        {
            SetRendererColor(_hitFlashColor);
            yield return new WaitForSeconds(_hitFlashDuration);

            RestoreRendererColor();
            if (i < _hitFlashCount - 1)
                yield return new WaitForSeconds(_hitFlashDuration);
        }

        _hitFlashRoutine = null;
    }

    private void SetRendererColor(Color color)
    {
        foreach (Renderer rendererComponent in _renderers)
        {
            if (rendererComponent == null || rendererComponent.material == null) continue;

            if (rendererComponent.material.HasProperty("_BaseColor"))
                rendererComponent.material.SetColor("_BaseColor", color);
            else if (rendererComponent.material.HasProperty("_Color"))
                rendererComponent.material.color = color;
        }
    }

    private void RestoreRendererColor()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            Renderer rendererComponent = _renderers[i];
            if (rendererComponent == null || rendererComponent.material == null) continue;

            Color baseColor = _baseColors[i];
            if (rendererComponent.material.HasProperty("_BaseColor"))
                rendererComponent.material.SetColor("_BaseColor", baseColor);
            else if (rendererComponent.material.HasProperty("_Color"))
                rendererComponent.material.color = baseColor;
        }
    }

    // Method for when Player wins if game time ends 
    public void PlayerSuccessTimer()
    {
        _successBar = GameManager.Instance.GameTimer;
        _successBar = Mathf.Clamp(_successBar, 0f, _maxSuccessMeter); // Clamps success bar to current max
        playerSuccessBarUI.UpdatePlayerSuccessUI();

        // Calls win condition if game length is reached 
        if (_successBar >= _maxSuccessMeter)
        {
            ManagePlayerWin();
        }
    }

    void Update()
    {
        PlayerSuccessTimer();
    }
}

