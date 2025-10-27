using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectPanelController : MonoBehaviour
{
    public TextMeshProUGUI objectNameText;
    public TextMeshProUGUI objectSummaryText;
    public RawImage objectImage;
    public Button backButton;
    public PanelManager panelManager;

    // Referencia al objeto inspeccionado
    private ObjectInspector currentInspector;

    void Start()
    {
        Debug.Log("Iniciando ObjectPanelController"); // Para debug
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners(); // Limpiar listeners previos
            backButton.onClick.AddListener(OnBackButton);
            Debug.Log("Botón Atrás conectado correctamente"); // Para debug
        }
        else
        {
            Debug.LogWarning("¡Botón Atrás no asignado en ObjectPanelController!"); // Para debug
        }
    }

    // Llamar este método cuando se inspecciona un objeto
    public void SetObject(ObjectInspector inspector)
    {
        currentInspector = inspector;
        if (objectNameText != null)
            objectNameText.text = inspector.objectName;
        if (objectSummaryText != null)
            objectSummaryText.text = inspector.objectSummary;
        if (objectImage != null)
            objectImage.texture = inspector.objectImage;
        // Mostrar panel solo si hay objeto
        if (panelManager == null)
        {
            panelManager = FindObjectOfType<PanelManager>();
        }
        if (panelManager != null)
            panelManager.ShowObjectPanel();
    }

    // Método alternativo para mostrar información de ObjectID sin ObjectInspector
    public void SetObjectFromID(ObjectID objectID)
    {
        currentInspector = null; // No hay inspector en este caso
        if (objectNameText != null)
            objectNameText.text = objectID.objectName;
        if (objectSummaryText != null)
            objectSummaryText.text = objectID.infoText; // Usamos infoText como summary
        if (objectImage != null)
            objectImage.texture = null; // Sin imagen disponible en ObjectID
        // Mostrar panel de objeto
        if (panelManager == null)
        {
            panelManager = FindObjectOfType<PanelManager>();
        }
        if (panelManager != null)
            panelManager.ShowObjectPanel();
    }

    // Llamar este método desde el botón o evento de parte seleccionada
    public void OnPartSelected(int partNumber, string description)
    {
        if (panelManager != null)
            panelManager.ShowPartDetailPanel();
        var partPanel = FindObjectOfType<PartDetailPanelController>();
        if (partPanel != null)
            partPanel.SetPartDetail(partNumber, description);
    }

    public void OnBackButton()
    {
        Debug.Log("Botón Atrás presionado"); // Para debug
        
        // Terminar inspección del objeto actual y asegurarse de que regrese a su posición
        if (currentInspector != null)
        {
            Debug.Log("Terminando inspección del objeto: " + currentInspector.objectName); // Para debug
            currentInspector.EndInspect(); // Esto moverá el objeto a su posición original
            currentInspector = null;
        }
        
        // Buscar y limpiar el ObjectInfoRaycast actual
        var infoRaycast = FindObjectOfType<ObjectInfoRaycast>();
        if (infoRaycast != null)
        {
            infoRaycast.ClearInspection();
        }

        // Mostrar panel normal después de terminar la inspección
        if (panelManager != null)
        {
            Debug.Log("Mostrando panel normal"); // Para debug
            panelManager.ShowNormalPanel();
        }
    }
}
