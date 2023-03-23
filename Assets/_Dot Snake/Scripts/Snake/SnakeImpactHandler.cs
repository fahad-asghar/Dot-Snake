using UnityEngine;

public class SnakeImpactHandler : MonoBehaviour
{
    private SnakeMovement _snakeMovement;
    private SnakeRegisterImpact _snakeImpact;
    private SnakeProperties _snakeProperties;
    private SnakeSpeedController _snakeSpeedController;

    private void Start()
    {
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeImpact = GetComponent<SnakeRegisterImpact>();
        _snakeProperties = GetComponent<SnakeProperties>();
        _snakeSpeedController = GetComponent<SnakeSpeedController>();

        _snakeImpact.OnImpact += OnImpact;
    }

    private void OnDestroy() => _snakeImpact.OnImpact -= OnImpact;

    private void OnImpact(string tag, GameObject impactObject)
    {
        switch (tag)
        {
            case "RightTrigger":
                if(_snakeMovement.direction == Direction.Left)
                    _snakeProperties.FadeOutFaceLayer();
                break;

            case "LeftTrigger":
                if(_snakeMovement.direction == Direction.Right) 
                    _snakeProperties.FadeOutFaceLayer();
                break;

            case "TopTrigger":
                if(_snakeMovement.direction == Direction.Down)
                    _snakeProperties.FadeOutFaceLayer();
                break;

            case "BottomTrigger":
                if(_snakeMovement.direction == Direction.Up)
                    _snakeProperties.FadeOutFaceLayer();
                break;

            case "Food":
                ImpactFood(impactObject);
                break;
            
            case "SpecialFood":
                ImpactSpecialFood(impactObject);
                break;
            
            case "HealthPoint":
                ImpactHealthPoint(impactObject);
                break;
            
            case "SnakeSegment":
                ImpactSnakeSegment();
                break;
            
            case "Obstacle":
                ImpactObstacle();
                break;

            case "LeftWall":
                ImpactLeftWall();
                break;
            
            case "RightWall":
                ImpactRightWall();
                break;
            
            case "TopWall":
                ImpactTopWall();
                break;

            case "BottomWall":
                ImpactBottomWall();
                break;      
        }
    }

    private void ImpactFood(GameObject impactObject)
    {
        SoundManager.instance.playSound(SoundManager.instance.eatPoint[UnityEngine.Random.Range(0, SoundManager.instance.eatPoint.Count)]);

        impactObject.GetComponent<Deactivate>().DeactivateObject();
        FoodSpawn.instance.activeFood = null;

        _snakeMovement.segments[0].GetChild(1).GetComponent<Animator>().Play("SnakeHeadBounce", -1, 0);

        _snakeProperties.Grow();
        _snakeSpeedController.IncreaseSpeed();

        GameController.instance.UpdateScore(80, 1);
        ObstacleSpawn.instance.ClearObstacles();         
        ObstacleSpawn.instance.CheckObstacleOdds();
        HealthSpawn.instance.CheckHealthOdds();
        FoodSpawn.instance.SpawnFood();
    }

    private void ImpactSpecialFood(GameObject impactObject)
    {
        SoundManager.instance.playSound(SoundManager.instance.specialPoint[UnityEngine.Random.Range(0, SoundManager.instance.specialPoint.Count)]);

        impactObject.GetComponent<Deactivate>().DeactivateObject();
        FoodSpawn.instance.activeFood = null;

        _snakeMovement.segments[0].GetChild(1).GetComponent<Animator>().Play("SnakeHeadBounce", -1, 0);

        _snakeProperties.Grow();
        _snakeSpeedController.IncreaseSpeed();

        GameController.instance.UpdateScore(8, 2);
        ObstacleSpawn.instance.ClearObstacles();         
        ObstacleSpawn.instance.CheckObstacleOdds();   
        HealthSpawn.instance.CheckHealthOdds();
        FoodSpawn.instance.SpawnFood();
    }

    private void ImpactHealthPoint(GameObject impactObject)
    {
        SoundManager.instance.playSound(SoundManager.instance.healthPoint[UnityEngine.Random.Range(0, SoundManager.instance.healthPoint.Count)]);
        impactObject.GetComponent<Deactivate>().DeactivateObject();
        HealthSpawn.instance.activeHealth = null;
        GameController.instance.AddLife(1);
    }

    private void ImpactSnakeSegment()
    {
        SoundManager.instance.playSound(SoundManager.instance.snakeHit[UnityEngine.Random.Range(0, SoundManager.instance.snakeHit.Count)]);

        GameController.instance.stopMovement = true;
        GameController.instance.DeductLife(1); 
    }

    private void ImpactObstacle()
    {
        SoundManager.instance.playSound(SoundManager.instance.snakeHit[UnityEngine.Random.Range(0, SoundManager.instance.snakeHit.Count)]);

        GameController.instance.stopMovement = true;
        GameController.instance.DeductLife(1); 
    }

    private void ImpactLeftWall()
    {
        float posX = _snakeMovement.segments[1].anchoredPosition.x;
        _snakeMovement.segments[0].anchoredPosition = new Vector3(Mathf.Abs(posX), _snakeMovement.segments[0].anchoredPosition.y, 0);
    }

    private void ImpactRightWall()
    {
        float posX = _snakeMovement.segments[1].anchoredPosition.x;
        _snakeMovement.segments[0].anchoredPosition = new Vector3(-posX, _snakeMovement.segments[0].anchoredPosition.y, 0);
    }

    private void ImpactTopWall()
    {
        float posY = _snakeMovement.segments[1].anchoredPosition.y;
        float offset = 10;
        _snakeMovement.segments[0].anchoredPosition = new Vector3(_snakeMovement.segments[0].anchoredPosition.x, -posY + offset, 0);
    }

    private void ImpactBottomWall()
    {
        float posY = _snakeMovement.segments[1].anchoredPosition.y;
        float offset = 10;
        _snakeMovement.segments[0].anchoredPosition = new Vector3(_snakeMovement.segments[0].anchoredPosition.x, Mathf.Abs(posY) + offset, 0);
    }
}
