using UnityEngine;
using UnityEngine.UI;

public class BossUIHelperScript : MonoBehaviour
{

    [SerializeField] private GameObject bossBarCollection;
    private GameObject bossBarUIOBject;
    private TMPro.TMP_Text bossText;
    private Slider bossHPBar;

    private EnemyHealthScript healthScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossBarCollection = GameObject.FindGameObjectWithTag("BossBarCollection"); // Find our Boss bar collection
        

        //Debug.Log(Resources.Load("BossBar"));

        bossBarUIOBject = Instantiate(Resources.Load("BossBar", typeof(GameObject))) as GameObject; // Instanciate prefab
        bossBarUIOBject.transform.SetParent(bossBarCollection.transform); // attach to collection
        bossBarUIOBject.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);

        bossText = bossBarUIOBject.GetComponentInChildren<TMPro.TMP_Text>();
        bossHPBar = bossBarUIOBject.GetComponentInChildren<Slider>();
        healthScript = GetComponent<EnemyHealthScript>();
    }

    // Update is called once per frame
    void Update()
    {
        bossText.text = healthScript.Type.ToString(); //enemy.name;
        bossHPBar.maxValue = healthScript.MaxHealth; //health.MaxHealth;
        bossHPBar.value = healthScript.CurrentHealth();
    }

    public void RemoveHealthBar()
    {
        Destroy(bossBarUIOBject);
    }

}
