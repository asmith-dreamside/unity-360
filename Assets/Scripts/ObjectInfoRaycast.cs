using UnityEngine;
using TMPro;

public class ObjectInfoRaycast : MonoBehaviour
{
    public static bool isInspecting = false;

    public Camera mainCamera;
    public TMP_Text objectNameText; // Texto para el nombre
    public TMP_Text infoText;       // Texto para el detalle
    public UnityEngine.UI.Image reticleImage; // Imagen de la retícula en el Canvas
    private float rayDistance = 100f;
    
    [Header("Configuración de Retícula")]
    public float reticleRadius = 50f; // Radio en píxeles desde el centro para activar raycast

    private ObjectInspector currentInspector;
    private ObjectID currentObjectID;
    
    // Propiedad para verificar si estamos inspeccionando
    public bool IsCurrentlyInspecting => currentInspector != null;

    // Verificar si la posición del click/touch está dentro del radio de la retícula
    private bool IsClickInsideReticle(Vector2 inputPosition)
    {
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        float distanceFromCenter = Vector2.Distance(inputPosition, screenCenter);
        return distanceFromCenter <= reticleRadius;
    }

    // Método para visualizar el radio de la retícula en el Scene View (solo para debug)
    void OnDrawGizmos()
    {
        if (mainCamera != null && reticleImage != null)
        {
            // Mostrar el radio de la retícula como una esfera de wireframe en la escena
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 10f);
            Vector3 worldCenter = mainCamera.ScreenToWorldPoint(screenCenter);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(worldCenter, 0.1f);
        }
    }

// Limpiar la inspección actual
    public void ClearInspection()
    {
        currentInspector = null;
        currentObjectID = null;
        if (objectNameText != null) objectNameText.enabled = false;
        if (infoText != null) infoText.enabled = false;
        isInspecting = false;
        ObjectInfoRaycast.isInspecting = false; // Actualizar la variable estática
    }

    void Update()
    {
        // Si estamos inspeccionando, mostrar info pero NO hacer más raycast ni interacciones
        if (currentInspector != null)
        {
            isInspecting = true;
            ObjectInfoRaycast.isInspecting = true; // Actualizar la variable estática
            
            if (currentObjectID != null)
            {
                objectNameText.text = currentObjectID.objectName;
                infoText.text = currentObjectID.infoText;
                objectNameText.enabled = true;
                infoText.enabled = true;
            }
            
            // Mantener la retícula en color verde mientras inspeccionamos
            if (reticleImage != null)
                reticleImage.color = Color.green;
                
            return; // IMPORTANTE: Salir aquí para no hacer más raycast
        }
        else
        {
            isInspecting = false;
            ObjectInfoRaycast.isInspecting = false; // Actualizar la variable estática
        }

        // Verificar que tenemos una cámara válida
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindObjectOfType<Camera>();
            }
            if (mainCamera == null)
            {
                return; // No podemos hacer raycast sin cámara
            }
        }
        
        // Raycast desde el centro de la pantalla (retícula)
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        // Detectar interacción: mouse click (PC) o toque (móvil/WebGL)
        // SOLO si el click/touch está dentro del radio de la retícula
        bool interact = false;
        bool isTouch = false;
        
        // Sistema unificado - verificar touch primero
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Solo interactuar si el toque está cerca de la retícula
                if (IsClickInsideReticle(touch.position))
                {
                    interact = true;
                    isTouch = true;
                }
            }
        }
        // Si no hay touch, usar mouse (PC, WebGL)
        else if (Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
        {
            // Solo interactuar si el click está cerca de la retícula
            if (IsClickInsideReticle(Input.mousePosition))
            {
                interact = true;
            }
        }

        // Si el raycast golpea un objeto con ObjectID
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Obtener componene ObjectID y ObjectInspector
            var objID = hit.collider.GetComponent<ObjectID>();
            var inspector = hit.collider.GetComponent<ObjectInspector>();

            if (objID != null)
            {
                // Mostrar información del ObjectID
                objectNameText.text = objID.objectName;
                infoText.text = objID.infoText;
                objectNameText.enabled = true;
                infoText.enabled = true;

                // Cambiar color de la retícula a verde si hay info
                if (reticleImage != null)
                    reticleImage.color = Color.green;

                // Solo permite inspeccionar si hay ObjectInspector y hay interacción
                if (inspector != null && interact)
                {
                    currentInspector = inspector;
                    currentObjectID = objID;
                    inspector.StartInspect();
                    isInspecting = true;
                    ObjectInfoRaycast.isInspecting = true; // Actualizar inmediatamente la variable estática
                }
                // Si solo hay ObjectID (sin inspector), permitir mostrar en panel al hacer click/toque
                else if (inspector == null && interact)
                {
                    // Buscar PanelManager y mostrar información en el panel de objeto
                    var panelManager = FindObjectOfType<PanelManager>();
                    if (panelManager != null)
                    {
                        var objPanel = FindObjectOfType<ObjectPanelController>();
                        if (objPanel != null)
                        {
                            objPanel.SetObjectFromID(objID);
                        }
                    }
                }
            }
            else
            {
                objectNameText.enabled = false;
                infoText.enabled = false;
                // Cambiar color de la retícula a blanco si no hay info
                if (reticleImage != null)
                    reticleImage.color = Color.white;
            }
        }
        else
        {
            objectNameText.enabled = false;
            infoText.enabled = false;
            // Cambiar color de la retícula a blanco si no hay nada
            if (reticleImage != null)
                reticleImage.color = Color.white;
        }
    }
}
