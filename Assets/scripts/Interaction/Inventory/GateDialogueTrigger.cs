using UnityEngine;
using Debug = UnityEngine.Debug;

public class GateDialogueTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public float triggerDistance = 8f;
    public Transform player;

    [Header("Dialogue Without Shotgun")]
    [TextArea(2, 5)]
    public string[] noShotgunLines = new string[]
    {
        "I remember bringing a shotgun at the back of the truck...",
        "I need to find it.",
    };

    [Header("Dialogue With Shotgun - Lock Intact")]
    [TextArea(2, 5)]
    public string[] hasShotgunLines = new string[]
    {
        "Let's try busting the lock with this.",
    };

    [Header("Dialogue With Shotgun - Lock Broken")]
    [TextArea(2, 5)]
    public string[] lockBrokenLines = new string[]
    {
        "The lock is broken... I can push it open now.",
    };

    private bool noShotgunTriggered = false;
    private bool hasShotgunTriggered = false;
    private bool lockBrokenTriggered = false;

    // NEW FLAG: Tracks if the lock has actually been shot and destroyed
    private bool isLockBroken = false;

    public ItemData shotgunItem;

    // Call this from Lock.cs when the lock breaks
    public void OnLockBroken()
    {
        isLockBroken = true;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > triggerDistance) return;
        if (DialogueSystem.Instance.IsDialogueOpen()) return;

        bool hasShotgun = InventorySystem.Instance.HasItem(shotgunItem);

        // PRIORITY 1: The lock is physically destroyed
        if (isLockBroken)
        {
            if (!lockBrokenTriggered)
            {
                lockBrokenTriggered = true;
                DialogueSystem.Instance.StartDialogue(lockBrokenLines);
            }
        }
        // PRIORITY 2: Lock is still intact, but the player holds the shotgun
        else if (hasShotgun)
        {
            if (!hasShotgunTriggered)
            {
                hasShotgunTriggered = true;
                DialogueSystem.Instance.StartDialogue(hasShotgunLines);
            }
        }
        // PRIORITY 3: Lock is intact and player has no shotgun
        else if (!hasShotgun)
        {
            if (!noShotgunTriggered)
            {
                noShotgunTriggered = true;
                DialogueSystem.Instance.StartDialogue(noShotgunLines);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}