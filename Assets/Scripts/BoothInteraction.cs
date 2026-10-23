using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

// Attach this to the booth GameObject (Icecreamcart).
// Detects when player is nearby, shows a prompt, and handles E key purchase.
public class BoothInteraction : MonoBehaviour
{
    [Header("Pricing")]
    public int price = 45;

    [Header("References")]
    [Tooltip("Drag the player GameObject (or PlayerCapsule) here")]
    public Transform playerTransform;
    [Tooltip("Drag the PromptCanvas GameObject here")]
    public GameObject promptCanvas;
    [Tooltip("Drag the PromptText (TextMeshPro) here")]
    public TMP_Text promptText;
    [Tooltip("Drag the held ice cream GameObject (the one parented to the camera) here")]
    public GameObject heldIceCream;

    [Header("Settings")]
    public float interactionRange = 3f;

    [Header("Audio (optional)")]
    public AudioClip purchaseSound;
    public AudioClip failSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Make sure prompt and ice cream start hidden
        if (promptCanvas != null) promptCanvas.SetActive(false);
        if (heldIceCream != null) heldIceCream.SetActive(false);
    }

    void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool inRange = distance < interactionRange;

        // Show or hide the prompt based on proximity
        if (promptCanvas != null)
            promptCanvas.SetActive(inRange);

        if (!inRange) return;

        // Update prompt text based on state
        bool alreadyHas = GameStateManager.Instance != null &&
                          GameStateManager.Instance.GetFlag("hasIceCream");
        bool canAfford = GameStateManager.Instance != null &&
                 GameStateManager.Instance.balance >= price;

Debug.Log("BOOTH SEES: Balance=" + (GameStateManager.Instance != null ? GameStateManager.Instance.balance.ToString() : "NULL") + " | Price=" + price + " | CanAfford=" + canAfford);

        if (promptText != null)
        {
            if (alreadyHas)
                promptText.text = "Already purchased!";
            else if (!canAfford)
                promptText.text = "Insufficient balance (¥" + price + ")";
            else
                promptText.text = "Press E to buy ice cream — ¥" + price;
        }

        // Listen for E key with new Input System
        if (!alreadyHas && canAfford &&
            Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Purchase();
        }
    }

    void Purchase()
    {
        if (GameStateManager.Instance == null) return;

        bool success = GameStateManager.Instance.SpendBalance(price);
        if (!success)
        {
            if (failSound != null) audioSource.PlayOneShot(failSound);
            return;
        }

        GameStateManager.Instance.SetFlag("hasIceCream", true);

        if (heldIceCream != null) heldIceCream.SetActive(true);
        if (purchaseSound != null) audioSource.PlayOneShot(purchaseSound);

        Debug.Log("Bought ice cream! Balance: ¥" + GameStateManager.Instance.balance);
    }
}