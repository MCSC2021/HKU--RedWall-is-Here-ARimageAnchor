using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class InDoorNav : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] private GameObject trackedImagePrefab;
    [SerializeField] private LineRenderer line;
    [SerializeField] private float therohold;

    private List<NavigationTarget> navigationTargets = new List<NavigationTarget>();
    private NavMeshSurface navMeshSurface;
    private NavMeshPath navMeshPath;

    private GameObject navigationBase;
    private Vector3 lastposition;
    

    private void Start()
    {
        navMeshPath = new NavMeshPath();

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private void Update()
    {
        if (navigationBase != null && navigationTargets.Count>0 && navMeshSurface != null)
        {
            if (Vector3.Distance(player.position, lastposition)<therohold) return;
            NavMesh.CalculatePath(player.position, navigationTargets[0].transform.position, NavMesh.AllAreas, navMeshPath);

            if (navMeshPath.status== NavMeshPathStatus.PathComplete) {
                line.positionCount = navMeshPath.corners.Length;
                line.SetPositions(navMeshPath.corners);
            } else {
                line.positionCount=0;
            }
        }
        lastposition = player.position;
    }



    void OnEnable() => m_TrackedImageManager.trackablesChanged.AddListener(OnChanged);

    void OnDisable() => m_TrackedImageManager.trackablesChanged.RemoveListener(OnChanged);

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            navigationBase = GameObject.Instantiate(trackedImagePrefab);

            navigationTargets.Clear();
            navigationTargets = navigationBase.transform.GetComponentsInChildren<NavigationTarget>().ToList();
            navMeshSurface = navigationBase.transform.GetComponentInChildren<NavMeshSurface>();
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            navigationBase.transform.SetPositionAndRotation(updatedImage.pose.position, Quaternion.Euler(0, updatedImage.pose.rotation.eulerAngles.y, 0));

        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
        }

    }
}
