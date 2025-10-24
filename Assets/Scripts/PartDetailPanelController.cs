using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartDetailPanelController : MonoBehaviour
{
    public TextMeshProUGUI partNameText;
    public TextMeshProUGUI partDescriptionText;
    public Button backButton;
    public PanelManager panelManager;

    void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackButton);
    }

    // Llamar este método para mostrar el detalle de una parte
    public void SetPartDetail(int partNumber, string description)
    {
        if (partNameText != null)
            partNameText.text = $"Parte {partNumber}";
        if (partDescriptionText != null)
            partDescriptionText.text = description;
    }

    void OnBackButton()
    {
        if (panelManager != null)
        {
            // Buscar el punto actualmente seleccionado y deseleccionarlo
            var allPoints = FindObjectsByType<ObjectDetailPoint>(FindObjectsSortMode.None);
            foreach (var point in allPoints)
            {
                // Usamos el método deselect que hemos creado
                point.Deselect();
            }
            
            panelManager.ShowObjectPanel();
        }
    }
}
