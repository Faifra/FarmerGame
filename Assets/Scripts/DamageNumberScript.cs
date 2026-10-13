using TMPro;
using UnityEngine;

/// <summary>
/// Displays floating damage numbers that face the player and fade over time.
/// </summary>
public class DamageNumberScript : MonoBehaviour
{
    // Controls the upward movement and lifetime of the damage number.
    [Header("Animation")]
    [SerializeField] private float floatUpSpeed = 1.5f;
    [SerializeField] private float floatUpHeight = 1f;
    [SerializeField] private float fadeDuration = 0.75f;

    // Defines the text color used for damage numbers.
    [Header("Appearance")]
    [SerializeField] private Color damageColor = Color.red;

    // References used to animate the text and orient it toward the player.
    private TMP_Text text;
    private Camera playerCamera;
    private Vector3 startPosition;
    private Color originalColor;
    private float elapsedTime;

    // Initializes the damage text and stores its starting values.
    public void Initialize(int damage)
    {
        text = GetComponentInChildren<TMP_Text>();

        if (text == null)
        {
            Destroy(gameObject);
            return;
        }

        playerCamera = Camera.main;

        text.text = damage.ToString();

        originalColor = text.color;
        text.color = damageColor;

        startPosition = transform.position;
        elapsedTime = 0f;
    }

    // Updates the floating animation, camera-facing rotation, and fade.
    private void LateUpdate()
    {
        if (elapsedTime >= fadeDuration)
        {
            Destroy(gameObject);
            return;
        }

        elapsedTime += Time.deltaTime;

        float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
        float height = Mathf.Min(
            floatUpSpeed * elapsedTime,
            floatUpHeight
        );

        transform.position = startPosition + Vector3.up * height;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(
                transform.position - playerCamera.transform.position
            );
        }

        Color color = damageColor;
        color.a = originalColor.a * (1f - progress);
        text.color = color;
    }
}