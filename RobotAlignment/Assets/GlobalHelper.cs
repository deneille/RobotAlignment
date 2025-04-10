using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalHelper
{
    public static string GetUniqueId(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}"; // Generate a unique ID using GUID
    }
}
