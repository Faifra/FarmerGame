using UnityEngine;

public class TomatoAI : MonoBehaviour
{
    public enum TomatoState
    {
        Mindless,
        Follower
    }

    public TomatoState state; //represent the current state of the tomato. 
    private PotatoAI leader; //represent the leader potato, assigned in SearchForLeader() method.
    private MoveTo movement;
    [SerializeField] private int searchRange = 30; //search range, can be modified.

    void Start()
    {
        movement = GetComponent<MoveTo>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case TomatoState.Mindless:
                SearchForLeader();
                break;
            case TomatoState.Follower:
                FollowLeader();
                break;
        }
    }

    private void SearchForLeader()
    {
        PotatoAI[] potatoes = FindObjectsByType<PotatoAI>(FindObjectsSortMode.None);

        foreach (PotatoAI potato in potatoes)
        {
            float distanceToLeader = Vector3.Distance(transform.position, potato.transform.position);

            if (distanceToLeader < searchRange && potato.TryRecruit())
            {
                Debug.Log("leader found!");
                leader = potato;
                state = TomatoState.Follower;
                break;
            }
        }
    }

    private void FollowLeader()
    {
        if (leader == null)
        {
            state = TomatoState.Mindless;
            movement.goal = GameObject.FindGameObjectWithTag("Player").transform;
            return;
        }

        movement.goal = leader.transform;
        
    }
}
