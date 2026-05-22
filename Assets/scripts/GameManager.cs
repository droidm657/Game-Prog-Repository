using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI objectiveText;

    [Header("Fuel System")]
    public int fuelCollected = 0;
    public int requiredFuel = 3;

    [Header("Generator")]
    public bool generatorFound = false;
    public bool generatorPowered = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        objectiveText.text =
            "OBJECTIVE:\nFind the Generator";
    }

    public void FoundGenerator()
    {
        generatorFound = true;

        objectiveText.text =
            "OBJECTIVE:\nFind Fuel Canisters (" +
            fuelCollected +
            "/" +
            requiredFuel +
            ")";
    }

    public void CollectFuel()
    {
        fuelCollected++;

        objectiveText.text =
            "OBJECTIVE:\nFind Fuel Canisters (" +
            fuelCollected +
            "/" +
            requiredFuel +
            ")";

        if (fuelCollected >= requiredFuel)
        {
            objectiveText.text =
                "OBJECTIVE:\nReturn to Generator";
        }
    }

    public bool HasEnoughFuel()
    {
        return fuelCollected >= requiredFuel;
    }

    public void ActivateGenerator()
    {
        generatorPowered = true;

        objectiveText.text =
            "OBJECTIVE:\nTurn On Generator Switch";
    }

    public void ActivateSwitch()
    {
        objectiveText.text =
            "POWER RESTORED!";
    }
}