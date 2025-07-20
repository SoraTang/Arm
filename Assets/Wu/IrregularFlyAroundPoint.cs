using UnityEngine;

public class IrregularFlyAroundPoint : MonoBehaviour
{
    public Transform centerPoint; // 飞行中心点
    public float radius = 2f;     // 最大飞行半径
    public float speed = 1f;      // 飞行速度
    public float noiseScale = 1f; // 噪声影响程度

    private float timeOffset;

    void Start()
    {
        // 给每个物体一个不同的噪声起点
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time * speed + timeOffset;

        // 使用Perlin Noise和正弦波生成不规则轨迹
        float angle = Mathf.PerlinNoise(t * noiseScale, 0f) * Mathf.PI * 2f;
        float height = Mathf.Sin(t) * 0.5f + Mathf.PerlinNoise(0f, t * noiseScale) * 0.5f;

        Vector3 offset = new Vector3(
            Mathf.Cos(angle),
            height,
            Mathf.Sin(angle)
        ) * radius;

        if (centerPoint != null)
            transform.position = centerPoint.position + offset;
    }
}