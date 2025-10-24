using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public Camera mainCamera; // Referencia a la cámara principal
    public RectTransform[] minimapPoints; // Puntos 2D en el minimapa (UI)
    public Vector3[] worldPositions; // Posiciones reales en el mundo para cada punto
    public Color defaultColor = Color.gray;
    public Color highlightColor = Color.red;

    void Update()
    {
        if (mainCamera == null || minimapPoints == null || worldPositions == null) return;
        if (minimapPoints.Length != worldPositions.Length) return;

        // Encuentra el punto más cercano a la cámara
        int closestIdx = -1;
        float minDist = float.MaxValue;
        Vector3 camPos = mainCamera.transform.position;
        for (int i = 0; i < worldPositions.Length; i++)
        {
            float dist = Vector3.Distance(camPos, worldPositions[i]);
            if (dist < minDist)
            {
                minDist = dist;
                closestIdx = i;
            }
        }

        // Actualiza los colores de los puntos
        for (int i = 0; i < minimapPoints.Length; i++)
        {
            var img = minimapPoints[i].GetComponent<Image>();
            if (img != null)
                img.color = (i == closestIdx) ? highlightColor : defaultColor;
        }
    }
}