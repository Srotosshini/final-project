using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Toggles a photo-mode UI overlay (camera viewfinder framing) on P key.
// Captures a PNG screenshot when player left-clicks while in photo mode.
public class PhotoMode : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag the PhotoOverlayCanvas GameObject here")]
    public GameObject photoOverlayUI;
    [Tooltip("Optional: drag the photo counter text here for live update")]
    public TMP_Text photoCounterText;

    [Header("Audio (optional)")]
    public AudioClip shutterSound;
    private AudioSource audioSource;

    private bool isInPhotoMode = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Make sure overlay starts hidden
        if (photoOverlayUI != null)
            photoOverlayUI.SetActive(false);

        UpdateCounterDisplay();
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Press P to toggle photo mode
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            TogglePhotoMode();
        }

        // Left click to take a photo (only while in photo mode)
        if (isInPhotoMode && Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            CapturePhoto();
        }
    }

    public void TogglePhotoMode()
    {
        isInPhotoMode = !isInPhotoMode;
        if (photoOverlayUI != null)
            photoOverlayUI.SetActive(isInPhotoMode);
    }

    void CapturePhoto()
    {
        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        // Hide overlay so it doesn't appear in the screenshot
        if (photoOverlayUI != null)
            photoOverlayUI.SetActive(false);

        // Wait one frame so the UI has actually been removed from rendering
        yield return new WaitForEndOfFrame();

        // Generate filename with timestamp
        string filename = "Photo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".png";
        string filePath = System.IO.Path.Combine(Application.dataPath, "..", filename);

        // Capture
        ScreenCapture.CaptureScreenshot(filename);
        Debug.Log("📸 Photo saved to: " + filePath);

        // Wait another frame for capture to complete
        yield return new WaitForEndOfFrame();

        // Restore overlay
        if (photoOverlayUI != null)
            photoOverlayUI.SetActive(true);

        // Increment counter (and play sound)
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.IncrementPhotoCount();

        if (shutterSound != null && audioSource != null)
            audioSource.PlayOneShot(shutterSound);

        UpdateCounterDisplay();
    }

    void UpdateCounterDisplay()
    {
        if (photoCounterText != null && GameStateManager.Instance != null)
            photoCounterText.text = "📸 Photos: " + GameStateManager.Instance.photosTaken;
    }
}