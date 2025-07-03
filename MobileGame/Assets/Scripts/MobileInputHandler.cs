using UnityEngine;

public class MobileInputHandler : MonoBehaviour
{
    public PlayerController playerController;

    public void MoveUp() => playerController.Move(Vector3Int.up);
    public void MoveDown() => playerController.Move(Vector3Int.down);
    public void MoveLeft() => playerController.Move(Vector3Int.left);
    public void MoveRight() => playerController.Move(Vector3Int.right);
}