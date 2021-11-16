using EW2;
using UnityEngine;

public class SpawnBoss : MonoBehaviour
{
    [SerializeField] private int bossId;
    [SerializeField] private Transform posSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        GamePlayController.Instance.SpawnController.SpawnEnemy(bossId, posSpawn.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
