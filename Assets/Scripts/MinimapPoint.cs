using UnityEngine;
using UnityEngine.UI;

public class MinimapPoint : MonoBehaviour
{
    public Sala linkedSala; // La sala que este punto representa
    public Image pointImage; // Imagen del punto en el minimapa
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    private RectTransform rectTransform;
    private bool isActive = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (pointImage == null)
            pointImage = GetComponent<Image>();
    }

    public void SetActive(bool active)
    {
        isActive = active;
        if (pointImage != null)
            pointImage.color = active ? activeColor : inactiveColor;
    }

    // Actualiza la posición del punto en el minimapa basado en la posición mundial de la sala
    public void UpdatePosition(Vector2 worldToMinimapScale)
    {
        if (linkedSala != null)
        {
            Vector3 worldPos = linkedSala.transform.position;
            // Convierte la posición mundial a coordenadas del minimapa
            Vector2 minimapPos = new Vector2(
                worldPos.x * worldToMinimapScale.x,
                worldPos.z * worldToMinimapScale.y
            );
            rectTransform.anchoredPosition = minimapPos;
        }
    }
}