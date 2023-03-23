using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawn : MonoBehaviour
{
    [SerializeField] private int safetyNet;
    [HideInInspector] public int maxObstacles = 0;

    [SerializeField] private List<GameObject> obstaclePrefabs;
    [SerializeField] private List<GameObject> dummyObstacles;

    private int obstacleIndex;

    [SerializeField] private Transform obstaclesParent;
    [SerializeField] private Transform dummyObstaclesParent;

    [SerializeField] private Transform grid;

    public List<GameObject> activeObstacles;

    [SerializeField] private SnakeMovement snakeMovement;

    public static ObstacleSpawn instance;

    private void Awake() => instance = this;
    
    public void CheckObstacleOdds()
    {
        if(maxObstacles == 0)
            return;

        int odds = Random.Range(1, 101);

        if(odds > 50)
        {
            int random = Random.Range(1, maxObstacles + 1);

            for(int i = 0; i < random; i++)
                ObstacleToSpawn();
        }         
    }

    public void ObstacleToSpawn()
    {
        obstacleIndex = Random.Range(0, obstaclePrefabs.Count);

        GameObject dummyObstacle = Instantiate(dummyObstacles[obstacleIndex]);

        dummyObstacle.GetComponent<RectTransform>().SetParent(dummyObstaclesParent);
        dummyObstacle.transform.localScale = Vector3.one;

        SetObstaclePosition(dummyObstacle);
    }

    private void SetObstaclePosition(GameObject dummyObstacle)
    {
        for(int i = 0; i < safetyNet; i++)
        {
            if(obstaclePrefabs[obstacleIndex].name.Equals("Obstacle-Horizontal-3"))
            {
                int positionIndex1 = Random.Range(0, grid.childCount);
                int positionIndex2 = positionIndex1 + 1;
                int positionIndex3 = positionIndex2 + 1;

                if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1)
                    continue;

                if(ExtractName(positionIndex2)[0] != ExtractName(positionIndex1)[0] 
                || ExtractName(positionIndex3)[0] != ExtractName(positionIndex1)[0])
                    continue;
                
                dummyObstacle.transform.position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;

                if(CheckSnakePathInterference(dummyObstacle.GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(0).GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(1).GetComponent<RectTransform>()))
                {
                    Debug.Log("Detected");
                    continue;
                }
                
                if(!grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap())
                {
                    SpawnObstacle(grid.GetChild(positionIndex1).GetComponent<RectTransform>());                 
                    return;
                }
            }

            else if(obstaclePrefabs[obstacleIndex].name.Equals("Obstacle-Horizontal-4"))
            {
                int positionIndex1 = Random.Range(0, grid.childCount);
                int positionIndex2 = positionIndex1 + 1;
                int positionIndex3 = positionIndex2 + 1;
                int positionIndex4 = positionIndex3 + 1;

                if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1 || positionIndex4 > grid.childCount - 1)
                    continue;

                if(ExtractName(positionIndex2)[0] != ExtractName(positionIndex1)[0] 
                || ExtractName(positionIndex3)[0] != ExtractName(positionIndex1)[0] 
                || ExtractName(positionIndex4)[0] != ExtractName(positionIndex1)[0])
                    continue;

                dummyObstacle.transform.position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;
                
                if(CheckSnakePathInterference(dummyObstacle.GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(0).GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(1).GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(2).GetComponent<RectTransform>()))
                {
                    Debug.Log("Detected");
                    continue;
                }

                if(!grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex4).GetComponent<ObjectOverlapStatus>().CheckOverlap())
                {
                    SpawnObstacle(grid.GetChild(positionIndex1).GetComponent<RectTransform>());                 
                    return;            
                }
            }

            else if(obstaclePrefabs[obstacleIndex].name.Equals("Obstacle-Vertical-3"))
            {
                int positionIndex1 = Random.Range(0, grid.childCount);
                int positionIndex2 = positionIndex1 + 23;
                int positionIndex3 = positionIndex2 + 23;

                if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1)
                    continue;

                if(ExtractName(positionIndex2)[1] != ExtractName(positionIndex1)[1] 
                || ExtractName(positionIndex3)[1] != ExtractName(positionIndex1)[1])
                    continue;
                
                dummyObstacle.transform.position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;
                
                if(CheckSnakePathInterference(dummyObstacle.GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(0).GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(1).GetComponent<RectTransform>()))
                {
                    Debug.Log("Detected");
                    continue;
                }

                if(!grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap())
                {
                    SpawnObstacle(grid.GetChild(positionIndex1).GetComponent<RectTransform>());      
                    return;
                }
            }

            else if(obstaclePrefabs[obstacleIndex].name.Equals("Obstacle-Vertical-4"))
            {
                int positionIndex1 = Random.Range(0, grid.childCount);
                int positionIndex2 = positionIndex1 + 23;
                int positionIndex3 = positionIndex2 + 23;
                int positionIndex4 = positionIndex3 + 23;

                if(positionIndex2 > grid.childCount - 1 || positionIndex3 > grid.childCount - 1 || positionIndex4 > grid.childCount - 1)
                    continue;

                if(ExtractName(positionIndex2)[1] != ExtractName(positionIndex1)[1] 
                || ExtractName(positionIndex3)[1] != ExtractName(positionIndex1)[1] 
                || ExtractName(positionIndex4)[1] != ExtractName(positionIndex1)[1])
                    continue;

                dummyObstacle.transform.position = grid.GetChild(positionIndex1).GetComponent<RectTransform>().position;

                if(CheckSnakePathInterference(dummyObstacle.GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(0).GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(1).GetComponent<RectTransform>())
                || CheckSnakePathInterference(dummyObstacle.transform.GetChild(2).GetComponent<RectTransform>()))
                {
                    Debug.Log("Detected");
                    continue;
                }

                if(!grid.GetChild(positionIndex1).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex2).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex3).GetComponent<ObjectOverlapStatus>().CheckOverlap() 
                && !grid.GetChild(positionIndex4).GetComponent<ObjectOverlapStatus>().CheckOverlap())
                {
                    SpawnObstacle(grid.GetChild(positionIndex1).GetComponent<RectTransform>());                   
                    return;
                }
            }
        }
        
        Destroy(dummyObstacle);
        Debug.Log("Safety net reached! Unable to spawn object");
    }

    private bool CheckSnakePathInterference(RectTransform dummyObstacle)
    {
        if(snakeMovement.direction == Direction.Up || snakeMovement.direction == Direction.Down)
        {
            if(snakeMovement.segments[0].anchoredPosition.x == dummyObstacle.anchoredPosition.x
            || Mathf.Abs(snakeMovement.segments[0].anchoredPosition.x - dummyObstacle.anchoredPosition.x) < 0.5f)
                return true;
        }

        else if(snakeMovement.direction == Direction.Left || snakeMovement.direction == Direction.Right)
        {
            if(snakeMovement.segments[0].anchoredPosition.y == dummyObstacle.anchoredPosition.y
            || Mathf.Abs(snakeMovement.segments[0].anchoredPosition.y - dummyObstacle.anchoredPosition.y) < 0.5f)
                return true;
        }

        return false;
    }

    public void SpawnObstacle(RectTransform position)
    {
        GameObject obstacle = Instantiate(obstaclePrefabs[obstacleIndex]);

        obstacle.GetComponent<RectTransform>().SetParent(obstaclesParent);
        obstacle.transform.localScale = Vector3.one;
        obstacle.GetComponent<RectTransform>().position = position.position;

        activeObstacles.Add(obstacle);
    }

    private string [] ExtractName(int positionIndex)
    {   
        string name = grid.GetChild(positionIndex).name;
        string [] nameArray = name.Split("_");

        return nameArray;
    }

    public void ClearObstacles()
    {
        for(int i = 0; i < dummyObstaclesParent.childCount; i++)
            Destroy(dummyObstaclesParent.GetChild(i).gameObject);

        for(int i = 0; i < activeObstacles.Count; i++)
            activeObstacles[i].GetComponent<Deactivate>().DeactivateObject();

        activeObstacles.Clear();
    } 
}
