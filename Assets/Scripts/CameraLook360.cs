using UnityEngine;

/// <summary>
/// Control de cámara 360 con sistema de retícula dinámico
/// COMPORTAMIENTO DINÁMICO:
/// - PC/Editor: Rotación libre en cualquier parte de la pantalla
/// - Móviles nativos: Rotación libre en cualquier parte de la pantalla
/// - WebGL con touch activo: Solo rotación fuera del radio de la retícula central
/// - WebGL sin touch (PC en navegador): Rotación libre en cualquier parte de la pantalla
/// </summary>
public class CameraLook360 : MonoBehaviour
{
    public float sensitivity = 2.0f;
    [Header("Configuración de Retícula")]
    public float reticleRadius = 50f; // Radio en píxeles desde el centro (debe coincidir con ObjectInfoRaycast)
    [Header("Info - Solo Lectura")]
    [SerializeField] private bool reticleSystemActive = false; // Solo para mostrar en Inspector
    
    private float rotationY = 0f;
    private float rotationX = 0f;
    private bool canRotateCamera = false; // Para saber si podemos rotar la cámara
    private Vector2 initialClickPosition;

    // La lógica de retícula se maneja dinámicamente según el input actual

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

        // Sistema simplificado según input actual
        bool inputHandled = false;
        
        // Manejo para touch (móviles, tablets)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                initialClickPosition = touch.position;
                #if UNITY_WEBGL
                // En WebGL con touch activo, usar sistema de retícula
                canRotateCamera = IsClickOutsideReticle(initialClickPosition);
                reticleSystemActive = true;
                #else
                // Móviles nativos - siempre permitir rotación
                canRotateCamera = true;
                reticleSystemActive = false;
                #endif
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
        
        // Si no hay touch input, usar mouse
        if (!inputHandled)
        {
            // Verificar el inicio del click
            if (Input.GetMouseButtonDown(0))
            {
                initialClickPosition = Input.mousePosition;
                
                #if UNITY_WEBGL
                // En WebGL sin touch activo (PC), permitir rotación libre
                canRotateCamera = true;
                reticleSystemActive = false;
                #else
                // PC/Editor - siempre permitir rotación libre
                canRotateCamera = true;
                reticleSystemActive = false;
                #endif
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
