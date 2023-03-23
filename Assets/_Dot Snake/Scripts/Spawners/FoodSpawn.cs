using System.Collections.Generic;
using UnityEngine;

public class FoodSpawn : MonoBehaviour
{
    [SerializeField] List<Color> foodColor;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private GameObject specialFoodPrefab;

    [SerializeField] private Transform foodParent;
    [SerializeField] private Transform grid;

    public int foodCounter = 0;
    public int foodColorIndex = 0;

    public GameObject activeFood;
    
    private ObstacleSpawn _obstacleSpawn;
    public static FoodSpawn instance;

    private void Awake() => instance = this;

    private void Start() => _obstacleSpawn = GetComponent<ObstacleSpawn>();

    public void SpawnFood()
    {
        GameObject food = null;

        if(foodCounter > 8)
        {
            food = Instantiate(specialFoodPrefab);
            food.GetComponent<SpecialFood>().SetColor(foodColor[foodColorIndex]);

            foodCounter = 0;

            if(foodColorIndex < foodColor.Count - 1)     
                foodColorIndex++;
            else
                foodColorIndex = 0;

            if(_obstacleSpawn.maxObstacles < 4)
                _obstacleSpawn.maxObstacles++;
        }

        else
        {
            food = Instantiate(foodPrefab);
            food.GetComponent<Food>().SetColor(foodColor[foodColorIndex]);
            GameController.instance.SetScoreIconColor(foodColor[foodColorIndex]);
            foodCounter++;
        }

        food.GetComponent<RectTransform>().SetParent(foodParent);
        food.transform.localScale = Vector3.one;
        SetFoodPosition(food);           
    }

    private void SetFoodPosition(GameObject food)
    {
        int positionIndex = Random.Range(0, grid.childCount);
        bool overlap = grid.GetChild(positionIndex).GetComponent<ObjectOverlapStatus>().CheckOverlap();

        if(!overlap)
        {
            food.GetComponent<RectTransform>().position = grid.GetChild(positionIndex).GetComponent<RectTransform>().position;
            activeFood = food;
        }
        else
            SetFoodPosition(food);
    } 
}
