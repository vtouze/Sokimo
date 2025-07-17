using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsLocked = true;

    public void OpenDoor()
    {
        IsLocked = false;
        Destroy(gameObject);
    }
}