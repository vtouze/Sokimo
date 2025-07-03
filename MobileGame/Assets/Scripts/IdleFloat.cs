using UnityEngine;

public class IdleFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.05f;
    [SerializeField] private float frequency = 1f;

    private Vector3 startLocalPos;
    private float phaseOffset;

    void Start()
    {
        startLocalPos = transform.localPosition;
        phaseOffset = Random.Range(0f, 2 * Mathf.PI);
    }

    void Update()
    {
        float offsetY = Mathf.Sin(Time.time * frequency * Mathf.PI * 2 + phaseOffset) * amplitude;
        transform.localPosition = startLocalPos + new Vector3(0, offsetY, 0);
    }
}