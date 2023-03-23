using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class SnakeProperties : MonoBehaviour
{
    [SerializeField] Transform point;
    [SerializeField] private GameObject snakeSegmentPrefab;
    [SerializeField] private Transform snakeSegmentParent;

    private SnakeMovement _snakeMovement;
    private SnakeImpactColorChange _snakeImpactColorChange;

    private void Start()
    {
        _snakeMovement = GetComponent<SnakeMovement>();
        _snakeImpactColorChange = GetComponent<SnakeImpactColorChange>();

        _snakeImpactColorChange.OnImpact_ColorEffectDone += OnImpact_ColorEffectDone;
    }

    private void OnDestroy() => _snakeImpactColorChange.OnImpact_ColorEffectDone -= OnImpact_ColorEffectDone;

    private void OnImpact_ColorEffectDone() => ShortenSnake();

    private void ShortenSnake()
    {
        if(_snakeMovement.segments.Count == 3)
            StartCoroutine(Reverse());

        else if(_snakeMovement.segments.Count >= 4 && _snakeMovement.segments.Count <= 10)
        {
            SoundManager.instance.playSound(SoundManager.instance.snakeShorten);

            _snakeMovement.segments[_snakeMovement.segments.Count - 1].GetComponent<Image>().DOFade(0, 1).OnComplete(delegate(){
                
                _snakeMovement.segments[_snakeMovement.segments.Count - 1].gameObject.SetActive(false);
                _snakeMovement.segments.RemoveAt(_snakeMovement.segments.Count - 1);

                StartCoroutine(Reverse());
            });
        }

        else
        {
            SoundManager.instance.playSound(SoundManager.instance.snakeShorten);

            for(int i = 1; i < 5; i++)
                _snakeMovement.segments[_snakeMovement.segments.Count - i].GetComponent<Image>().DOFade(0, 1);

            _snakeMovement.segments[_snakeMovement.segments.Count - 5].GetComponent<Image>().DOFade(0, 1).OnComplete(delegate(){
                
                for(int i = 0; i < 5; i++)
                {
                    _snakeMovement.segments[_snakeMovement.segments.Count - 1].gameObject.SetActive(false);
                    _snakeMovement.segments.RemoveAt(_snakeMovement.segments.Count - 1);
                }
                StartCoroutine(Reverse());
            });
        }
    }

    private IEnumerator Reverse()
    {   
        _snakeMovement.segments[0].GetComponent<Collider2D>().enabled = false;

        for(int i = _snakeMovement.segments.Count; i < _snakeMovement.segments.Count + 10; i++)
        {
            for(int j = 0; j < _snakeMovement.segments.Count - 1; j++)
                _snakeMovement.segments[j].anchoredPosition = _snakeMovement.segments[j + 1].anchoredPosition;

            SoundManager.instance.playSound(SoundManager.instance.snakeMove, 0.1f);
            _snakeMovement.segments[_snakeMovement.segments.Count - 1].anchoredPosition = _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1 - i].anchoredPosition;
            
            yield return new WaitForSeconds(0.05f);
        }

        for(int i = 0; i < 10; i++)
            _snakeMovement.snakePath.RemoveAt(_snakeMovement.snakePath.Count - 1);
        
        SetNewDirection();
        _snakeMovement.Invoke("StartMovement", 0.6f);      
    }

    private void SetNewDirection()
    {
        //Up
        if(_snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionX == 0
        && _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionY == 40)    
            _snakeMovement.direction = Direction.Up;
        

        //Down
        else if(_snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionX == 0
        && _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionY == -40)
            _snakeMovement.direction = Direction.Down;

        //Right
        else if(_snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionX == 40
        && _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionY == 0)
            _snakeMovement.direction = Direction.Right;

        //Left
        else if(_snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionX == -40
        && _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionY == 0)
            _snakeMovement.direction = Direction.Left;


        _snakeMovement.directionX = _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionX;
        _snakeMovement.directionY = _snakeMovement.snakePath[_snakeMovement.snakePath.Count - 1].directionY;
    }

    public void FadeOutFaceLayer()
    {
        _snakeMovement.segments[0].GetChild(0).GetComponent<Image>().DOFade(1, 0.1f);
        _snakeMovement.segments[0].GetChild(1).GetComponent<Image>().DOFade(0, 0.1f); 
    }

    public void FadeInFaceLayer()
    {
        _snakeMovement.segments[0].GetChild(0).GetComponent<Image>().DOFade(0, 0.1f);
        _snakeMovement.segments[0].GetChild(1).GetComponent<Image>().DOFade(1, 0.1f); 
    }

    public void RotateFace(float rotateFactor) => _snakeMovement.segments[0].transform.GetChild(0).DORotate(new Vector3(0, 0, rotateFactor), 0, RotateMode.Fast);

    public void Grow()
    {
        GameObject snakeSegment = Instantiate(snakeSegmentPrefab);

        snakeSegment.GetComponent<RectTransform>().SetParent(snakeSegmentParent);
        snakeSegment.transform.position = _snakeMovement.segments[_snakeMovement.segments.Count - 1].position;
        snakeSegment.transform.localScale = Vector3.one;

        _snakeMovement.segments.Add(snakeSegment.GetComponent<RectTransform>());
    }
}
