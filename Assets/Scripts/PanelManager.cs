using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    public GameObject normalPanel;
    public GameObject objectPanel;
    public GameObject partDetailPanel;  // Mantenemos público para acceso desde ObjectInspector
    public GameObject currentLocationPanel; // Nuevo panel para ubicación actual
    public Button backButton;

    // Referencias a los controladores
    private PartDetailPanelController partDetailController;

    void Start()
    {
            Debug.Log("[PanelManager] Iniciando configuración...");
            // Siempre activar primero el normalPanel y desactivar los demás
            if (normalPanel != null)
            {
                normalPanel.SetActive(true);
                if (objectPanel != null) objectPanel.SetActive(false);
                if (partDetailPanel != null) partDetailPanel.SetActive(false);
                if (currentLocationPanel != null) currentLocationPanel.SetActive(false);
                Debug.Log("[PanelManager] normalPanel activado como el primer panel");
            }
            else
            {
                Debug.LogError("[PanelManager] ERROR: normalPanel no está asignado en el Inspector");
            }

            // Verificar todos los paneles
            Debug.Log($"[PanelManager] Estado de los paneles: Normal({normalPanel != null}), Object({objectPanel != null}), PartDetail({partDetailPanel != null})");

            // Inicializar referencias
            if (partDetailPanel != null)
            {
                Debug.Log($"[PanelManager] Buscando PartDetailPanelController en {partDetailPanel.name}");
                partDetailController = partDetailPanel.GetComponent<PartDetailPanelController>();
                if (partDetailController == null)
                {
                    Debug.LogError($"[PanelManager] ERROR: El panel de detalles '{partDetailPanel.name}' no tiene el componente PartDetailPanelController");
                }
                else
                {
                    Debug.Log("[PanelManager] PartDetailPanelController encontrado correctamente");
                }
            }
            else
            {
                Debug.LogError("[PanelManager] ERROR: Panel de detalles (partDetailPanel) no está asignado en el Inspector");
            }

            // Solo mostrar el panel de ubicación si el nombre no está vacío
            ShowCurrentLocationPanel(""); // Esto ahora no desactivará el normalPanel si el nombre está vacío
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackButton);
                Debug.Log("[PanelManager] Botón Back configurado correctamente");
            }
            else
            {
                Debug.LogWarning("[PanelManager] Advertencia: No hay botón Back asignado");
            }
    }

    public void ShowNormalPanel()
    {
    if (normalPanel != null) normalPanel.SetActive(true);
    if (objectPanel != null) objectPanel.SetActive(false);
    if (partDetailPanel != null) partDetailPanel.SetActive(false);
    if (currentLocationPanel != null) currentLocationPanel.SetActive(false);
    }

    public void ShowObjectPanel()
    {
    if (normalPanel != null) normalPanel.SetActive(false);
    if (objectPanel != null) objectPanel.SetActive(true);
    if (partDetailPanel != null) partDetailPanel.SetActive(false);
    if (currentLocationPanel != null) currentLocationPanel.SetActive(false);
    }

    public void ShowPartDetailPanel()
    {
        Debug.Log("[PanelManager] Intentando mostrar el panel de detalles...");
        
        if (partDetailPanel == null)
        {
            Debug.LogError("[PanelManager] ERROR: El partDetailPanel no está asignado en el inspector. Por favor, asigna el panel de detalles en el componente PanelManager.");
            return;
        }

        // Verificar que el panel tiene el controlador
        var controller = partDetailPanel.GetComponent<PartDetailPanelController>();
        if (controller == null)
        {
            Debug.LogError($"[PanelManager] ERROR: El panel '{partDetailPanel.name}' no tiene el componente PartDetailPanelController");
            return;
        }

    if (normalPanel != null) normalPanel.SetActive(false);
    if (objectPanel != null) objectPanel.SetActive(false);
    if (currentLocationPanel != null) currentLocationPanel.SetActive(false);
    partDetailPanel.SetActive(true);
        
        Debug.Log($"[PanelManager] Panel de detalles '{partDetailPanel.name}' mostrado correctamente");
    }

    public void HideAllPanels()
    {
    if (normalPanel != null) normalPanel.SetActive(false);
    if (objectPanel != null) objectPanel.SetActive(false);
    if (partDetailPanel != null) partDetailPanel.SetActive(false);
    if (currentLocationPanel != null) currentLocationPanel.SetActive(false);
    }

    public void OnBackButton()
    {
        // Volver al panel de objeto si estamos en detalle de parte
        ShowObjectPanel();
    }

    // Nuevo método para mostrar el panel de ubicación actual y actualizar el nombre
    public void ShowCurrentLocationPanel(string locationName)
    {
        if (string.IsNullOrEmpty(locationName))
        {
            // Si el nombre está vacío, mostrar el normalPanel y ocultar el de ubicación
            if (normalPanel != null) normalPanel.SetActive(true);
            if (currentLocationPanel != null) currentLocationPanel.SetActive(false);
            if (objectPanel != null) objectPanel.SetActive(false);
            if (partDetailPanel != null) partDetailPanel.SetActive(false);
        }
        else
        {
            if (currentLocationPanel != null)
            {
                currentLocationPanel.SetActive(true);
                // Buscar el componente de texto y actualizarlo
                var text = currentLocationPanel.GetComponentInChildren<TMPro.TMP_Text>();
                if (text != null)
                    text.text = locationName;
            }
            // Ocultar los otros paneles
            if (normalPanel != null) normalPanel.SetActive(false);
            if (objectPanel != null) objectPanel.SetActive(false);
            if (partDetailPanel != null) partDetailPanel.SetActive(false);
        }
    }
    // Método público para actualizar solo el texto de ubicación actual
    public void UpdateCurrentLocationText(string locationName)
    {
        if (currentLocationPanel != null)
        {
            var text = currentLocationPanel.GetComponentInChildren<TMPro.TMP_Text>();
            if (text != null)
                text.text = locationName;
        }
    }

    // La funcionalidad de preguntas ahora se maneja en Manager.cs
}
