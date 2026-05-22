using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door")]
    public string requiredKeyID;

    public bool isLocked = true;

    public Transform doorPivot;

    public float openAngle = 90f;

    public float openSpeed = 2f;

    private Quaternion closedRotation;

    private Quaternion openRotation;

    private bool isOpen = false;

    private bool playerNearby = false;

    void Start()
    {
        closedRotation = doorPivot.rotation;

        openRotation =
            Quaternion.Euler(
                doorPivot.eulerAngles +
                new Vector3(0, openAngle, 0)
            );
    }

    void Update()
    {
        if (playerNearby &&
            Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }

        // Animate Door
        if (isOpen)
        {
            doorPivot.rotation =
                Quaternion.Slerp(
                    doorPivot.rotation,
                    openRotation,
                    Time.deltaTime * openSpeed
                );
        }
        else
        {
            doorPivot.rotation =
                Quaternion.Slerp(
                    doorPivot.rotation,
                    closedRotation,
                    Time.deltaTime * openSpeed
                );
        }
    }

    void TryOpenDoor()
    {
        if (isLocked)
        {
            PlayerInventory inventory =
                FindObjectOfType<PlayerInventory>();

            if (inventory.HasKey(requiredKeyID))
            {
                isLocked = false;

                Debug.Log("Door unlocked!");
            }
            else
            {
                Debug.Log("Door locked.");
                return;
            }
        }

        isOpen = !isOpen;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            if (isLocked)
            {
                InteractionUI.instance.ShowInteraction(
                    "Press E to unlock door"
                );
            }
            else
            {
                InteractionUI.instance.ShowInteraction(
                    "Press E to open door"
                );
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            InteractionUI.instance.HideInteraction();
        }
    }
}