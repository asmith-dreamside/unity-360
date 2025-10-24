using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class SalaMinimapHighlighter : MonoBehaviour
{
    public Camera mainCamera;
    public List<Sala> salas; // Lista de objetos Sala (ordenados)
    public List<RawImage> minimapIcons; // Lista de íconos RawImage en el minimapa (mismo orden que salas)
    public Color defaultColor = Color.gray;
    public Color highlightColor = Color.green;
    public TMP_Text locationText; // Texto de ubicación actual

    void Update()
    {
        if (mainCamera == null || salas == null || minimapIcons == null) return;
        if (salas.Count != minimapIcons.Count) return;

        int closestIdx = -1;
        float minDist = float.MaxValue;
        Vector3 camPos = mainCamera.transform.position;

        // Buscar la sala más cercana
        for (int i = 0; i < salas.Count; i++)
        {
            float dist = Vector3.Distance(camPos, salas[i].transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closestIdx = i;
                
            }
        }

        // Actualizar colores de los íconos RawImage
        for (int i = 0; i < minimapIcons.Count; i++)
        {
            if (minimapIcons[i] != null)
            {
                minimapIcons[i].color = (i == closestIdx) ? highlightColor : defaultColor;
                string nombreSala = salas[closestIdx].salaName;
                locationText.text = nombreSala;
                
            }
        }

        
    }
}
