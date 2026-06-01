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

    public ItemData shotgunItem;

    // Call this from Lock.cs when lock breaks
    public void OnLockBroken()
    {
        lockBrokenTriggered = false; // reset so it triggers again
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > triggerDistance) return;
        if (DialogueSystem.Instance.IsDialogueOpen()) return;

        bool hasShotgun = InventorySystem.Instance.HasItem(shotgunItem);

        if (hasShotgun && !lockBrokenTriggered)
        {
            // Lock just broken dialogue
            if (!lockBrokenTriggered)
            {
                lockBrokenTriggered = true;
                DialogueSystem.Instance.StartDialogue(lockBrokenLines);
            }
        }
        else if (hasShotgun && !hasShotgunTriggered)
        {
            hasShotgunTriggered = true;
            DialogueSystem.Instance.StartDialogue(hasShotgunLines);
        }
        else if (!hasShotgun && !noShotgunTriggered)
        {
            noShotgunTriggered = true;
            DialogueSystem.Instance.StartDialogue(noShotgunLines);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}