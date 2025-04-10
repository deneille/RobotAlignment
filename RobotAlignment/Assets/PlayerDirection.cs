using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirection : MonoBehaviour
{
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (input.x > 0) // Right
        {
            transform.rotation = Quaternion.Euler(0, 0, 90); // Facing right
        }
        else if (input.x < 0) // Left
        {
            transform.rotation = Quaternion.Euler(0, 0, -90); // Facing left
        }
        else if (input.y > 0) // Up
        {
            transform.rotation = Quaternion.Euler(0, 0, 180); // Facing up
        }
        else if (input.y < 0) // Down (default)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Facing down
        }
    }
}

