using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AmmoSystem : MonoBehaviour
{
    public static AmmoSystem Instance;

    private int currentAmmo = 0;

    void start()
    {
        currentAmmo = 6;
        Debug.Log($"Starting Ammo: {currentAmmo}");
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddAmmo(int amount)
    {
        currentAmmo += amount;
        Debug.Log($"Ammo: {currentAmmo}");
    }

    public bool UseAmmo(int amount)
    {
        if (currentAmmo >= amount)
        {
            currentAmmo -= amount;
            Debug.Log($"Ammo remaining: {currentAmmo}");
            return true;
        }
        Debug.Log("Out of ammo!");
        return false;
    }

    public int GetAmmo() => currentAmmo;
}
