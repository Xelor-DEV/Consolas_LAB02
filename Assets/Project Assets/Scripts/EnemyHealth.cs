using UnityEngine;

public class EnemyHealth : HealthManager
{
    [SerializeField] private EnemyAI enemyAI;

    public override void Death()
    {
        if (enemyAI != null && enemyAI.spawner != null)
        {
            enemyAI.spawner.ReportEnemyDeath();
        }

        Destroy(gameObject);
    }
}