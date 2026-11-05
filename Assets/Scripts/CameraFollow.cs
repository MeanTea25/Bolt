using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 14f, -9f);

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 focus = target.position + Vector3.up;
        transform.position = Vector3.Lerp(transform.position, focus + offset, 5f * Time.deltaTime);
        transform.LookAt(focus);
    }
}
