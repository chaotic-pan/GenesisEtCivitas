using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    [Header("Tree Prefabs")]
    [SerializeField] private GameObject[] hotTreePrefabs;
    [SerializeField] private GameObject[] warmTreePrefabs;
    [SerializeField] private GameObject[] normalTreePrefabs;
    [SerializeField] private GameObject[] freshTreePrefabs;
    [SerializeField] private GameObject[] coldTreePrefabs;
    [Space]
    [Tooltip("Needs to be uneven")]
    [SerializeField] private int treeSpawnDistance = 5;
    [SerializeField] private float minTreeSizeFactor;
    [SerializeField] private float maxTreeSizeFactor;

    // from 135 (0 degree) to 255 in steps of 24
    private const int ColdTreeMaxTemp = 159;
    private const int FreshTreeMaxTemp = 183;
    private const int NormalTreeMaxTemp = 207;
    private const int WarmTreeMaxTemp = 231;

    public void GenerateTrees(int points, int[,] vegetation, int[,] climate, float[,]heightMap, float mapHeightMultiplier, int chunkSize)
    {
        var trees = GameObject.Find("Trees");
        if (trees != null) DestroyImmediate(trees);
        trees = new GameObject("Trees");
        
        var halfTreeSpawnDistance = treeSpawnDistance / 2;
        var treeSpawnDistanceSquared = treeSpawnDistance * treeSpawnDistance;
        var mapSize = points;
        
        var vegetationMap = vegetation;
        var temperatureMap = climate;
        for (var x = halfTreeSpawnDistance; x < mapSize; x += treeSpawnDistance)
        {
            for (var y = halfTreeSpawnDistance; y < mapSize; y += treeSpawnDistance)
            {
                var surroundingVegetationSum = 0;

                for (var checkX = x - halfTreeSpawnDistance; checkX < x + halfTreeSpawnDistance; checkX++)
                {
                    for (var checkY = y - halfTreeSpawnDistance; checkY < y + halfTreeSpawnDistance; checkY++)
                    {
                        surroundingVegetationSum += vegetationMap[checkX, checkY];    
                    }
                }
                var vegetationAverage = surroundingVegetationSum / treeSpawnDistanceSquared;
                var vegetationValue = Mathf.InverseLerp(0, 15, vegetationAverage);
                if (Random.Range(0.3f, 1f) > vegetationValue)
                    continue;

                var treeWorldX = x + Random.Range(-1, 1);
                var treeWorldZ = -y + Random.Range(-1, 1);
                if (vegetationMap[treeWorldX, - treeWorldZ] <= 1)
                    continue;
                
                var treeWorldPosition = new Vector3(treeWorldX, heightMap[x, y] * mapHeightMultiplier, treeWorldZ) - new Vector3(0.5f, 0, -0.5f) * chunkSize;
                var temperature = temperatureMap[treeWorldX, -treeWorldZ];
                var treePrefab = temperature switch
                {
                    <= ColdTreeMaxTemp => coldTreePrefabs[Random.Range(0, coldTreePrefabs.Length)],
                    <= FreshTreeMaxTemp => freshTreePrefabs[Random.Range(0, freshTreePrefabs.Length)],
                    <= NormalTreeMaxTemp => normalTreePrefabs[Random.Range(0, normalTreePrefabs.Length)],
                    <= WarmTreeMaxTemp => warmTreePrefabs[Random.Range(0, warmTreePrefabs.Length)],
                    _ => hotTreePrefabs[Random.Range(0, hotTreePrefabs.Length)]
                };
                var newTree = Instantiate(treePrefab, treeWorldPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                newTree.transform.localScale *= Random.Range(minTreeSizeFactor, maxTreeSizeFactor);
                newTree.transform.SetParent(trees.transform);
            }
        }
    }

    public void GrowTreeAbility(Vector2Int point, int range, int[,] vegetationMap, int[,] temperatureMap, float[,]heightMap, float mapHeightMultiplier, int chunkSize)
    {
      var trees = GameObject.Find("Trees");;
        
        var halfTreeSpawnDistance = treeSpawnDistance / 2;
        var treeSpawnDistanceSquared = treeSpawnDistance * treeSpawnDistance;
        
        for (var x = point.x-range+halfTreeSpawnDistance; x < point.x+range; x += treeSpawnDistance)
        {
            for (var y = point.y-range+halfTreeSpawnDistance; y < point.y+range; y += treeSpawnDistance)
            {
                var dis = Vector2.Distance(point, new Vector2(x, y));
                if (dis > range) continue;
                var vegetationValue = Mathf.InverseLerp(50, -150, dis);
                if (Random.Range(0, 1f) > vegetationValue)
                    continue;

                var treeWorldX = x + Random.Range(-1, 1);
                var treeWorldZ = -y + Random.Range(-1, 1);

                var treeWorldPosition = new Vector3(treeWorldX, heightMap[x, y] * mapHeightMultiplier, treeWorldZ) - new Vector3(0.5f, 0, -0.5f) * chunkSize;
                var temperature = temperatureMap[treeWorldX, -treeWorldZ];
                var treePrefab = temperature switch
                {
                    <= ColdTreeMaxTemp => coldTreePrefabs[Random.Range(0, coldTreePrefabs.Length)],
                    <= FreshTreeMaxTemp => freshTreePrefabs[Random.Range(0, freshTreePrefabs.Length)],
                    <= NormalTreeMaxTemp => normalTreePrefabs[Random.Range(0, normalTreePrefabs.Length)],
                    <= WarmTreeMaxTemp => warmTreePrefabs[Random.Range(0, warmTreePrefabs.Length)],
                    _ => hotTreePrefabs[Random.Range(0, hotTreePrefabs.Length)]
                };
                var newTree = Instantiate(treePrefab, treeWorldPosition, Quaternion.Euler(0, Random.Range(0, 360), 0), transform);
                newTree.transform.localScale *= Random.Range(minTreeSizeFactor, maxTreeSizeFactor);
                newTree.transform.SetParent(trees.transform);
            }
        }
    }

    
    private void OnValidate()
    {
        if (treeSpawnDistance % 2 == 0)
            treeSpawnDistance = 5;
    }
}