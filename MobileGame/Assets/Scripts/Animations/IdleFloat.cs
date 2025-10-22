using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    public enum IdleType
    {
        FloatOnly,
        FloatAndRotate,
        ScaleAndRotate,
        FloatRotateX,
        RotateZPingPong
    }

    [Header("Animation Type")]
    [SerializeField] private IdleType idleType = IdleType.FloatOnly;

    [Header("Float Settings")]
    [SerializeField] private float amplitude = 0.1f;
    [SerializeField] private float duration = 0.5f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationAmount = 45f; // For Z rotation (left/right)
    [SerializeField] private float xRotationAngle = 15f;  // For X tilt
    [SerializeField] private float zRotationSpeed = 360f; // Degrees per second for continuous Z rotation

    [Header("Scale Settings")]
    [SerializeField] private float scaleAmount = 1.25f;

    [Header("Game Settings")]
    [SerializeField] private bool isInGame = true;

    private Vector3 startLocalPos;
    private Vector3 startLocalEuler;
    private Vector3 startScale;

    private void Start()
    {
        InitializeTransform();
        StartSelectedAnimation();
    }

    private void InitializeTransform()
    {
        if (isInGame)
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
        startLocalPos = transform.localPosition;
        startLocalEuler = transform.localEulerAngles;
        startScale = transform.localScale;
    }

    private void StartSelectedAnimation()
    {
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
            case IdleType.FloatRotateX:
                StartFloatAndRotateX();
                break;
            case IdleType.RotateZPingPong:
                StartRotateZPingPong();
                break;
        }
    }

    private void StartFloatOnly()
    {
        AnimateFloat();
    }

    private void StartFloatAndRotate()
    {
        AnimateFloat();
        AnimateZRotation();
    }

    private void StartScaleAndRotate()
    {
        AnimateScale();
        AnimateContinuousZRotation();
    }

    private void StartFloatAndRotateX()
    {
        AnimateFloat();
        AnimateXRotation();
    }

    private void StartRotateZPingPong()
    {
        LeanTween.rotateLocal(
                gameObject,
                new Vector3(0f, 0f, startLocalEuler.z - rotationAmount),
                duration
            )
            .setEaseInOutSine()
            .setLoopPingPong(1)
            .setOnComplete(StartRotateZPingPong);
    }

    // Animation Helpers (unchanged)
    private void AnimateFloat()
    {
        LeanTween.moveLocalY(gameObject, startLocalPos.y + amplitude, duration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));
    }

    private void AnimateZRotation()
    {
        LeanTween.rotateLocal(gameObject, new Vector3(0f, 0f, startLocalEuler.z + rotationAmount), duration * 2f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));
    }

    private void AnimateContinuousZRotation()
    {
        LeanTween.rotateAroundLocal(gameObject, Vector3.forward, zRotationSpeed, duration)
            .setEaseLinear()
            .setLoopClamp();
    }

    private void AnimateXRotation()
    {
        LeanTween.rotateLocal(gameObject,
            new Vector3(startLocalEuler.x + xRotationAngle, startLocalEuler.y, startLocalEuler.z),
            duration * 2f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));
    }

    private void AnimateScale()
    {
        LeanTween.scale(gameObject, startScale * scaleAmount, duration)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setDelay(Random.Range(0f, duration));
    }
}