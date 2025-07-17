using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    public enum IdleType { FloatOnly, FloatAndRotate, ScaleAndRotate }

    [SerializeField] private IdleType idleType = IdleType.FloatOnly;

    [Header("Float Settings")]
    [SerializeField] private float amplitude = 10f;
    [SerializeField] private float duration = 0.75f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationAmount = 90f;

    [Header("Scale Settings")]
    [SerializeField] private float scaleAmount = 1.25f;

    private Vector3 startLocalPos;
    private Vector3 startLocalEuler;
    private Vector3 startScale;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalEuler = transform.localEulerAngles;
        startScale = transform.localScale;

        switch (idleType)
        {
            case IdleType.FloatOnly:
                StartFloatOnly();
                break;
            case IdleType.FloatAndRotate:
                StartFloatAndRotate();
                break;
            case IdleType.ScaleAndRotate:
                StartScaleAndRotate();
                break;
        }
    }

    private void StartFloatOnly()
    {
        LeanTween.moveLocalY(gameObject, startLocalPos.y + amplitude, duration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration)); // random delay up to duration
    }

    private void StartFloatAndRotate()
    {
        LeanTween.moveLocalY(gameObject, startLocalPos.y + amplitude, duration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));

        LeanTween.rotateLocal(gameObject, new Vector3(0f, 0f, startLocalEuler.z + rotationAmount), duration * 2f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));
    }

    private void StartScaleAndRotate()
    {
        // Scale up and down
        LeanTween.scale(gameObject, startScale * scaleAmount, duration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));

        // Infinite rotation around Z
        LeanTween.rotateAroundLocal(gameObject, Vector3.forward, 360f, duration * 3f)
            .setEaseLinear()
            .setLoopClamp();
    }
}