// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CameraFocusZone : MonoBehaviour
// {
//     public Transform target;

//     private static TargettingCamera cam;

//     public float cameraSize;

//     private float prevCameraSize;

//     public void Start() {
//         if  (cam == null) {
//             cam = GameObject.FindObjectOfType<TargettingCamera>();
//         }
//     }

//     public void OnTriggerEnter2D(Collider2D col) {
//         if (col.gameObject.CompareTag("Player")) {
//             cam.SetTarget(target);
//             prevCameraSize = cam.GetZoom();
//             cam.SetZoom(cameraSize);
//         }
//     }

//     public void OnTriggerExit2D(Collider2D col) {
//         if (col.gameObject.CompareTag("Player")) {
//             // In case 2 zones overlap
//             if (cam.target == target) {
//                 cam.ClearTarget();
//                 cam.SetZoom(prevCameraSize);
//             }
//         }
//     }
// }
