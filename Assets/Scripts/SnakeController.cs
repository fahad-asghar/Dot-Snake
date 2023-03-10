using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SnakeController : MonoBehaviour
{
    [SerializeField] private Image gridFrame;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color blinkColor;

    [SerializeField] private GameObject snakeSegmentPrefab;
    [SerializeField] private Transform snakeSegmentParent;
   
    [SerializeField][Tooltip("The lower the more")] private float moveSpeed;
 
    public List<RectTransform> segments;
    public List<SnakePath> snakePath = new List<SnakePath>();

    private int directionX;
    private int directionY;
    private float timer;
    bool blockInput = false;


    private void Start()
    {
        SwipeManager.OnSwipe += OnSwipe;
        directionX = 0;
        directionY = 40;

        GameController.instance.SpawnFood();
    }

    private void OnSwipe(SwipeData swipeData)
    {
        if(GameController.instance.stopMovement)
            return;
            
        if(swipeData.Direction == SwipeDirection.Up){
            if(directionY != -40 && !blockInput){
                directionX = 0;
                directionY = 40;
                blockInput = true;
            }
        }

        else if(swipeData.Direction == SwipeDirection.Down){
            if(directionY != 40 && !blockInput){
                directionX = 0;
                directionY = -40;
                blockInput = true;
            }
        }

        else if(swipeData.Direction == SwipeDirection.Left){
            if(directionX != 40 && !blockInput){
                directionX = -40;
                directionY = 0;
                blockInput = true;
            }
        }

        else if(swipeData.Direction == SwipeDirection.Right){
            if(directionX != -40 && !blockInput){
                directionX = 40;
                directionY = 0;
                blockInput = true;
            }
        }
    }

    private void Update()
    {
        if(GameController.instance.stopMovement)
            return;

        HandleMovement();     
    }
 
    private void HandleMovement(){

        timer += Time.deltaTime;
        
        if(timer < moveSpeed)
            return;

        SoundManager.instance.playSound(SoundManager.instance.snakeMove, 0.1f);
        blockInput = false;
        timer = 0;
        
        for(int i = segments.Count - 1; i > 0; i--)
            segments[i].anchoredPosition = segments[i - 1].anchoredPosition;

        segments[0].anchoredPosition = new Vector2(Mathf.Round(segments[0].anchoredPosition.x + directionX), Mathf.Round(segments[0].anchoredPosition.y + directionY));
        
        //For rotating the face of the snake
        if(directionX == -40 && directionY == 0) //Left
            segments[0].transform.GetChild(0).DORotate(new Vector3(0, 0, 207), 0, RotateMode.Fast);
        else if(directionX == 40 && directionY == 0) // Right
            segments[0].transform.GetChild(0).DORotate(new Vector3(0, 0, 27), 0, RotateMode.Fast);
        else if(directionX == 0 && directionY == 40) //Up
            segments[0].transform.GetChild(0).DORotate(new Vector3(0, 0, 117), 0, RotateMode.Fast);  
        else if(directionX == 0 && directionY == -40) //Down
            segments[0].transform.GetChild(0).DORotate(new Vector3(0, 0, 297), 0, RotateMode.Fast);

        segments[0].GetChild(0).GetComponent<Image>().DOFade(0, 0.1f);
        segments[0].GetChild(1).GetComponent<Image>().DOFade(1, 0.1f); 
    
        snakePath.Add(new SnakePath(){
            anchoredPosition = segments[0].anchoredPosition,
            directionX = directionX,
            directionY = directionY
        });
    }   

    private void IncreaseSpeed()
    {
        if(moveSpeed > 0.03f)
            moveSpeed -= 0.002f;
    }

    private void OnTriggerEnter2D(Collider2D obj)
    {
        if(obj.CompareTag("RightTrigger"))
        {
            if(directionX == -40 && directionY == 0) //Moving Left
            {
                segments[0].GetChild(0).GetComponent<Image>().DOFade(1, 0.1f);
                segments[0].GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
            }
        }
        if(obj.CompareTag("LeftTrigger"))
        {
            if(directionX == 40 && directionY == 0) //Moving Right
            {
                segments[0].GetChild(0).GetComponent<Image>().DOFade(1, 0.1f);
                segments[0].GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
            }
        }

        if(obj.CompareTag("TopTrigger"))
        {
            if(directionX == 0 && directionY == -40) //Moving Down
            {
                segments[0].GetChild(0).GetComponent<Image>().DOFade(1, 0.1f);
                segments[0].GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
            }
        }

        if(obj.CompareTag("BottomTrigger"))
        {
            if(directionX == 0 && directionY == 40) //Moving Up
            {
                segments[0].GetChild(0).GetComponent<Image>().DOFade(1, 0.1f);
                segments[0].GetChild(1).GetComponent<Image>().DOFade(0, 0.1f);
            }
        }


        if(obj.CompareTag("Food"))
        {
            SoundManager.instance.playSound(SoundManager.instance.eatPoint[UnityEngine.Random.Range(0, SoundManager.instance.eatPoint.Count)]);

            obj.gameObject.GetComponent<ObjectProperties>().Hide();
            GameController.instance.activeFood = null;

            segments[0].GetChild(1).GetComponent<Animator>().Play("SnakeHeadBounce", -1, 0);

            Grow();
            IncreaseSpeed();

            GameController.instance.UpdateScore(8, 1);
            GameController.instance.ClearObstacles();         
            GameController.instance.CheckObstacleOdds();
            GameController.instance.CheckHealthPointOdds();
            GameController.instance.SpawnFood();
        }

        if(obj.CompareTag("SpecialFood"))
        {
            SoundManager.instance.playSound(SoundManager.instance.specialPoint[UnityEngine.Random.Range(0, SoundManager.instance.specialPoint.Count)]);

            obj.gameObject.GetComponent<ObjectProperties>().Hide();
            GameController.instance.activeFood = null;

            segments[0].GetChild(1).GetComponent<Animator>().Play("SnakeHeadBounce", -1, 0);

            Grow();
            IncreaseSpeed();

            GameController.instance.UpdateScore(8, 5);
            GameController.instance.ClearObstacles();         
            GameController.instance.CheckObstacleOdds();
            GameController.instance.CheckHealthPointOdds();
            GameController.instance.SpawnFood();
        }

        if(obj.CompareTag("HealthPoint"))
        {
            SoundManager.instance.playSound(SoundManager.instance.healthPoint[UnityEngine.Random.Range(0, SoundManager.instance.healthPoint.Count)]);
            obj.gameObject.GetComponent<ObjectProperties>().Hide();
            GameController.instance.activeHealthPoint = null;
            GameController.instance.AddLife(1);
        }

        if(obj.CompareTag("SnakeSegment") || obj.CompareTag("Obstacle"))
        {
            SoundManager.instance.playSound(SoundManager.instance.snakeHit[UnityEngine.Random.Range(0, SoundManager.instance.snakeHit.Count)]);

            GameController.instance.stopMovement = true;
            GameController.instance.DeductLife(1);             
        }

        if(obj.CompareTag("LeftWall"))
        {
            float posX = segments[1].anchoredPosition.x;
            segments[0].anchoredPosition = new Vector3(Mathf.Abs(posX), segments[0].anchoredPosition.y, 0);
        }
        if(obj.CompareTag("RightWall"))
        {
            float posX = segments[1].anchoredPosition.x;
            segments[0].anchoredPosition = new Vector3(-posX, segments[0].anchoredPosition.y, 0);
        }
        if(obj.CompareTag("TopWall"))
        {
            float posY = segments[1].anchoredPosition.y;
            float offset = 10;
            segments[0].anchoredPosition = new Vector3(segments[0].anchoredPosition.x, -posY + offset, 0);
        }
        if(obj.CompareTag("BottomWall"))
        {
            float posY = segments[1].anchoredPosition.y;
            float offset = 10;
            segments[0].anchoredPosition = new Vector3(segments[0].anchoredPosition.x, Mathf.Abs(posY) + offset, 0);
        }
    }

    private void Grow()
    {
        GameObject snakeSegment = Instantiate(snakeSegmentPrefab);
        snakeSegment.GetComponent<RectTransform>().SetParent(snakeSegmentParent);
        snakeSegment.transform.position = segments[segments.Count - 1].position;
        snakeSegment.transform.localScale = Vector3.one;
        segments.Add(snakeSegment.GetComponent<RectTransform>());
    }

    public void ChangeColors()
    {
        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(hitColor, 0.5f);
        
        segments[0].GetChild(0).GetComponent<Image>().DOColor(hitColor, 0.5f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(hitColor, 0.5f);

        gridFrame.DOColor(hitColor, 0.5f).OnComplete(delegate(){
            
            for(int i = 1; i < segments.Count; i++)
                segments[i].GetComponent<Image>().DOColor(defaultColor, 0.5f).SetDelay(1);
            
            segments[0].GetChild(0).GetComponent<Image>().DOColor(defaultColor, 0.5f).SetDelay(1);
            segments[0].GetChild(1).GetComponent<Image>().DOColor(defaultColor, 0.5f).SetDelay(1);

            gridFrame.DOColor(defaultColor, 0.5f).SetDelay(1).OnComplete(delegate(){
                ShortenSnake();
            });
        });
    }

    private void ShortenSnake()
    {          
        if(segments.Count == 3)
            StartCoroutine(Reverse());

        else if(segments.Count >= 4 && segments.Count <= 10)
        {
            SoundManager.instance.playSound(SoundManager.instance.snakeShorten);

            segments[segments.Count - 1].GetComponent<Image>().DOFade(0, 1).OnComplete(delegate(){
                segments[segments.Count - 1].gameObject.SetActive(false);
                segments.RemoveAt(segments.Count - 1);
                StartCoroutine(Reverse());
            });
        }

        else
        {
            SoundManager.instance.playSound(SoundManager.instance.snakeShorten);

            segments[segments.Count - 1].GetComponent<Image>().DOFade(0, 1);
            segments[segments.Count - 2].GetComponent<Image>().DOFade(0, 1);
            segments[segments.Count - 3].GetComponent<Image>().DOFade(0, 1);
            segments[segments.Count - 4].GetComponent<Image>().DOFade(0, 1);
            segments[segments.Count - 5].GetComponent<Image>().DOFade(0, 1).OnComplete(delegate(){
                segments[segments.Count - 1].gameObject.SetActive(false);
                segments.RemoveAt(segments.Count - 1);
                segments[segments.Count - 1].gameObject.SetActive(false);
                segments.RemoveAt(segments.Count - 1);
                segments[segments.Count - 1].gameObject.SetActive(false);
                segments.RemoveAt(segments.Count - 1);
                segments[segments.Count - 1].gameObject.SetActive(false);
                segments.RemoveAt(segments.Count - 1);
                segments[segments.Count - 1].gameObject.SetActive(false);
                segments.RemoveAt(segments.Count - 1);
                StartCoroutine(Reverse());
            });
        }
    }

    private IEnumerator Reverse()
    {   
        segments[0].GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(moveSpeed);

        for(int i = segments.Count; i < segments.Count + 5; i++)
        {
            for(int j = 0; j < segments.Count - 1; j++)
            {
                segments[j].anchoredPosition = segments[j + 1].anchoredPosition;
            }

            SoundManager.instance.playSound(SoundManager.instance.snakeMove, 0.1f);
            segments[segments.Count - 1].anchoredPosition = snakePath[snakePath.Count - 1 - i].anchoredPosition;
            yield return new WaitForSeconds(moveSpeed);
        }

        for(int i = 0; i < 5; i++)
            snakePath.RemoveAt(snakePath.Count - 1);

        directionX = snakePath[snakePath.Count - 1].directionX;
        directionY = snakePath[snakePath.Count - 1].directionY;

        Blink();  
    }

    public void Blink()
    {
        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(blinkColor, 0.4f);
        
        segments[0].GetChild(0).GetComponent<Image>().DOColor(blinkColor, 0.4f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(blinkColor, 0.4f);

        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(0.4f);
        
        segments[0].GetChild(0).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(0.4f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(0.4f);

        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(0.8f);
        
        segments[0].GetChild(0).GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(0.8f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(0.8f);

        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1.2f);

        segments[0].GetChild(0).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1.2f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(1.2f);

        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(1.6f);

        segments[0].GetChild(0).GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(1.6f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(blinkColor, 0.4f).SetDelay(1.6f);

        for(int i = 1; i < segments.Count; i++)
            segments[i].GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(2f);
        
        segments[0].GetChild(0).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(2f);
        segments[0].GetChild(1).GetComponent<Image>().DOColor(defaultColor, 0.4f).SetDelay(2f);

        for(float i = 0; i < 2f; i = i + 0.8f)
            Invoke("PlayBlinkSound", i);
     
        Invoke("Move", 2.3f);
    }

    private void PlayBlinkSound()
    {
        SoundManager.instance.playSound(SoundManager.instance.snakeBlink, 0.2f);
    }

    public void Move()
    {
        segments[0].GetComponent<Collider2D>().enabled = true;
        GameController.instance.stopMovement = false; 
    }
}


[System.Serializable]
public class SnakePath
{
    public Vector3 anchoredPosition;
    public int directionX;
    public int directionY;
}
