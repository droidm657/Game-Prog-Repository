using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Lock : MonoBehaviour
{
    [Header("Lock Settings")]
    public int hitsToBreak = 1;
    public AudioSource breakSound;

    [Header("Gates")]
    public GameObject leftGate;   // Gate.001
    public GameObject rightGate;  // Gate.002

    private int currentHits = 0;

    public void ShootLock()
    {
        currentHits++;
        Debug.Log($"Lock hit! {currentHits}/{hitsToBreak}");

        if (currentHits >= hitsToBreak)
            BreakLock();
    }

    void BreakLock()
    {
        Debug.Log("Lock broken!");

        if (breakSound != null)
            breakSound.Play();

        // Notify gate dialogue
        GateDialogueTrigger trigger = FindAnyObjectByType<GateDialogueTrigger>();
        if (trigger != null)
            trigger.OnLockBroken();

        // Notify gate interact
        GateInteraction gate = FindAnyObjectByType<GateInteraction>();
        if (gate != null)
            gate.UnlockGate();

        Destroy(gameObject, 0.1f);
    }

    void TriggerGate(GameObject gate)
    {
        if (gate == null) return;

        Animator anim = gate.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("Open");
        else
            Debug.LogWarning($"No Animator found on {gate.name}!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
