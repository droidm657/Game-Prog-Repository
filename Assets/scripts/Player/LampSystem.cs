using UnityEngine;

public class LampSystem : MonoBehaviour
{
    public Light lampLight;

    private bool isOn = false;

    void Start()
    {
        if (lampLight != null)
            lampLight.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("F pressed on lamp");

            isOn = !isOn;

            if (lampLight != null)
            {
                lampLight.enabled = isOn;
                Debug.Log("Lamp state: " + isOn);
            }
            else
            {
                Debug.Log("Lamp Light is NULL!");
            }
        }
    }
}