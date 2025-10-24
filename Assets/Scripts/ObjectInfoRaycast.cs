using UnityEngine;
using TMPro;

public class ObjectInfoRaycast : MonoBehaviour
{
    public static bool isInspecting = false;

    public Camera mainCamera;
    public TMP_Text objectNameText; // Texto para el nombre
    public TMP_Text infoText;       // Texto para el detalle
    private float rayDistance = 100f;

    private ObjectInspector currentInspector;
    private ObjectID currentObjectID;

// Limpiar la inspección actual
    public void ClearInspection()
    {
        currentInspector = null;
        currentObjectID = null;
        if (objectNameText != null) objectNameText.enabled = false;
        if (infoText != null) infoText.enabled = false;
        isInspecting = false;
    }

    void Update()
    {
        // Si estamos inspeccionando, mostrar info y salir con Escape
        if (currentInspector != null)
        {
            isInspecting = true;
            if (currentObjectID != null)
            {
                objectNameText.text = currentObjectID.objectName;
                infoText.text = currentObjectID.infoText;
                objectNameText.enabled = true;
                infoText.enabled = true;
            }
            return;
        }
        else
        {
            isInspecting = false;
        }
        // Raycast desde la cámara hacia el punto del mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

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
                
                // Solo permite inspeccionar si hay ObjectInspector y el botón derecho NO está presionado
                if (inspector != null && Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
                {
                    currentInspector = inspector;
                    currentObjectID = objID;
                    inspector.StartInspect();
                    isInspecting = true;
                }
                // Si solo hay ObjectID (sin inspector), permitir mostrar en panel al hacer click
                else if (inspector == null && Input.GetMouseButtonDown(0) && !Input.GetMouseButton(1))
                {
                    // Buscar PanelManager y mostrar información en el panel de objeto
                    var panelManager = FindFirstObjectByType<PanelManager>();
                    if (panelManager != null)
                    {
                        var objPanel = FindFirstObjectByType<ObjectPanelController>();
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
            }
        }
        else
        {
            objectNameText.enabled = false;
            infoText.enabled = false;
        }
    }
}
