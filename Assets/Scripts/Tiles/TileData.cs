using UnityEngine;
using UnityEngine.Tilemaps;
public class TileData
{
    
    public float travelCost, 
        landFertility,
        firmness,
        ore,
        vegetation,
        animalPopulation,
        animalHostility,
        climate,
        waterValue,
        height
        ;

    public TileData(float travelCost, float landFertility, float firmness, float ore, float vegetation,
        float animalPopulation, float animalHostility, float climate, float waterValue, float height)
    {
        this.travelCost = travelCost; 
        this.landFertility = landFertility;
        this.firmness = firmness;
        this.ore = ore;
        this.vegetation = vegetation;
        this.animalPopulation = animalPopulation;
        this.animalHostility = animalHostility;
        this.climate = climate;
        this.waterValue = waterValue;
        this.height = height;
    }
}