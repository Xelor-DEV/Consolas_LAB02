using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Config")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f; // Tiempo entre spawns (segundos)
    public float spawnRadius = 10f; // Radio del área de spawn

    [Header("NavMesh Config")]
    public float checkRadius = 10f; // Radio para buscar puntos en NavMesh
    public int maxAttempts = 30; 

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        Vector3 spawnPoint = GetRandomNavMeshPoint();

        if (spawnPoint != Vector3.zero)
        {
            Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
        }
        else
        {
            Debug.LogWarning("No se pudo encontrar un punto válido en el NavMesh para spawnear");
        }

        yield return new WaitForSeconds(spawnInterval);

        StartCoroutine(SpawnEnemies());
    }

    Vector3 GetRandomNavMeshPoint()
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * spawnRadius;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, checkRadius, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if(showGizmos == false)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}