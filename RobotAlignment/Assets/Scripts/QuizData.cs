using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "ScriptableObjects/QuizData", order = 1)]
public class QuizData : ScriptableObject
{
    public string [] questions; // Array of quiz questions
    public bool [] answers; // Array of answers corresponding to the questions
    public float timeLimit; // Time limit for the quiz question
}
