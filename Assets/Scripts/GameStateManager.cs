using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

// This script tracks all global game state — balance, photos, flags like "hasIceCream".
// It uses the Singleton pattern so other scripts can access it via GameStateManager.Instance
public class GameStateManager : MonoBehaviour
{
    // ─── SINGLETON SETUP ───────────────────────────────────
    // 'Instance' is a static reference, meaning any script can call
    // GameStateManager.Instance.balance from anywhere
    public static GameStateManager Instance { get; private set; }

    // ─── INSPECTOR FIELDS ──────────────────────────────────
    [Header("Starting Values")]
    public int startingBalance = 250;

    [Header("Optional UI Auto-Update")]
    [Tooltip("Drag a TMP_Text here and it will auto-update when balance changes")]
    public TMP_Text balanceUIText;
    [Tooltip("Drag a TMP_Text here for photo count display")]
    public TMP_Text photoCountUIText;

    // ─── STATE VARIABLES ───────────────────────────────────
    public int balance { get; private set; }
    public int photosTaken { get; private set; }

    // Dictionary stores arbitrary boolean flags (hasScannedFace, hasIceCream, etc.)
    private Dictionary<string, bool> flags = new Dictionary<string, bool>();

    // Event that other scripts can subscribe to for change notifications
    public UnityEvent OnStateChanged;

    // ─── UNITY LIFECYCLE ───────────────────────────────────
    void Awake()
    {
        // Singleton enforcement: if another instance exists, destroy this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize starting state
        balance = startingBalance;
        photosTaken = 0;

        if (OnStateChanged == null)
            OnStateChanged = new UnityEvent();
    }

    void Start()
    {
        // Refresh UI on start
        UpdateUI();
    }

    // ─── PUBLIC METHODS ────────────────────────────────────

    // Try to spend money. Returns true if successful, false if insufficient.
    public bool SpendBalance(int amount)
    {
        if (balance < amount)
        {
            Debug.Log("Not enough balance! Need " + amount + ", have " + balance);
            return false;
        }
        balance -= amount;
        UpdateUI();
        OnStateChanged.Invoke();
        return true;
    }

    // Add money (in case you ever need it — e.g., refund)
    public void AddBalance(int amount)
    {
        balance += amount;
        UpdateUI();
        OnStateChanged.Invoke();
    }

    // Increase photo counter
    public void IncrementPhotoCount()
    {
        photosTaken++;
        UpdateUI();
        OnStateChanged.Invoke();
    }

    // Set a named flag (e.g., SetFlag("hasIceCream", true))
    public void SetFlag(string flagName, bool value)
    {
        flags[flagName] = value;
        OnStateChanged.Invoke();
    }

    // Read a named flag (returns false if it was never set)
    public bool GetFlag(string flagName)
    {
        return flags.ContainsKey(flagName) && flags[flagName];
    }

    // ─── INTERNAL ──────────────────────────────────────────
    private void UpdateUI()
    {
        if (balanceUIText != null)
            balanceUIText.text = "¥" + balance;
        if (photoCountUIText != null)
            photoCountUIText.text = "Photos: " + photosTaken;
    }
}