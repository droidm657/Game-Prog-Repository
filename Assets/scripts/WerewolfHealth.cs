using UnityEngine;

public class WerewolfHealth : MonoBehaviour
{
    public BasicMonsterAI monsterAI;

    [Header("Shotgun Stun")]
    public float shotgunStunDuration = 5f;

    private void Start()
    {
        if (monsterAI == null)
        {
            monsterAI = GetComponent<BasicMonsterAI>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (monsterAI != null)
        {
            monsterAI.StunMonster(
                shotgunStunDuration
            );
        }

        Debug.Log(
            "Werewolf stunned by shotgun!"
        );
    }
}
