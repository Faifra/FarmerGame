using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFootstepsScript : MonoBehaviour
{
    [SerializeField] AudioClip footStepSFX;
    [SerializeField, Range(0,1)] private float TimeInBetweenSteps = 0.035f;

    private InputAction moveAction;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        StartCoroutine(PlayFootSteps());
    }

    IEnumerator PlayFootSteps()
    {
        while (true)
        {
            if (moveAction.ReadValue<Vector2>().magnitude > 0.1f && PlayerCollisionScript.isGrounded())
            {
                AudioManager.instance.PlaySFX(footStepSFX);
            }

            yield return new WaitForSeconds(TimeInBetweenSteps);
        }
    }
}
