using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public List<string> keys = new List<string>();

    public bool HasKey(string keyID)
    {
        return keys.Contains(keyID);
    }

    public void AddKey(string keyID)
    {
        if (!keys.Contains(keyID))
        {
            keys.Add(keyID);

            Debug.Log("Picked up: " + keyID);
        }
    }
}