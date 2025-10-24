using UnityEngine;

public class ObjectInspector : MonoBehaviour
{
    public float inspectDistance = 2f; // Distancia frente a la cámara para inspección
    public float inspectHeightOffset = 0f; // Offset de altura para inspección
    public float rotationSpeed = 100f;
    public Transform pivotPoint; // Punto alrededor del cual rotará el objeto
    
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalPivotLocalPos; // Para guardar la posición local del pivote
    private bool inspecting = false;
    private Camera mainCamera;
    private Vector3 previousMousePosition;

    // Integración con PanelManager
    public PanelManager panelManager;
    public string objectName;
    public string objectSummary;
    public Texture objectImage;

    // Sistema de puntos de detalle
    private ObjectDetailPoint[] detailPoints;

    void Start()
    {
        mainCamera = Camera.main;
        // Obtener todos los puntos de detalle del objeto
        detailPoints = GetComponentsInChildren<ObjectDetailPoint>();
        
        // Buscar el PanelManager si no está asignado
        if (panelManager == null)
        {
            panelManager = FindAnyObjectByType<PanelManager>();
            if (panelManager == null)
            {
                Debug.LogError("[ObjectInspector] No se encontró ningún PanelManager en la escena. Por favor, asegúrate de que existe un objeto con el componente PanelManager.");
            }
        }

        // Si no hay un punto pivote asignado, crear uno en el centro del objeto
        if (pivotPoint == null)
        {
            GameObject pivot = new GameObject("InspectionPivot");
            pivotPoint = pivot.transform;
            pivotPoint.SetParent(transform);
            
            // Calcular el centro basado en el renderer si existe
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                pivotPoint.localPosition = renderer.bounds.center - transform.position;
            }
            else
            {
                pivotPoint.localPosition = Vector3.zero;
            }
        }
        
        // Guardar la posición local original del pivote
        originalPivotLocalPos = pivotPoint.localPosition;
    }

    public void StartInspect()
    {
        if (inspecting)
        {
            Debug.LogError("[ObjectInspector] Ya está en modo inspección");
            return;
        }
        
        inspecting = true;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

    // Calcular la posición frente a la cámara usando inspectDistance
    Vector3 targetPosition = mainCamera.transform.position + mainCamera.transform.forward * inspectDistance;
    targetPosition.y += inspectHeightOffset;

    // Ajustar la posición para que el pivote quede en el punto de inspección
    Vector3 pivotWorldPos = transform.TransformPoint(pivotPoint.localPosition);
    Vector3 offset = pivotWorldPos - transform.position;
    transform.position = targetPosition - offset;

        // Alinear el objeto con la cámara, pero manteniendo la rotación original en el eje Y
        Vector3 lookDirection = (mainCamera.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Euler(originalRotation.eulerAngles.x, lookRotation.eulerAngles.y, originalRotation.eulerAngles.z);
        
        // Mostrar los números de los puntos de detalle
        foreach (var point in detailPoints)
        {
            if (point != null)
            {
                point.ShowPoint(true);
            }
            else
            {
                Debug.LogError("[ObjectInspector] ¡Se encontró un punto de detalle null!");
            }
        }
        
        // Mostrar panel de inspección de objeto y pasar referencia de este objeto
        if (panelManager != null)
        {
            panelManager.ShowObjectPanel();
            var objPanel = FindFirstObjectByType<ObjectPanelController>();
            if (objPanel != null)
            {
                objPanel.SetObject(this);
            }
            else
            {
                Debug.LogError("[ObjectInspector] ¡No se encontró ObjectPanelController en la escena!");
            }
        }
        else
        {
            Debug.LogError("[ObjectInspector] ¡PanelManager no está asignado!");
        }
    }

    void Update()
    {
        if (!inspecting) return;

        // Rotar el objeto solo con el click izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            previousMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 deltaMousePosition = Input.mousePosition - previousMousePosition;
            float rotationX = deltaMousePosition.y * rotationSpeed * Time.deltaTime;
            float rotationY = -deltaMousePosition.x * rotationSpeed * Time.deltaTime;
            
            // Crear la rotación alrededor del pivote
            Vector3 pivotWorldPos = transform.TransformPoint(pivotPoint.localPosition);
            transform.RotateAround(pivotWorldPos, transform.right, rotationX);
            transform.RotateAround(pivotWorldPos, Vector3.up, rotationY);
            
            previousMousePosition = Input.mousePosition;
        }

        // El modo inspección solo termina con el botón 'Atrás' en la UI
    }

    public void EndInspect()
    {
    inspecting = false;
    transform.position = originalPosition;
    transform.rotation = originalRotation;
        
        // Ocultar los números de los puntos de detalle
        // ShowPoint ya incluye lógica para deseleccionar puntos si es necesario
        foreach (var point in detailPoints)
        {
            if (point != null)
            {
                point.ShowPoint(false);
            }
        }
        
        // Volver al panel normal
        if (panelManager != null)
            panelManager.ShowNormalPanel();
        // Actualizar el estado global de inspección
        ObjectInfoRaycast.isInspecting = false;
    }

    // Método para mostrar la información de un punto de detalle
    public void ShowDetailPoint(int pointNumber, string description)
    {
        
        // Verificar que estamos en modo inspección
        if (!inspecting)
        {
            Debug.LogError("[ObjectInspector] Intentando mostrar detalles fuera del modo inspección");
            return;
        }
        
        if (panelManager != null && panelManager.partDetailPanel != null)
        {
            var partPanel = panelManager.partDetailPanel.GetComponent<PartDetailPanelController>();
            if (partPanel != null)
            {
                
                // Primero activamos el panel para asegurarnos de que el controlador funcione
                panelManager.ShowPartDetailPanel();
                
                // Ahora configuramos la información
                partPanel.SetPartDetail(pointNumber, description);
                
                // Ya no necesitamos gestionar los colores aquí, lo hace cada punto
            }
            else
            {
                Debug.LogError("[ObjectInspector] ¡Error! El panel de detalles no tiene el componente PartDetailPanelController");
            }
        }
        else
        {
            Debug.LogError("[ObjectInspector] ¡Error! No se encontró el PanelManager o el panel de detalles no está asignado");
        }
    }
}
