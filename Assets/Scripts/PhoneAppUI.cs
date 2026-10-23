using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;  // NEW INPUT SYSTEM
using TMPro;

// This script controls the phone app overlay UI.
// Press TAB to toggle it. Shows balance, photo count, and ride times.
public class PhoneAppUI : MonoBehaviour
{
    [Header("Panel Reference")]
    [Tooltip("Drag the PhonePanel GameObject here")]
    public RectTransform phonePanel;

    [Header("UI Text References")]
    [Tooltip("Drag BalanceText here")]
    public TMP_Text balanceText;
    [Tooltip("Drag PhotoCountText here")]
    public TMP_Text photoCountText;

    [Header("Slide Animation")]
    [Tooltip("Off-screen X position (e.g., 500)")]
    public float hiddenPosX = 500f;
    [Tooltip("On-screen X position (e.g., -50)")]
    public float visiblePosX = -50f;
    public float slideDuration = 0.3f;

    [Header("Audio (Optional)")]
    public AudioClip openSound;
    private AudioSource audioSource;

    // Internal state
    private bool isOpen = false;
    private Coroutine slideCoroutine;

    void Start()
    {
        // Get or add an audio source for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Start hidden
        Vector2 pos = phonePanel.anchoredPosition;
        pos.x = hiddenPosX;
        phonePanel.anchoredPosition = pos;

        // Initial UI sync
        Refresh();
    }

    void Update()
    {
        // NEW INPUT SYSTEM: Detect TAB key press
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        isOpen = !isOpen;

        // Play sound if assigned
        if (openSound != null && audioSource != null)
            audioSource.PlayOneShot(openSound);

        // Stop existing slide if running, start new one
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideRoutine(isOpen));

        if (isOpen) Refresh();
    }

    private IEnumerator SlideRoutine(bool opening)
    {
        Vector2 startPos = phonePanel.anchoredPosition;
        Vector2 endPos = new Vector2(opening ? visiblePosX : hiddenPosX, startPos.y);

        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / slideDuration);
            // Smooth easing for nicer feel
            k = k * k * (3f - 2f * k);
            phonePanel.anchoredPosition = Vector2.Lerp(startPos, endPos, k);
            yield return null;
        }
        phonePanel.anchoredPosition = endPos;
    }

    public void Refresh()
    {
        if (GameStateManager.Instance == null) return;

        if (balanceText != null)
            balanceText.text = "¥" + GameStateManager.Instance.balance;
        if (photoCountText != null)
            photoCountText.text = "Photos: " + GameStateManager.Instance.photosTaken;
    }
}