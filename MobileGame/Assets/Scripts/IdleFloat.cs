using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 10;
    [SerializeField] private float duration = 0.75f;

    private Vector3 startLocalPos;

    void Start()
    {
        startLocalPos = transform.localPosition;
        StartIdleFloat();
    }

    private void StartIdleFloat()
    {
        LeanTween.moveLocalY(gameObject, startLocalPos.y + amplitude, duration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));
    }
}