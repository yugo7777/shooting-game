using UnityEngine;
using System.Collections;

public class LookAtMouse : MonoBehaviour{

    public float speed;

    void FixedUpdate () {

        Plane playerPlane = new Plane(Vector3.up, transform.position);
   
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

        float hitdist = 0.0f;

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast (ray, out hitdist)){
            
            // Get the point along the ray that hits the calculated distance.
            Vector3 targetPoint = ray.GetPoint(hitdist);
            transform.LookAt(targetPoint);
        }
    }
}