using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform spawnPoint;
    public float timeBetweenWaves = 5f;
    float stopwatchForWaves = 0;
    private int waveNumber = 0;


    void Update()
    {
        stopwatchForWaves += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.N) || stopwatchForWaves > timeBetweenWaves) // Extra: Summon next wave early
        {
            stopwatchForWaves = 0;
            StartCoroutine(SpawnWave());
        }
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;
        for (int i = 0; i < waveNumber; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], spawnPoint.position, spawnPoint.rotation);
    }
}