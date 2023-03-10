using UnityEngine;
using DG.Tweening;

public class ObjectProperties : MonoBehaviour
{
    [SerializeField] private ObjectType objectType;

    public enum ObjectType
    {
        Obstacle,
        Food,
        SpecialFood,
        HealthPoint
    }

    void Start()
    {
        if(objectType == ObjectType.Obstacle)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);
        }

        else if(objectType == ObjectType.Food)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f).OnComplete(delegate(){
                Bounce();
            }); 
        }

        else if(objectType == ObjectType.SpecialFood)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f);
        }

        else if(objectType == ObjectType.HealthPoint)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.2f).OnComplete(delegate(){
                Invoke("Hide", 10);
            });
        }   
    }

    private void Bounce()
    {
        transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f).OnComplete(delegate(){
            transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).OnComplete(delegate(){
                transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).OnComplete(delegate(){
                    transform.DOScale(new Vector3(0.9f, 0.9f, 0.9f), 0.1f).OnComplete(delegate(){
                        transform.DOScale(Vector3.one, 0.1f).OnComplete(delegate(){
                            Invoke("Bounce", 5);
                        });
                    });
                });
            });
        });
    }

    public void Hide()
    {
        CancelInvoke();
        DOTween.Kill(transform);
        
        gameObject.GetComponent<Collider2D>().enabled = false;

        for(int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Collider2D>().enabled = false;

        transform.DOScale(Vector3.zero, 0.2f).OnComplete(delegate(){
            Destroy(gameObject);
        });
    }
}
