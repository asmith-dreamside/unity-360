using UnityEngine;

public class CameraLook360 : MonoBehaviour
{
    public float sensitivity = 2.0f;
    private float rotationY = 0f;
    private float rotationX = 0f;

    void Update()
    {
        // Solo rota la cámara con el click izquierdo si NO se está inspeccionando
        if (ObjectInfoRaycast.isInspecting)
            return;
        if (Input.GetMouseButton(0)) // Click izquierdo para rotar la cámara
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
            rotationX -= mouseX;
            rotationY += mouseY;

            // Limitar la rotación vertical para evitar giros completos
            rotationY = Mathf.Clamp(rotationY, -90f, 90f);

            // Aplicar la rotación a la cámara
            transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
        }
    }
}
