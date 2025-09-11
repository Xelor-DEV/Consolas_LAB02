using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "VibrationConfig", menuName = "Tank/Vibration Configuration")]
public class VibrationConfig : ScriptableObject
{
    [Header("Vibration Settings")]
    [Range(0f, 1f)]
    public float lowFrequencyIntensity = 0.5f;
    [Range(0f, 1f)]
    public float highFrequencyIntensity = 0.5f;
    public float duration = 0.2f;

    private Dictionary<Gamepad, Coroutine> activeVibrations = new Dictionary<Gamepad, Coroutine>();
    private MonoBehaviour coroutineRunner;

    public void Initialize(MonoBehaviour runner)
    {
        coroutineRunner = runner;
        activeVibrations.Clear();
    }

    public void TriggerVibration(Gamepad gamepad)
    {
        if (gamepad == null || coroutineRunner == null) return;

        // Si ya hay una vibración activa para este gamepad, la detenemos
        if (activeVibrations.TryGetValue(gamepad, out Coroutine existingVibration))
        {
            coroutineRunner.StopCoroutine(existingVibration);
        }

        // Iniciamos una nueva vibración
        Coroutine newVibration = coroutineRunner.StartCoroutine(VibrationRoutine(gamepad));
        activeVibrations[gamepad] = newVibration;
    }

    public void StopAllVibrations()
    {
        foreach (var kvp in activeVibrations)
        {
            if (kvp.Value != null && coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(kvp.Value);
            }
            kvp.Key?.SetMotorSpeeds(0f, 0f);
        }
        activeVibrations.Clear();
    }

    public void StopVibration(Gamepad gamepad)
    {
        if (gamepad != null && activeVibrations.TryGetValue(gamepad, out Coroutine vibration))
        {
            if (coroutineRunner != null)
            {
                coroutineRunner.StopCoroutine(vibration);
            }
            gamepad.SetMotorSpeeds(0f, 0f);
            activeVibrations.Remove(gamepad);
        }
    }

    private IEnumerator VibrationRoutine(Gamepad gamepad)
    {
        gamepad.SetMotorSpeeds(lowFrequencyIntensity, highFrequencyIntensity);
        yield return new WaitForSeconds(duration);
        gamepad.SetMotorSpeeds(0f, 0f);

        // Removemos del diccionario cuando termina
        if (activeVibrations.ContainsKey(gamepad))
        {
            activeVibrations.Remove(gamepad);
        }
    }
}