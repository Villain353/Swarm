using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    public GameObject swarmAgentPrefab;
    public int swarmSize = 100;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < swarmSize; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            GameObject swarmAgent = Instantiate(swarmAgentPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }
    
    
}
