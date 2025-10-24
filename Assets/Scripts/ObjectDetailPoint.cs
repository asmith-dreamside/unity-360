using UnityEngine;

public class ObjectDetailPoint : MonoBehaviour
{
    [Header("Configuración")]
    public int pointNumber = 1;
    [TextArea(3,10)]
    public string detailDescription = "Descripción del punto";
    
    [Header("Apariencia")]
    public float sphereRadius = 0.1f;
    public Color normalColor = Color.yellow;
    public Color hoverColor = Color.red;
    public Color selectedColor = Color.green;
    
    // Referencias privadas
    private GameObject sphereObject;
    private Material sphereMaterial;
    private SphereCollider sphereCollider;
    private ObjectInspector parentInspector;
    private bool isInspectable = false;
    private bool isSelected = false;
    
    // Referencia estática al punto actualmente seleccionado
    private static ObjectDetailPoint currentlySelectedPoint = null;
    
    void Awake()
    {
        // Crear la esfera visual
        CreateSphere();
        
        // Obtener referencia al inspector padre
        parentInspector = GetComponentInParent<ObjectInspector>();
    }
    
    // Crear una esfera visual simple
    void CreateSphere()
    {
        // Crear un objeto para la esfera
        sphereObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphereObject.name = "DetailSphere";
        sphereObject.transform.parent = transform;
        sphereObject.transform.localPosition = Vector3.zero;
        sphereObject.transform.localScale = Vector3.one * sphereRadius * 2;
        
        // Obtener y configurar el material
        Renderer renderer = sphereObject.GetComponent<Renderer>();
        sphereMaterial = renderer.material;
        sphereMaterial.color = normalColor;
        
        // Eliminar el collider de la primitiva y crear uno propio
        Destroy(sphereObject.GetComponent<Collider>());
        
        // Crear un SphereCollider en este objeto
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.radius = sphereRadius;
        sphereCollider.isTrigger = true;
        
        // Ocultar al inicio
        sphereObject.SetActive(false);
    }
    
    public void ShowPoint(bool show)
    {
        if (sphereObject != null)
        {
            sphereObject.SetActive(show);
            isInspectable = show;
            
            if (!show && isSelected)
            {
                // Si estamos ocultando el punto y estaba seleccionado, deseleccionarlo
                Deselect();
            }
            
            // Resetear el color al normal cuando se muestra
            if (show)
            {
                SetSphereColor(normalColor);
            }
        }
    }
    
    public void SetSphereColor(Color color)
    {
        if (sphereMaterial != null)
        {
            sphereMaterial.color = color;
        }
    }
    
    // Método para seleccionar este punto
    public void Select()
    {
        // Si hay un punto seleccionado anteriormente, deseleccionarlo
        if (currentlySelectedPoint != null && currentlySelectedPoint != this)
        {
            currentlySelectedPoint.Deselect();
        }
        
        // Establecer este punto como el seleccionado
        currentlySelectedPoint = this;
        isSelected = true;
        SetSphereColor(selectedColor);
        
        // Mostrar la información del punto
        if (parentInspector != null)
        {
            parentInspector.ShowDetailPoint(pointNumber, detailDescription);
        }
    }
    
    // Método para deseleccionar este punto
    public void Deselect()
    {
        // Solo deseleccionar si este punto es el seleccionado
        if (isSelected)
        {
            isSelected = false;
            SetSphereColor(normalColor);
            
            // Limpiar la referencia estática si este era el punto seleccionado
            if (currentlySelectedPoint == this)
            {
                currentlySelectedPoint = null;
            }
        }
    }
    
    void OnMouseDown()
    {
        if (!isInspectable) return;
        Select();
    }
    
    void OnMouseEnter()
    {
        if (!isInspectable || isSelected) return;
        SetSphereColor(hoverColor);
    }
    
    void OnMouseExit()
    {
        if (!isInspectable || isSelected) return;
        SetSphereColor(normalColor);
    }
}