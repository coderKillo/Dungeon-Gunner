using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int currentHealth;

    private int startingHealth;
    public int StartingHealth
    {
        set
        {
            startingHealth = value;
            currentHealth = value;
        }
        get
        {
            return startingHealth;
        }
    }
}
