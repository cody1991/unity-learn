using UnityEngine;

[CreateAssetMenu(fileName = "powerup", menuName = "Scriptable Objects/powerup")]
public class powerup : ScriptableObject
{
    [SerializeField] string powerupName;
    [SerializeField] float valueChange;
    [SerializeField] float time;

    public string GetPowerupName() {
        return powerupName;
    }

    public float GetValueChange() {
        return valueChange;
    }

    public float GetTime() {
        return time;
    }
}
