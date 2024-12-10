using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs; // ������ ������ ������ �迭
    public BoxCollider spawnArea;     // ���� ������ ����� Box Collider
    public int trashCount = 15;       // ������ ������ ����

    void Start()
    {
        SpawnTrash();
    }

    void SpawnTrash()
    {
        for (int i = 0; i < trashCount; i++)
        {
            Vector3 randomPosition = GetRandomPositionInArea();
            GameObject randomTrash = GetRandomTrashPrefab();
            Instantiate(randomTrash, randomPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPositionInArea()
    {
        Vector3 center = spawnArea.bounds.center;
        Vector3 size = spawnArea.bounds.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);
        float y = center.y + 0.2f; // ���� ���̸� ����

        return new Vector3(x, y, z);
    }

    GameObject GetRandomTrashPrefab()
    {
        int randomIndex = Random.Range(0, trashPrefabs.Length);
        return trashPrefabs[randomIndex];
    }
}
