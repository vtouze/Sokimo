using UnityEngine;

public class TutorialAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private float blockDuration = 5f;

    private void Start()
    {
        if (_playerController != null)
        {
            _playerController.isBlocked = true;
            animator.speed = 1.5f;
        }

        StartCoroutine(UnblockAfterDelay());
    }

    private System.Collections.IEnumerator UnblockAfterDelay()
    {
        yield return new WaitForSeconds(blockDuration);

        if (_playerController != null)
        {
            _playerController.isBlocked = false;
        }
    }
}