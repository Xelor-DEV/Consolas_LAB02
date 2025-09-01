using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TankSelector : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private RectTransform tankButton;
    [SerializeField] private RectTransform turretButton;
    [SerializeField] private TMP_Text tankText;

    private int tankNumber;
    private bool isDriverSelected = false;
    private bool isGunnerSelected = false;

    public RectTransform TankButtonRect => tankButton;
    public RectTransform TurretButtonRect => turretButton;
    public bool IsDriverSelected => isDriverSelected;
    public bool IsGunnerSelected => isGunnerSelected;

    public void Initialize(int tankNumber)
    {
        this.tankNumber = tankNumber;
        tankText.text = "Tank " + tankNumber;
    }

    public void SetDriverSelected()
    {
        isDriverSelected = true;
    }

    public void SetGunnerSelected()
    {
        isGunnerSelected = true;
    }

    public void ResetSelection()
    {
        isDriverSelected = false;
        isGunnerSelected = false;
    }
}