using UnityEngine;
using TMPro;

[DefaultExecutionOrder(-50)] // Ejecutar antes que otros scripts
public class TextMeshProMaterialFixer : MonoBehaviour
{
    [Header("Configuración")]
    public bool fixOnStart = true;
    public bool showDetailedLogs = true;
    
    void Start()
    {
        if (fixOnStart)
        {
            FixAllTextMeshProMaterials();
        }
    }
    
    [ContextMenu("Arreglar Todos los TextMeshPro")]
    public void FixAllTextMeshProMaterials()
    {
        Debug.Log("=== INICIANDO ARREGLO DE MATERIALES TEXTMESHPRO ===");
        
        // Buscar LiberationSans SDF
        TMP_FontAsset liberationSansFont = FindLiberationSansFont();
        
        if (liberationSansFont == null)
        {
            Debug.LogError("[MaterialFixer] ¡No se pudo encontrar LiberationSans SDF! Usando fuente por defecto.");
            liberationSansFont = TMP_Settings.defaultFontAsset;
        }
        
        if (liberationSansFont == null)
        {
            Debug.LogError("[MaterialFixer] ¡No hay fuente por defecto disponible!");
            return;
        }
        
        Debug.Log($"[MaterialFixer] Usando fuente: {liberationSansFont.name}");
        
        // Encontrar todos los TextMeshPro en la escena (incluyendo inactivos)
        TextMeshProUGUI[] allTextMeshPros = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        Debug.Log($"[MaterialFixer] Encontrados {allTextMeshPros.Length} componentes TextMeshProUGUI");
        
        int fixedCount = 0;
        int errorCount = 0;
        
        foreach (TextMeshProUGUI tmp in allTextMeshPros)
        {
            // Saltar TextMeshPros que son parte de prefabs no instanciados
            if (tmp.gameObject.scene.name == null)
                continue;
                
            try
            {
                bool needsFix = false;
                string statusBefore = "";
                
                if (tmp.font == null)
                {
                    needsFix = true;
                    statusBefore = "Sin fuente";
                }
                else if (!tmp.font.name.Contains("LiberationSans") && !tmp.font.name.Contains("SDF"))
                {
                    needsFix = true;
                    statusBefore = $"Fuente incorrecta: {tmp.font.name}";
                }
                
                if (needsFix)
                {
                    // Aplicar LiberationSans SDF
                    tmp.font = liberationSansFont;
                    
                    // Aplicar material si está disponible
                    if (liberationSansFont.material != null)
                    {
                        tmp.fontMaterial = liberationSansFont.material;
                    }
                    
                    // Asegurar visibilidad
                    tmp.enabled = true;
                    tmp.gameObject.SetActive(true);
                    
                    // Asegurar color visible
                    Color color = tmp.color;
                    if (color.a < 0.1f)
                    {
                        color.a = 1f;
                        tmp.color = color;
                    }
                    
                    fixedCount++;
                    
                    if (showDetailedLogs)
                    {
                        Debug.Log($"[MaterialFixer] ARREGLADO: {GetTextMeshProPath(tmp)} - {statusBefore} → {liberationSansFont.name}");
                    }
                }
                else
                {
                    if (showDetailedLogs)
                    {
                        Debug.Log($"[MaterialFixer] OK: {GetTextMeshProPath(tmp)} - Ya tiene fuente correcta: {tmp.font.name}");
                    }
                }
            }
            catch (System.Exception e)
            {
                errorCount++;
                Debug.LogError($"[MaterialFixer] ERROR procesando {GetTextMeshProPath(tmp)}: {e.Message}");
            }
        }
        
        Debug.Log($"=== ARREGLO COMPLETADO ===");
        Debug.Log($"[MaterialFixer] Total procesados: {allTextMeshPros.Length}");
        Debug.Log($"[MaterialFixer] Arreglados: {fixedCount}");
        Debug.Log($"[MaterialFixer] Errores: {errorCount}");
        Debug.Log($"[MaterialFixer] Ya correctos: {allTextMeshPros.Length - fixedCount - errorCount}");
    }
    
    TMP_FontAsset FindLiberationSansFont()
    {
        // Buscar en varias ubicaciones posibles
        string[] searchPaths = {
            "Fonts & Materials/LiberationSans SDF",
            "LiberationSans SDF",
            "TextMeshPro/Fonts/LiberationSans SDF",
            "Fonts/LiberationSans SDF"
        };
        
        foreach (string path in searchPaths)
        {
            TMP_FontAsset font = Resources.Load<TMP_FontAsset>(path);
            if (font != null)
            {
                Debug.Log($"[MaterialFixer] LiberationSans SDF encontrado en: {path}");
                return font;
            }
        }
        
        // Buscar por nombre en todos los recursos
        TMP_FontAsset[] allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        foreach (TMP_FontAsset font in allFonts)
        {
            if (font.name.Contains("LiberationSans") && font.name.Contains("SDF"))
            {
                Debug.Log($"[MaterialFixer] LiberationSans SDF encontrado por búsqueda: {font.name}");
                return font;
            }
        }
        
        Debug.LogWarning("[MaterialFixer] No se encontró LiberationSans SDF en ninguna ubicación");
        return null;
    }
    
    string GetTextMeshProPath(TextMeshProUGUI tmp)
    {
        string path = tmp.name;
        Transform parent = tmp.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
    
    [ContextMenu("Mostrar Información de Fuentes")]
    public void ShowFontsInfo()
    {
        Debug.Log("=== INFORMACIÓN DE FUENTES TEXTMESHPRO ===");
        
        TMP_FontAsset[] allFonts = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
        Debug.Log($"Total de fuentes encontradas: {allFonts.Length}");
        
        foreach (TMP_FontAsset font in allFonts)
        {
            Debug.Log($"Fuente: {font.name} - Material: {(font.material != null ? font.material.name : "NULL")}");
        }
        
        Debug.Log($"Fuente por defecto de TMP: {(TMP_Settings.defaultFontAsset != null ? TMP_Settings.defaultFontAsset.name : "NULL")}");
    }
}