using UnityEngine;

public class TeleportDestination : MonoBehaviour
{
    public GameObject salaTarget; // GameObject destino de la sala (debe tener el componente Sala)

    void OnMouseDown()
    {
        // Mover la cámara principal a la posición de la sala
        var cam = Camera.main;
        if (cam != null && salaTarget != null)
        {
            cam.transform.position = salaTarget.transform.position;
            // ajusta al eje Y en 0
            cam.transform.position = new Vector3(cam.transform.position.x, 0f, cam.transform.position.z);
            cam.transform.rotation = salaTarget.transform.rotation;
        }
        // Actualizar el texto de ubicación actual usando CurrentLocationPanelController
        if (salaTarget != null)
        {
            var sala = salaTarget.GetComponent<Sala>();
            if (sala != null)
            {
                var locationPanelController = FindObjectOfType<CurrentLocationPanelController>();
                if (locationPanelController != null)
                    locationPanelController.SetLocation(sala.salaName);
            }
        }
    }
}
