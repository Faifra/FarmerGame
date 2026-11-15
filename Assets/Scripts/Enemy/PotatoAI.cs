using UnityEngine;

public class PotatoAI : MonoBehaviour
{
    [SerializeField] private int maxFollowers = 4;

    [SerializeField] private int followerCount = 0;

    public bool HasAvailableSlots => followerCount < maxFollowers;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TryRecruit()
    {
        if (followerCount >= maxFollowers)
            return false;

        followerCount++;
        return true;
    }
}
