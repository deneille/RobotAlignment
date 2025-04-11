using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "ScriptableObjects/QuizData", order = 1)]
public class QuizData : ScriptableObject
{
    public string question;
    public bool correctAnswer; // True for correct, false for incorrect
    public float timeLimit; // Time limit for the quiz question
}
