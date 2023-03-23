using UnityEngine;

public class HealthSpawn : MonoBehaviour
{
    [SerializeField] private GameObject healthPrefab;

    [SerializeField] private Transform healthParent;
    [SerializeField] private Transform grid;

    public GameObject activeHealth;

    public static HealthSpawn instance;

    private void Awake() => instance = this;

    public void CheckHealthOdds()
    {
        if(GameController.instance.lives >= 3 || activeHealth != null)
            return;
        
        int odds = Random.Range(1, 101);

        if(odds > 95)
            SpawnHealth();
    }

    private void SpawnHealth()
    {
        GameObject health = Instantiate(healthPrefab);

        health.GetComponent<RectTransform>().SetParent(healthParent);
        health.transform.localScale = Vector3.one;
        SetHealthPosition(health);    
    }

    private void SetHealthPosition(GameObject health)
    {
        int positionIndex = Random.Range(0, grid.childCount);
        bool overlap = grid.GetChild(positionIndex).GetComponent<ObjectOverlapStatus>().CheckOverlap();

        if(!overlap)
        {
            health.GetComponent<RectTransform>().position = grid.GetChild(positionIndex).GetComponent<RectTransform>().position;
            activeHealth = health;
        }
        else
            SetHealthPosition(health);
    } 
}
