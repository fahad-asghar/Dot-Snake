using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOverlapStatus : MonoBehaviour
{
    [SerializeField] private SnakeMovement snakeMovement;

    public bool CheckOverlap()
    {
        for(int i = 0; i < snakeMovement.segments.Count; i++)
        {
            if(GetComponent<RectTransform>().position == snakeMovement.segments[i].position)
                return true;
        }

        if(ObstacleSpawn.instance.activeObstacles.Count > 0)
        {
            for(int i = 0; i < ObstacleSpawn.instance.activeObstacles.Count; i++)
            {
                if(GetComponent<RectTransform>().position == ObstacleSpawn.instance.activeObstacles[i].GetComponent<RectTransform>().position)
                    return true;
                
                for(int j = 0; j < ObstacleSpawn.instance.activeObstacles[i].transform.childCount; j++)
                {
                    if(GetComponent<RectTransform>().position == ObstacleSpawn.instance.activeObstacles[i].transform.GetChild(j).GetComponent<RectTransform>().position)
                        return true;
                }
            }
        }

        if(HealthSpawn.instance.activeHealth != null)
        {
            if(GetComponent<RectTransform>().position == HealthSpawn.instance.activeHealth.GetComponent<RectTransform>().position)
                return true;
        }

        if(FoodSpawn.instance.activeFood != null)
        {
            if(GetComponent<RectTransform>().position == FoodSpawn.instance.activeFood.GetComponent<RectTransform>().position)
                return true;
        }

        return false;
    }
}
