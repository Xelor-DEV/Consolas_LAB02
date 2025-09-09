using UnityEngine;

public enum PowerUpType
{
    None,
    Ammo,
    Speed
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType;

    public PowerUpType Type
    {
        get { return powerUpType; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch (powerUpType)
            {
                case PowerUpType.Speed:
                    TankMovement movement = other.GetComponent<TankMovement>();
                    if (movement != null)
                    {
                        movement.ActivateSpeedBoost();
                    }
                    break;

                case PowerUpType.Ammo:
                    TurretControl turret = other.GetComponentInChildren<TurretControl>();
                    if (turret != null)
                    {
                        turret.AddAmmo(turret.AmmoPerPowerUp);
                    }
                    break;
            }

            // Destruir el powerup después de recogerlo
            Destroy(gameObject);
        }
    }
}