using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI output;  // Reference to output text for general messages

    [SerializeField] private Button actionButton;  // The single action button for spinning and skipping
    [SerializeField] private TextMeshProUGUI actionButtonText;  // Reference to the text component of the action button

    [Header("Claim Popup UI")]
    [SerializeField] private GameObject claimPopup;  // Reference to the popup panel
    [SerializeField] private TextMeshProUGUI popupContentName;  // Reference to the text in the popup
    [SerializeField] private Image popupContentIcon;  // Reference to the icon in the popup
    [SerializeField] private Button claimButton;  // Button to claim the reward from the popup

    [Header("Inventory UI")]
    [SerializeField] private TextMeshProUGUI inventoryOutput;  // UI element to display inventory

    private void Start()
    {
        // Register button listeners
        actionButton.onClick.AddListener(OnActionButtonClicked);
        claimButton.onClick.AddListener(OnClaimButtonClicked);

        // Ensure claim popup is hidden at start
        claimPopup.SetActive(false);

        // Set the initial text for the action button
        actionButtonText.text = "Play";
    }

    public void UpdateOutput(string message)
    {
        output.text = message;
    }

    public void UpdateInventoryUI(List<string> rewards)
    {
        inventoryOutput.text = "Inventory:\n";
        foreach (var reward in rewards)
        {
            inventoryOutput.text += reward + "\n";
        }
    }

    public void ShowClaimPopup(WheelContent content)
    {
        popupContentName.text = content.contentName;
        popupContentIcon.sprite = content.icon;
        claimPopup.SetActive(true);
    }

    private void OnActionButtonClicked()
    {
        Wheel activeWheel = GameManager.Instance.GetActiveWheel();

        if (activeWheel != null && activeWheel.IsSpinning())
        {
            // If the wheel is spinning, skip to the end
            activeWheel.StartSkip();
        }
        else
        {
            // If the wheel is not spinning, start spinning
            GameManager.Instance.NotifyObservers("Spin");
            Debug.Log("Action button clicked to spin");
            // Change the button text to "Skip"
            actionButtonText.text = "Skip";
        }
    }

    private void OnClaimButtonClicked()
    {
        // Logic for claim button click
        Debug.Log("Claim button clicked");
        ClaimReward();
        AudioManager.Instance.PlaySFX("Claim");

        // Hide claim popup after claiming
        claimPopup.SetActive(false);

        // Reset the action button text to "Play"
        actionButtonText.text = "Play";
    }

    private void ClaimReward()
    {
        // Get the current reward from the active wheel
        Wheel activeWheel = GameManager.Instance.GetActiveWheel();
        if (activeWheel != null)
        {
            string reward = activeWheel.GetCurrentReward();
            Debug.Log("Claimed reward: " + reward);

            // Add the claimed reward to the inventory
            Inventory.Instance.AddReward(reward);

            // Update the inventory UI
            UpdateInventoryUI(Inventory.Instance.GetRewards());
        }
    }
}
