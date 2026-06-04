using UnityEngine;
using System.Collections;

public class DeathCameraEffect : MonoBehaviour
{
    public Camera playerCamera;

    public IEnumerator ZoomToMonster(Transform monster)
    {
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        Vector3 endPos =
            monster.position
            - monster.forward * 1.2f
            + Vector3.up * 1.5f;

        Quaternion endRot =
            Quaternion.LookRotation(monster.position - endPos);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 0.5f;

            playerCamera.transform.position =
                Vector3.Lerp(startPos, endPos, t);

            playerCamera.transform.rotation =
                Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }
    }
}