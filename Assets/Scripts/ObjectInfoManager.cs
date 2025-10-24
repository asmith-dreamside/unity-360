using UnityEngine;
using System.Collections.Generic;

public class ObjectInfoManager : MonoBehaviour
{
    [System.Serializable]
    public class ObjectInfo
    {
        public string objectID;
        public string infoText;
        public string detailText;
        public GameObject objectPrefab; // Para la escena de detalle
    }

    public List<ObjectInfo> objectsInfo = new List<ObjectInfo>();
    private Dictionary<string, ObjectInfo> infoDict;

    void Awake()
    {
        infoDict = new Dictionary<string, ObjectInfo>();
        foreach (var obj in objectsInfo)
        {
            infoDict[obj.objectID] = obj;
        }
    }

    public string GetInfoText(string objectID)
    {
        if (infoDict.ContainsKey(objectID))
            return infoDict[objectID].infoText;
        return "";
    }

    public ObjectInfo GetObjectInfo(string objectID)
    {
        if (infoDict.ContainsKey(objectID))
            return infoDict[objectID];
        return null;
    }
}
