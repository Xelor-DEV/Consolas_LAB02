using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerCursor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image cursorImage;
    [SerializeField] private TMP_Text playerText;
    [SerializeField] private PlayerInput playerInput;

    private InputDevice device;
    private int playerNumber;
    private Color playerColor;

    private List<RectTransform> buttonPositions = new List<RectTransform>();
    private int currentPositionIndex = 0;
    private bool isReady = false;

    public int DeviceId => device.deviceId;

    public void Initialize(InputDevice device, int playerNumber, Color playerColor)
    {
        this.device = device;
        this.playerNumber = playerNumber;
        this.playerColor = playerColor;

        // Configurar apariencia
        cursorImage.color = playerColor;
        playerText.text = "Player " + playerNumber;
        playerText.color = playerColor;

        // Configurar input
        playerInput.SwitchCurrentControlScheme(device);

        // Obtener posiciones de botones del PlayerManager
        buttonPositions = PlayerManager.Instance.GetAllButtonPositions();

        // Posicionar el cursor en el primer botón disponible al inicio
        if (buttonPositions.Count > 0)
        {
            currentPositionIndex = 0;
            MoveToPosition(buttonPositions[currentPositionIndex]); // Usar DOTween para moverlo suavemente
        }
    }
    // Estos métodos serán llamados por los eventos de Unity del PlayerInput
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (isReady) return;

        Vector2 direction = context.ReadValue<Vector2>();

        if (direction.x > 0.5f)
        {
            // Mover a la derecha
            currentPositionIndex = (currentPositionIndex + 1) % buttonPositions.Count;
            MoveToPosition(buttonPositions[currentPositionIndex]);
        }
        else if (direction.x < -0.5f)
        {
            // Mover a la izquierda
            currentPositionIndex--;
            if (currentPositionIndex < 0) currentPositionIndex = buttonPositions.Count - 1;
            MoveToPosition(buttonPositions[currentPositionIndex]);
        }
    }

    private void MoveToPosition(RectTransform target)
    {
        // Usar DOTween para animar el movimiento
        cursorImage.rectTransform.DOMove(target.position, 0.2f).SetEase(Ease.OutBack);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (isReady || !context.performed) return;

        // Intentar seleccionar la posición actual
        int tankNumber;
        bool isDriver;

        bool success = PlayerManager.Instance.TrySelectPosition(
            currentPositionIndex,
            playerNumber,
            out tankNumber,
            out isDriver
        );

        if (success)
        {
            Debug.Log($"Player {playerNumber} seleccionó {(isDriver ? "Driver" : "Gunner")} del Tank {tankNumber}");

            // Marcar como listo y cambiar apariencia
            isReady = true;
            cursorImage.DOColor(new Color(playerColor.r, playerColor.g, playerColor.b, 0.5f), 0.3f);

            // Notificar al PlayerManager que este jugador está listo
            PlayerManager.Instance.PlayerReady();
        }
        else
        {
            Debug.Log("Esta posición ya está ocupada");
            // Puedes agregar una animación de rechazo aquí
        }
    }
}