using System.Collections;
using UnityEngine;

// Spawns firework particle prefabs at random positions above the castle
// on a timer. Plays delayed boom audio.
public class FireworksLauncher : MonoBehaviour
{
    [Header("Firework Prefabs")]
    [Tooltip("Drag one or more firework particle prefabs here")]
    public GameObject[] fireworkPrefabs;

    [Header("Spawn Area (relative to this GameObject)")]
    public Vector3 spawnAreaSize = new Vector3(15f, 5f, 10f);

    [Header("Timing")]
    public float minInterval = 3f;
    public float maxInterval = 7f;
    [Tooltip("Wait this many seconds after scene starts before first firework")]
    public float startDelay = 5f;
    [Tooltip("How long until each spawned firework is destroyed")]
    public float fireworkLifetime = 5f;

    [Header("Audio")]
    public AudioClip[] boomSounds;
    [Tooltip("Delay between visual and sound — fakes speed-of-sound")]
    public float audioDelaySeconds = 0.4f;
    [Range(0f, 1f)]
    public float boomVolume = 0.7f;

    void Start()
    {
        StartCoroutine(LaunchLoop());
    }

    IEnumerator LaunchLoop()
    {
        // Initial wait before first firework
        yield return new WaitForSeconds(startDelay);

        while (true)
        {
            SpawnFirework();
            float wait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnFirework()
    {
        if (fireworkPrefabs == null || fireworkPrefabs.Length == 0) return;

        // Pick random prefab
        GameObject prefab = fireworkPrefabs[Random.Range(0, fireworkPrefabs.Length)];

        // Pick random position within spawn area, relative to this GameObject's position
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );
        Vector3 spawnPos = transform.position + randomOffset;

        // Instantiate the firework
        GameObject fw = Instantiate(prefab, spawnPos, Quaternion.identity);
        Destroy(fw, fireworkLifetime);

        // Play delayed boom
        StartCoroutine(DelayedBoom(spawnPos));
    }

    IEnumerator DelayedBoom(Vector3 pos)
    {
        yield return new WaitForSeconds(audioDelaySeconds);

        if (boomSounds != null && boomSounds.Length > 0)
        {
            AudioClip clip = boomSounds[Random.Range(0, boomSounds.Length)];
            AudioSource.PlayClipAtPoint(clip, pos, boomVolume);
        }
    }

    // Visualize the spawn area in the Scene view (helpful for placement)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}