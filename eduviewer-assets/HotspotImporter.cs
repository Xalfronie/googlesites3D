// HotspotImporter.cs — EduViewer -> Unity glTFast
// Colocar en Assets/Scripts/, asignar modelRoot y dotPrefab en Inspector.
using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable] public class EVHotspot { public string id; public string label; public string description; public string position; public string normal; public string color; public string kind; public float arrowDeg; public string anchorNodeKey; public Vector3 offset; public string category; }
[System.Serializable] public class EVHotspotFile { public string version; public int count; public EVHotspot[] hotspots; }

public class HotspotImporter : MonoBehaviour {
    public Transform modelRoot;
    public GameObject hotspotPrefab;
    public string fileName = "modelo-3d-hotspots.json";
    void Start() {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(path)) { Debug.LogWarning("[EduViewer] hotspots.json no encontrado: " + path); return; }
        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<EVHotspotFile>(json);
        foreach (var hs in data.hotspots) {
            Vector3 pos = ParseVec3(hs.position);
            Transform anchor = null;
            if (!string.IsNullOrEmpty(hs.anchorNodeKey)) anchor = FindDeep(modelRoot, hs.anchorNodeKey);
            Vector3 world = anchor ? anchor.position + (hs.offset) : pos;
            var go = Instantiate(hotspotPrefab, world, Quaternion.identity, transform);
            go.name = hs.label;
            if (anchor) go.transform.SetParent(anchor, true);
        }
        Debug.Log("[EduViewer] Hotspots importados: " + data.count);
    }
    Vector3 ParseVec3(string s) { var p = s.Split(' '); return new Vector3(float.Parse(p[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(p[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(p[2], System.Globalization.CultureInfo.InvariantCulture)); }
    Transform FindDeep(Transform root, string key) { if (root == null || string.IsNullOrEmpty(key)) return null; string leaf = key.Contains("/") ? key.Substring(key.LastIndexOf('/') + 1) : key; foreach (var t in root.GetComponentsInChildren<Transform>(true)) if (t.name == leaf) return t; return null; }
}
