using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FauxGravitySun : MonoBehaviour {

    public float gravity = -5.0f;



	public void Orbit(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 gravityDown = (transform.position - body.position).normalized;

        Vector3 bodyUp = body.up;
        //Vector3 bodyDown = -body.up;

        //body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);
        body.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);

        //Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        //Quaternion targetRotation = Quaternion.FromToRotation(bodyDown, gravityDown) * body.rotation;
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityDown) * body.rotation;


        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, 5 * Time.deltaTime);
    }
}
