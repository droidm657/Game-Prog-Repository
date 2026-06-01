using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class GateInteraction : MonoBehaviour
{
    [Header("Gates")]
    public Animator leftGate;
    public Animator rightGate;

    [Header("Settings")]
    public float interactRange = 3f;
    public string openTrigger = "Open";

    private bool isLocked = true;
    private bool isOpen = false;

    public void UnlockGate()
    {
        isLocked = false;
        Debug.Log($"UnlockGate called! isLocked is now: {isLocked}");
    }

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed on gate!");
            TryOpen(playerTransform);
        }
    }

    private Transform playerTransform;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerTransform = other.transform;
            UIManager.Instance?.ShowPickupPrompt("Press E to open gate");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerTransform = null;
            UIManager.Instance?.HidePickupPrompt();
        }
    }

    public void TryOpen(Transform player)
    {
        Debug.Log($"TryOpen called! isLocked: {isLocked} isOpen: {isOpen}");

        if (isLocked)
        {
            Debug.Log("Still locked!");
            UIManager.Instance?.ShowPickupPrompt("Shoot the lock first!");
            return;
        }

        if (!isOpen)
        {
            Debug.Log("Opening gate!");
            OpenGate();
        }
    }

    void OpenGate()
    {
        isOpen = true;

        if (leftGate != null)
            leftGate.SetTrigger(openTrigger);
        if (rightGate != null)
            rightGate.SetTrigger(openTrigger);

        Debug.Log("Gates opening!");
        UIManager.Instance?.HidePickupPrompt();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
    void TriggerGate(Animator anim, string gateName)
    {
        if (anim == null)
        {
            Debug.Log($"{gateName} Animator is null!");
            return;
        }
        if (anim.runtimeAnimatorController == null)
        {
            Debug.Log($"{gateName} has no controller!");
            return;
        }
        Debug.Log($"Triggering Open on {gateName}");
        anim.SetTrigger("Open");
    }
}
