using UnityEngine;
using TMPro;

public class CurrentLocationPanelController : MonoBehaviour
{
    public TMP_Text locationText;

    // Llama este método para actualizar el nombre de la sala
    public void SetLocation(string locationName)
    {
        if (locationText != null)
            locationText.text = locationName;
    }
}
