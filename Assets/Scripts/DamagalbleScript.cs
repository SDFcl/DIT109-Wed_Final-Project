using UnityEngine;

public class DamagalbleScript : MonoBehaviour
{
    private StatsAll StatsAll;
    private int MaxHp;
    public int NowHp;
    public int HpDie = 0;

    public bool IsAlive;
    public bool DontDie = false;

    public ApiManager apiManager;

    private void Awake()
    {
        if (gameObject.CompareTag("Player"))
        {
            apiManager = FindAnyObjectByType<ApiManager>();
        }
    }

    
    private void Start()
    {
        StatsAll = GetComponent<StatsAll>();
        NowHp = StatsAll.HP;
        if (apiManager != null)
        {
            apiManager.health = NowHp;
        }
        IsAlive = true;
        
    }
    public void HIT(int Damage)
    {
        NowHp -= Damage;
        if (apiManager != null)
        {
            apiManager.health = NowHp;
        }
        if (NowHp <= 0) 
        {
            NowHp = 0;
        }
        Debug.Log(gameObject.name + " " + NowHp.ToString());
        if (NowHp <= HpDie && DontDie == false) 
        {
            IsAlive = false;
        }
    }

    public string getHp() { return NowHp.ToString(); }
}
