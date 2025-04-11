using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRobotDialogue", menuName = "ScriptableObjects/RobotDialogue", order = 1)]
public class RobotDialogue : ScriptableObject
{
    public string[] dialogueLines;
    public float typingSpeed = 0.05f; // Speed at which the text is displayed
    public bool[] autoProgressLines;
    public float autoProgressDelay = 2f; // Delay before auto-progressing to the next line

}
