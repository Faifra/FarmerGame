using UnityEngine;

[CreateAssetMenu(fileName = "New Power-Up", menuName = "Scriptable Objects/PowerUpSO")]
public class PowerUpSO : ScriptableObject
{
    public string powerupname;
    public Sprite icon;
    public PowerUpType type;
}

public enum PowerUpType
{
    SuperMegaJump,
}
