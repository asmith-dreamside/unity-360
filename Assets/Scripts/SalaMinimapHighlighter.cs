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
        try
        {
            // Verificar referencias nulas
            if (mainCamera == null || salas == null || minimapIcons == null || locationText == null)
            {
                return;
            }

            // Verificar listas vacías o desiguales
            if (salas.Count == 0 || minimapIcons.Count == 0 || salas.Count != minimapIcons.Count)
            {
                return;
            }

            int closestIdx = -1;
            float minDist = float.MaxValue;
            Vector3 camPos = mainCamera.transform.position;

            // Buscar la sala más cercana
            for (int i = 0; i < salas.Count; i++)
            {
                if (salas[i] == null) continue;

                float dist = Vector3.Distance(camPos, salas[i].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestIdx = i;
                }
            }

            // Actualizar colores de los íconos RawImage y el texto
            if (closestIdx >= 0 && closestIdx < salas.Count)
            {
                for (int i = 0; i < minimapIcons.Count; i++)
                {
                    if (minimapIcons[i] != null)
                    {
                        minimapIcons[i].color = (i == closestIdx) ? highlightColor : defaultColor;
                    }
                }

                if (salas[closestIdx] != null)
                {
                    string nombreSala = salas[closestIdx].salaName;
                    if (!string.IsNullOrEmpty(nombreSala))
                    {
                        locationText.text = nombreSala;
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error en SalaMinimapHighlighter: {e.Message}");
        }
    }
}
