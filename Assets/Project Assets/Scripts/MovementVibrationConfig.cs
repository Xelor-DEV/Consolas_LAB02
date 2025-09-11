using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MovementVibrationConfig", menuName = "Tank/Movement Vibration Configuration")]
public class MovementVibrationConfig : ScriptableObject
{
    [Header("Movement Vibration Settings")]
    [Range(0f, 1f)]
    public float idleLowFrequency = 0f;
    [Range(0f, 1f)]
    public float idleHighFrequency = 0f;
    [Range(0f, 1f)]
    public float movingLowFrequency = 0.2f;
    [Range(0f, 1f)]
    public float movingHighFrequency = 0.1f;
    [Range(0f, 1f)]
    public float runningLowFrequency = 0.4f;
    [Range(0f, 1f)]
    public float runningHighFrequency = 0.2f;

    public float transitionSmoothness = 2f;

    // Método simple para obtener los valores de vibración según el estado
    public (float low, float high) GetVibrationValues(bool isMoving, bool isRunning)
    {
        if (!isMoving) return (idleLowFrequency, idleHighFrequency);
        return isRunning ? (runningLowFrequency, runningHighFrequency) : (movingLowFrequency, movingHighFrequency);
    }
}