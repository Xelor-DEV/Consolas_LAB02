using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class PowerUpSpawner : MonoBehaviour
{
    public GameObject powerUpPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 5f;
    public int maxSpawnAttempts = 30;

    private void Start()
    {
        StartCoroutine(SpawnPowerUpsRoutine());
    }

    private IEnumerator SpawnPowerUpsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            TrySpawnPowerUp();
        }
    }

    private void TrySpawnPowerUp()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, spawnRadius, NavMesh.AllAreas))
        {
            Instantiate(powerUpPrefab, hit.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibuja la esfera del área de spawn
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        // Dibuja una esfera más pequeña en el centro como referencia
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}