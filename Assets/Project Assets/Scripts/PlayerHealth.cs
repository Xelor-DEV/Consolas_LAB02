using UnityEngine;

public class PlayerHealth : HealthManager
{
    public override void Death()
    {
        TankManager tankManager = GetComponent<TankManager>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (tankManager != null)
        {
            tankManager.DisableTankControls();
            rb.isKinematic = true;

            GameManager.Instance.OnTankDestroyed(tankManager.TankNumber);
        }
    }
}
