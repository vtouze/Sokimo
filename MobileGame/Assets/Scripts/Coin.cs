using UnityEngine;

public class Coin : MonoBehaviour
{
    public void Collect()
    {
        Debug.Log("Coin collected at " + transform.position);
        // TODO: play animation/VFX
        Destroy(gameObject); // Replace with animation+destroy later
    }
}