using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public Transform target;
    public float speed = 2f;

    void Update()
    {
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > 0.5f)
        {
            transform.position += direction * speed * Time.deltaTime;
           Vector3 lookDir = new Vector3(direction.x, 0, direction.z);
if (lookDir != Vector3.zero) transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}