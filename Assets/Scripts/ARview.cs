using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
// Set up touch input propagation while using Instant Preview in the editor.
using Input = GoogleARCore.InstantPreviewInput;
#endif

public class ARview : Manipulator
{
    [Serializable]
    public struct prefabNAme
    {
        public string name;
        public GameObject prefab;
    }
    public prefabNAme[] prefabs;
    public Camera FirstPersonCamera;
    public GameObject GameObjectPrefab;
    private const float k_PrefabRotation = 180.0f;
    public GameObject UiGameobject;
    UIfunction uIfunction;
    public int numberOfObjectsAllowed = 1;
    private int currentNumberOfObjects = 0;
    private GameObject ARprefab;
    public GameObject ManipulatorPrefab;
    private bool m_IsQuitting = false;
    // Start is called before the first frame update
    void Start()
    {
        uIfunction = UiGameobject.GetComponent<UIfunction>();
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.TargetObject == null)
        {
            return true;
        }

        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.WasCancelled)
        {
            return;
        }

        // If gesture is targeting an existing object we are done.
        if (gesture.TargetObject != null)
        {
            return;
        }

        // Raycast against the location the player touched to search for planes.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(
                gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
        {
            // Use hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                foreach(var item in prefabs)
                {
                    if(item.name == uIfunction.secondFunction())
                    {
                        // Choose the prefab based on the Trackable that got hit.
                        GameObjectPrefab = item.prefab;
                    }
                }

                if(currentNumberOfObjects < numberOfObjectsAllowed)
                {
                    currentNumberOfObjects = currentNumberOfObjects + 1;
                    // Instantiate prefab at the hit pose.
                    // Instantiate manipulator.
                    var manipulator =
                        Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);
                    ARprefab = Instantiate(GameObjectPrefab, hit.Pose.position, hit.Pose.rotation);
                    ARprefab.transform.parent = manipulator.transform;
                    if ((hit.Flags & TrackableHitFlags.PlaneWithinPolygon) != TrackableHitFlags.None)
                    {
                        // Get the camera position and match the y-component with the hit position.
                        Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                        cameraPositionSameY.y = hit.Pose.position.y;
                        // Have Andy look toward the camera respecting his "up" perspective, which may be from ceiling.
                        ARprefab.transform.LookAt(cameraPositionSameY, ARprefab.transform.up);
                    }

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                    // Make game object a child of the anchor.
                    manipulator.transform.parent = anchor.transform;
                    // Select the placed object.
                    manipulator.GetComponent<Manipulator>().Select();
                }
            }
        }
    }


    public void destroy()
    {
        if(ARprefab != null)
        {
            Destroy(ARprefab);
        }
        currentNumberOfObjects = currentNumberOfObjects - 1;
        Screen.orientation = ScreenOrientation.Portrait ;
        Debug.Log("itsnjewfejn monkey");
    }
}
