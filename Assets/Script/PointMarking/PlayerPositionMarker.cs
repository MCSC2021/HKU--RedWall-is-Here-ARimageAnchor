using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPositionMarker : MonoBehaviour {
    public Button storePositionButton;
    public Button saveDataButton;
    private Transform ScanPrefabsOffset;
    private List<Vector3> playerPositions = new List<Vector3>();

    //visual
    public GameObject spherePrefab; // Prefab for the sphere
    public LineRenderer lineRenderer; // LineRenderer for connecting spheres
    private List<GameObject> spawnedSpheres = new List<GameObject>();

    void Start() {
        storePositionButton.onClick.AddListener(StorePlayerPosition);
        saveDataButton.onClick.AddListener(SaveDataToFile);
    }

    public void SetNavigationBaseTransform(Transform transform) {
        ScanPrefabsOffset = transform;
    }

    void StorePlayerPosition() {
        if (ScanPrefabsOffset == null) return;
        Vector3 relativePosition = ScanPrefabsOffset.InverseTransformPoint(Camera.main.transform.position);
        playerPositions.Add(relativePosition);
        Debug.Log("Stored player position: " + relativePosition);

        // Spawn a sphere at the relative position
        GameObject sphere = Instantiate(spherePrefab, ScanPrefabsOffset);
        sphere.transform.localPosition = relativePosition;
        sphere.transform.localScale = Vector3.one * 0.2f; // Set sphere radius to 0.2
        spawnedSpheres.Add(sphere); 
        // Draw a line connecting to the last stored position
        if (spawnedSpheres.Count > 1) { 
            lineRenderer.positionCount = spawnedSpheres.Count;
            for (int i = 0; i < spawnedSpheres.Count; i++) {
                lineRenderer.SetPosition(i, spawnedSpheres[i].transform.position);
            }
        }
    }
    void SaveDataToFile() { 
        string folderPath = Path.Combine(Application.persistentDataPath, "ArVector3 data");
        if (!Directory.Exists(folderPath)) { 
            Directory.CreateDirectory(folderPath);
        } 
        string filePath = Path.Combine(folderPath, "PlayerPositions.json");
        string jsonData = JsonUtility.ToJson(new Serialization<Vector3>(playerPositions), true);
        File.WriteAllText(filePath, jsonData); 
        Debug.Log("Saved player positions to: " + filePath);
    }
}

[System.Serializable] 
public class Serialization<T> {
    public List<T> target;
    public Serialization(List<T> target) {
        this.target = target;
    }
}
