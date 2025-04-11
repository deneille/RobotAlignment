using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryInventory : MonoBehaviour
{
    public int maxHits = 2;

    private int currentHits = 0;

    public void AddHit(){
        currentHits += 1;
        Debug.Log("Hit added. Current hits: " + currentHits);
        // Check if the current hits exceed the maximum hits
        if (currentHits >= maxHits){
            Destroy(gameObject);
            Debug.Log("Factory destroyed.");
        }
    }
    public int GetHitCount()
    {
        return currentHits;
    }
}
