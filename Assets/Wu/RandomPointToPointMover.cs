using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RandomPointToPointMover : MonoBehaviour
{
    public Transform centerPoint;    // 中心点
    public float radius = 2f;        // 随机点距离中心的半径
    public float minSpeed = 1f;      // 最小速度
    public float maxSpeed = 3f;      // 最大速度

    private Vector3 startPoint;
    private Vector3 endPoint;
    private float moveSpeed;
    private float t = 0f;

    void Start()
    {
        // 初始点为当前位置或中心点附近随机点
        startPoint = transform.position;
        endPoint = GetRandomPointAroundCenter();
        moveSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void Update()
    {
        t += Time.deltaTime * moveSpeed / Vector3.Distance(startPoint, endPoint);
        transform.position = Vector3.Lerp(startPoint, endPoint, t);

        if (t >= 1f)
        {
            // 到达终点后，起点变为当前点，终点重新随机
            startPoint = endPoint;
            endPoint = GetRandomPointAroundCenter();
            moveSpeed = Random.Range(minSpeed, maxSpeed);
            t = 0f;
        }
    }

    Vector3 GetRandomPointAroundCenter()
    {
        return centerPoint.position + Random.onUnitSphere * radius;
    }
    void OnSelectEnter(XRBaseInteractor interactor)
    {
        Debug.Log("OnSelectEnter");
        this.enabled = false;
    }
}