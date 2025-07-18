using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;
using Random = UnityEngine.Random;

public class Vegetation : MonoBehaviour
{
    [SerializeField] private Mesh[] simplePlants;
    [SerializeField] private Material[] simplePlantMaterials;
    
    private TileManager _tileManager;
    private MapExtractor _mapExtractor;
    
    private Dictionary<Vector3Int, TileData> _subsetDict;
    private Dictionary<Vector3Int, List<VegData>> _tileMatrixDict;
    private Dictionary<(int, int), List<Matrix4x4>> _vegMatrices;

    private Vector3 _scale = new (300, 300, 300);
    private int _maxTileVegetationHalfed = 3;
    
    private void Start()
    {
        _vegMatrices = new Dictionary<(int, int), List<Matrix4x4>>();
        _tileMatrixDict  = new Dictionary<Vector3Int, List<VegData>>();
        
        _tileManager = TileManager.Instance;
        _mapExtractor = MapExtractor.Instance;
        
        GameEvents.Lifecycle.OnTileManagerFinishedInitializing += UpdateVegetation;
    }

    private void UpdateVegetation()
    {
        var subset = _tileManager.GetAllTileDataDict();
        _subsetDict = subset.Where(tile => !tile.Value.isWater).ToDictionary(tile => tile.Key, tile => tile.Value);
        
        foreach (var tileKey in _subsetDict.Keys)
        {
            var tile = _subsetDict[tileKey];
            var vegetation = tile.vegetation;
            var _currentVegetationHalfed = Mathf.Ceil(vegetation / 4);
           
            if (vegetation == 0) continue;
            
            var mat = 0;

            if (tile.landFertility >= 12)
            {
                mat = 3;
            } else if (tile.landFertility >= 8 && tile.landFertility < 12)
            {
                mat = 2;
            } else if (tile.landFertility >= 4 && tile.landFertility < 8)
            {
                mat = 1;
            }
            
            
            var currentVegetationCount = 0;

            if (_tileMatrixDict.ContainsKey(tileKey))
                currentVegetationCount = Mathf.FloorToInt(_tileMatrixDict[tileKey].Count / 2);
            
            
            if (!_tileMatrixDict.ContainsKey(tileKey))
                _tileMatrixDict.Add(tileKey, new List<VegData>());

            if (currentVegetationCount < _maxTileVegetationHalfed)
            {
                var diff = _currentVegetationHalfed - currentVegetationCount;

                for (var i = 0; i < diff; i++)
                {
                    var posOffset = new Vector3(
                        Random.Range(-8, 8),
                        0f,
                        Random.Range(-8, 8)
                    );
                    
                    var mesh = Random.Range(0, simplePlants.Length);
                    var posInWorld = _tileManager.TileToWorld(tileKey) + posOffset;
                    var height = _mapExtractor.GetHeightByWorldCoord(posInWorld);

                    /*if (height > 12)
                        continue;*/
                    
                    posInWorld.y = height;
                    var matrix = Matrix4x4.TRS(posInWorld, Quaternion.identity * Quaternion.Euler(-90f, 0f, 0f), _scale);

                    var veg = new VegData()
                    {
                        Material = mat,
                        Mesh = mesh,
                        Matrix = matrix
                    };

                    _tileMatrixDict[tileKey].Add(veg);
                }
            }
        }

        _vegMatrices = GetVegetationMatrix(_tileMatrixDict);
    }
    
    private Dictionary<(int, int), List<Matrix4x4>> GetVegetationMatrix(Dictionary<Vector3Int, List<VegData>> tileDataDict) {
        var result = new Dictionary<(int, int), List<Matrix4x4>>();

        foreach (var tile in tileDataDict)
        {
            var vegList = tile.Value;

            foreach (var veg in vegList)
            {
                var key = (veg.Mesh, veg.Material);
                
                if (!result.TryGetValue(key, out var vegMatrices))
                {
                    vegMatrices = new List<Matrix4x4>();
                    result[key] = vegMatrices;
                }
            
                vegMatrices.Add(veg.Matrix);
            }
        }

        return result;
    }
    
    void Update()
    {
        foreach (var veg in _vegMatrices.Keys)
        {
            Graphics.DrawMeshInstanced(simplePlants[veg.Item1], 0, simplePlantMaterials[veg.Item2], _vegMatrices[veg]);
        }
    }
    
    private struct VegData
    {
        public int Mesh;
        public int Material;
        public Matrix4x4 Matrix;   
    }
}

