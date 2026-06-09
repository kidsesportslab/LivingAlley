using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotateSpeed = 3f;
    private bool isDragging = false;
    private Vector3 lastMousePos;

    void Update()
    {
        // WASD移動
        float h = 0f, v = 0f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;

        transform.position += transform.right * h * moveSpeed * Time.deltaTime;
        transform.position += new Vector3(transform.forward.x, 0, transform.forward.z).normalized * v * moveSpeed * Time.deltaTime;

        // 右クリックドラッグで視点回転
        if (Input.GetMouseButtonDown(1)) { isDragging = true; lastMousePos = Input.mousePosition; }
        if (Input.GetMouseButtonUp(1)) isDragging = false;
        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;
            transform.eulerAngles += new Vector3(-delta.y * rotateSpeed * Time.deltaTime, delta.x * rotateSpeed * Time.deltaTime, 0);
            lastMousePos = Input.mousePosition;
        }
    }
}