using UnityEngine;
using DG.Tweening;

public class GateTriggerController : MonoBehaviour
{
    [Header("两个触发区域完成后要移动的门")]
    public Transform door;

    [Header("移动参数")]
    public Vector3 moveOffset = new Vector3(0, 3f, 0);  // 上移3单位
    public float moveDuration = 1f;
    public Ease moveEase = Ease.OutQuad;

    private bool[] zonesTriggered = new bool[2];
    private bool doorOpened = false;

    public void NotifyZoneTriggered(int zoneID)
    {
        if (zoneID < 0 || zoneID > 1) return;

        zonesTriggered[zoneID] = true;

        if (!doorOpened && zonesTriggered[0] && zonesTriggered[1])
        {
            OpenGate();
        }
    }

    private void OpenGate()
    {
        doorOpened = true;

        if (door != null)
        {
            door.DOMove(door.position + moveOffset, moveDuration)
                .SetEase(moveEase);
            Debug.Log("门已打开！");
        }
    }
}
