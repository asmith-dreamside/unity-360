using UnityEngine;

public class CameraLook360 : MonoBehaviour
{
    public float sensitivity = 2.0f;
    [Header("Configuración de Retícula")]
    public float reticleRadius = 50f; // Radio en píxeles desde el centro (debe coincidir con ObjectInfoRaycast)
    
    private float rotationY = 0f;
    private float rotationX = 0f;
    private bool canRotateCamera = false; // Para saber si podemos rotar la cámara
    private Vector2 initialClickPosition;

    // Verificar si la posición del click está fuera del radio de la retícula
    private bool IsClickOutsideReticle(Vector2 inputPosition)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float distanceFromCenter = Vector2.Distance(inputPosition, screenCenter);
        return distanceFromCenter > reticleRadius;
    }

    void Update()
    {
        // Solo rota la cámara si NO se está inspeccionando
        if (ObjectInfoRaycast.isInspecting)
            return;

        // Sistema unificado para todas las plataformas
        bool inputHandled = false;
        
        // Manejo prioritario para touch (móviles, tablets)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                initialClickPosition = touch.position;
                // Solo permitir rotación si el toque inicial está fuera del radio de la retícula
                canRotateCamera = IsClickOutsideReticle(initialClickPosition);
            }
            
            if (canRotateCamera && touch.phase == TouchPhase.Moved)
            {
                float deltaX = touch.deltaPosition.x * sensitivity * 0.1f;
                float deltaY = touch.deltaPosition.y * sensitivity * 0.1f;
                rotationX -= deltaX;
                rotationY += deltaY;

                // Limitar la rotación vertical
                rotationY = Mathf.Clamp(rotationY, -90f, 90f);

                // Aplicar la rotación a la cámara
                transform.localEulerAngles = new Vector3(rotationY, rotationX, 0);
            }
            
            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                canRotateCamera = false;
            }
            
            inputHandled = true;
        }
        
        // Si no hay touch input, usar mouse (PC, WebGL)
        if (!inputHandled)
        {
            // Verificar el inicio del click
            if (Input.GetMouseButtonDown(0))
            {
                initialClickPosition = Input.mousePosition;
                // Solo permitir rotación si el click inicial está fuera del radio de la retícula
                canRotateCamera = IsClickOutsideReticle(initialClickPosition);
            }

            // Solo rotar si tenemos permiso y estamos manteniendo el click
            if (canRotateCamera && Input.GetMouseButton(0))
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

            // Resetear el permiso cuando soltamos el click
            if (Input.GetMouseButtonUp(0))
            {
                canRotateCamera = false;
            }
        }
    }
}
