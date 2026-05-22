using UnityEngine;
using TMPro;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI instance;

    [Header("UI")]
    public GameObject interactionPanel;

    public TMP_Text interactionText;

    void Awake()
    {
        instance = this;

        HideInteraction();
    }

    public void ShowInteraction(string message)
    {
        interactionPanel.SetActive(true);

        interactionText.text = message;
    }

    public void HideInteraction()
    {
        interactionPanel.SetActive(false);
    }
}