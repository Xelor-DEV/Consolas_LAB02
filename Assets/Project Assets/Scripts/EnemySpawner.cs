using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Config")]
    public GameObject enemyPrefab;
    public float spawnInterval = 5f; // Tiempo entre spawns (segundos)
    public float spawnRadius = 10f; // Radio del área de spawn
    public int initialSpawnCount = 1; // Cantidad inicial de enemigos


    [Header("NavMesh Config")]
    public float checkRadius = 10f; // Radio para buscar puntos en NavMesh
    public int maxAttempts = 30;

    [Header("Victory Conditions")]
    public int requiredKills = 10;
    public int currentKills = 0;

    [Header("Events")]
    public UnityEvent<int> OnKillsUpdated;
    public UnityEvent OnVictoryConditionMet;

    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;

    private void Start()
    {
        // Generar enemigos iniciales
        for (int i = 0; i < initialSpawnCount; i++)
        {
            SpawnSingleEnemy();
        }

        // Iniciar spawn periódico
        StartCoroutine(SpawnEnemiesRoutine());
    }

    void SpawnSingleEnemy()
    {
        Vector3 spawnPoint = GetRandomNavMeshPoint();

        if (spawnPoint != Vector3.zero)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation);
            EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.spawner = this;
            }
        }
        else
        {
            Debug.LogWarning("No se pudo encontrar un punto válido en el NavMesh para spawnear");
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        yield return new WaitForSeconds(spawnInterval);
        SpawnSingleEnemy();
        SpawnEnemiesRoutine();
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

    public void ReportEnemyDeath()
    {
        currentKills++;
        OnKillsUpdated?.Invoke(currentKills);

        if (currentKills >= requiredKills)
        {
            OnVictoryConditionMet?.Invoke();
        }
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