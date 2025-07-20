using UnityEngine;

public class MoveBetweenTwoPoints : MonoBehaviour
{
    public Transform pointA;      // 起点
    public Transform pointB;      // 终点
    public float minSpeed = 1f;   // 最小速度
    public float maxSpeed = 3f;   // 最大速度

    private Transform targetPoint;
    private float moveSpeed;

    void Start()
    {
        targetPoint = pointB;
        RandomizeSpeed();
    }

    void Update()
    {
        if (pointA == null || pointB == null) return;

        // 移动到目标点
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, moveSpeed * Time.deltaTime);

        // 到达目标点后切换目标并随机速度
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
            RandomizeSpeed();
        }
    }

    void RandomizeSpeed()
    {
        moveSpeed = Random.Range(minSpeed, maxSpeed);
    }
}