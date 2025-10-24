using UnityEngine;

[CreateAssetMenu(menuName = "Dreamside/ObjectData")]
public class ObjectData : ScriptableObject
{
    public string objectName;
    [TextArea]
    public string objectSummary;
    public Texture objectImage;
    // Puedes agregar más campos si lo necesitas
}
