// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// // Add this component to UI elements you want to auto-register with GameManager
// public class UIElement : MonoBehaviour
// {
//     // The key to register this UI element with
//     public string registryKey;
    
//     private void Awake()
//     {
//         if (string.IsNullOrEmpty(registryKey))
//         {
//             // Use object name as default registry key
//             registryKey = gameObject.name;
//         }
        
//         // Register with GameManager
//         GameManager.RegisterUI(registryKey, gameObject);
//         Debug.Log($"UI Element '{gameObject.name}' registered with key '{registryKey}'");
//     }
    
//     private void OnDestroy()
//     {
//         // Optional: You could implement unregistering logic here if needed
//     }
// }