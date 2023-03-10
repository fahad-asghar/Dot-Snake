using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOverlapStatus : MonoBehaviour
{
    [SerializeField] private SnakeController snakeController;

    public bool CheckOverlap()
    {
        for(int i = 0; i < snakeController.segments.Count; i++)
        {
            if(GetComponent<RectTransform>().position == snakeController.segments[i].position)
                return true;
        }

        if(GameController.instance.activeObstacle.Count > 0)
        {
            for(int i = 0; i < GameController.instance.activeObstacle.Count; i++)
            {
                if(GetComponent<RectTransform>().position == GameController.instance.activeObstacle[i].GetComponent<RectTransform>().position)
                    return true;
                
                for(int j = 0; j < GameController.instance.activeObstacle[i].transform.childCount; j++)
                {
                    if(GetComponent<RectTransform>().position == GameController.instance.activeObstacle[i].transform.GetChild(j).GetComponent<RectTransform>().position)
                        return true;
                }
            }
        }

        if(GameController.instance.activeHealthPoint != null)
        {
            if(GetComponent<RectTransform>().position == GameController.instance.activeHealthPoint.GetComponent<RectTransform>().position)
                return true;
        }

        if(GameController.instance.activeFood != null)
        {
            if(GetComponent<RectTransform>().position == GameController.instance.activeFood.GetComponent<RectTransform>().position)
                return true;
        }

        return false;
    }
}
