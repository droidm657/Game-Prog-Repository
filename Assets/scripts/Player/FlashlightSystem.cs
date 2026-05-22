using UnityEngine;

public class FlashlightSystem : MonoBehaviour
{
    [Header("Flashlight")]
    public GameObject flashlightObject;

    public Light flashlightLight;

    private bool isOn = true;

    void Start()
    {
        SetFlashlight(isOn);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;

            SetFlashlight(isOn);
        }
    }

    void SetFlashlight(bool state)
    {
        flashlightObject.SetActive(state);

        flashlightLight.enabled = state;
    }
}