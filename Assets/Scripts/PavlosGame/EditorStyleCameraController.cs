using UnityEngine;

public class EditorStyleCameraController : MonoBehaviour
{
    public Transform cameraTransform; // Drag your actual Camera here
    public float panSpeed = 0.01f;
    public float rotateSpeed = 5f;
    public float zoomSpeed = 10f;

    private Vector3 lastMousePos;

    void Update()
    {
        // Mouse delta
        Vector3 mouseDelta = Input.mousePosition - lastMousePos;
        lastMousePos = Input.mousePosition;

        // --- Right Click = Rotate camera ---
        if (Input.GetMouseButton(1))
        {
            float rotX = -mouseDelta.y * rotateSpeed * Time.deltaTime;
            float rotY = mouseDelta.x * rotateSpeed * Time.deltaTime;

            transform.eulerAngles += new Vector3(rotX, rotY, 0f);
        }

        // --- Middle Click = Pan (screen-space) ---
        if (Input.GetMouseButton(2))
        {
            Vector3 move = new Vector3(-mouseDelta.x, -mouseDelta.y, 0f) * panSpeed;
            transform.Translate(move, Space.Self);
        }

        // --- Scroll Wheel = Zoom ---
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            transform.Translate(Vector3.forward * scroll * zoomSpeed, Space.Self);
        }
    }
}
