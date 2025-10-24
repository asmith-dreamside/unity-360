using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystemDiagnostic : MonoBehaviour
{
    [Header("Ejecutar Diagnóstico")]
    public bool runDiagnostic = false;
    
    void Update()
    {
        if (runDiagnostic)
        {
            runDiagnostic = false;
            RunCompleteDiagnostic();
        }
    }
    
    [ContextMenu("Ejecutar Diagnóstico Completo")]
    public void RunCompleteDiagnostic()
    {
        Debug.Log("=== DIAGNÓSTICO COMPLETO DEL SISTEMA UI ===");
        
        // 1. Verificar Canvas
        DiagnoseCanvas();
        
        // 2. Verificar PanelManager
        DiagnosePanelManager();
        
        // 3. Verificar NormalPanelController
        DiagnoseNormalPanelController();
        
        // 4. Verificar TeleportDestination
        DiagnoseTeleportDestination();
        
        // 5. Verificar toda la jerarquía UI
        DiagnoseUIHierarchy();
        
        Debug.Log("=== FIN DEL DIAGNÓSTICO ===");
    }
    
    void DiagnoseCanvas()
    {
        Debug.Log("--- DIAGNÓSTICO CANVAS ---");
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        Debug.Log($"Número de Canvas encontrados: {canvases.Length}");
        
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"Canvas: {canvas.name}");
            Debug.Log($"  - Activo: {canvas.gameObject.activeInHierarchy}");
            Debug.Log($"  - Enabled: {canvas.enabled}");
            Debug.Log($"  - Render Mode: {canvas.renderMode}");
            Debug.Log($"  - Sort Order: {canvas.sortingOrder}");
            
            if (canvas.worldCamera != null)
                Debug.Log($"  - World Camera: {canvas.worldCamera.name}");
            else
                Debug.Log("  - World Camera: null");
        }
    }
    
    void DiagnosePanelManager()
    {
        Debug.Log("--- DIAGNÓSTICO PANEL MANAGER ---");
        PanelManager panelManager = FindFirstObjectByType<PanelManager>();
        
        if (panelManager == null)
        {
            Debug.LogError("PanelManager NO ENCONTRADO!");
            return;
        }
        
        Debug.Log($"PanelManager encontrado en: {panelManager.name}");
        Debug.Log($"  - Activo: {panelManager.gameObject.activeInHierarchy}");
        Debug.Log($"  - Enabled: {panelManager.enabled}");
        
        // Usar reflexión para acceder a los campos privados
        var fields = typeof(PanelManager).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(GameObject))
            {
                GameObject panel = (GameObject)field.GetValue(panelManager);
                Debug.Log($"  - {field.Name}: {(panel != null ? panel.name : "NULL")}");
                if (panel != null)
                {
                    Debug.Log($"    - Activo: {panel.activeInHierarchy}");
                }
            }
        }
    }
    
    void DiagnoseNormalPanelController()
    {
        Debug.Log("--- DIAGNÓSTICO NORMAL PANEL CONTROLLER ---");
        NormalPanelController controller = FindFirstObjectByType<NormalPanelController>();
        
        if (controller == null)
        {
            Debug.LogError("NormalPanelController NO ENCONTRADO!");
            return;
        }
        
        Debug.Log($"NormalPanelController encontrado en: {controller.name}");
        Debug.Log($"  - Activo: {controller.gameObject.activeInHierarchy}");
        Debug.Log($"  - Enabled: {controller.enabled}");
        
        // Verificar los campos públicos de texto
        var fields = typeof(NormalPanelController).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.FieldType == typeof(TextMeshProUGUI))
            {
                TextMeshProUGUI textComponent = (TextMeshProUGUI)field.GetValue(controller);
                Debug.Log($"  - {field.Name}: {(textComponent != null ? textComponent.name : "NULL")}");
                
                if (textComponent != null)
                {
                    Debug.Log($"    - GameObject Activo: {textComponent.gameObject.activeInHierarchy}");
                    Debug.Log($"    - Component Enabled: {textComponent.enabled}");
                    Debug.Log($"    - Texto actual: '{textComponent.text}'");
                    Debug.Log($"    - Color: {textComponent.color}");
                    Debug.Log($"    - Font Size: {textComponent.fontSize}");
                    Debug.Log($"    - Canvas Renderer Cull: {textComponent.canvasRenderer.cull}");
                }
            }
        }
    }
    
    void DiagnoseTeleportDestination()
    {
        Debug.Log("--- DIAGNÓSTICO TELEPORT DESTINATION ---");
        TeleportDestination[] destinations = FindObjectsOfType<TeleportDestination>();
        Debug.Log($"Número de TeleportDestination encontrados: {destinations.Length}");
        
        foreach (TeleportDestination dest in destinations)
        {
            Debug.Log($"TeleportDestination: {dest.name}");
            Debug.Log($"  - Activo: {dest.gameObject.activeInHierarchy}");
            Debug.Log($"  - Enabled: {dest.enabled}");
            
            Collider col = dest.GetComponent<Collider>();
            if (col != null)
            {
                Debug.Log($"  - Collider: {col.GetType().Name}, Enabled: {col.enabled}");
            }
            else
            {
                Debug.LogWarning($"  - NO TIENE COLLIDER!");
            }
        }
    }
    
    void DiagnoseUIHierarchy()
    {
        Debug.Log("--- DIAGNÓSTICO JERARQUÍA UI ---");
        
        // Buscar todos los TextMeshProUGUI en la escena
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true); // incluye inactivos
        Debug.Log($"Total de componentes TextMeshProUGUI encontrados: {allTexts.Length}");
        
        foreach (TextMeshProUGUI text in allTexts)
        {
            Debug.Log($"TextMeshProUGUI: {text.name}");
            Debug.Log($"  - Path: {GetGameObjectPath(text.gameObject)}");
            Debug.Log($"  - Activo en jerarquía: {text.gameObject.activeInHierarchy}");
            Debug.Log($"  - Activo en sí mismo: {text.gameObject.activeSelf}");
            Debug.Log($"  - Component enabled: {text.enabled}");
            Debug.Log($"  - Texto: '{text.text}'");
            Debug.Log($"  - Color: {text.color}");
            Debug.Log($"  - Alpha: {text.color.a}");
        }
    }
    
    string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}