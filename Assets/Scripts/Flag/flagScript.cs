using UnityEngine;

public class flagScript : MonoBehaviour
{
    [SerializeField] GameObject flag;
    [SerializeField] GameObject waveManagerGO;
    [SerializeField] bool isPrepPhase = true;
    [SerializeField] float bottomFlag;
    [SerializeField] float topFlag;
    [SerializeField] float flagSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isPrepPhase = waveManagerGO.GetComponent<WaveManager>().IsPrepPhase;
        float targetY = isPrepPhase ? bottomFlag : topFlag;

        Vector3 pos = flag.transform.localPosition;

        pos.y = Mathf.MoveTowards(
            pos.y,
            targetY,
            flagSpeed * Time.deltaTime
        );

        flag.transform.localPosition = pos;
    }
}
