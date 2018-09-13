using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public GameObject sun;
    public float atractionForce = 3;

    private Rigidbody rgb;
    private TrailRenderer tR;
	// Use this for initialization
	void Start () {
        rgb = GetComponent<Rigidbody>();
        tR = GetComponent<TrailRenderer>();

        tR.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        if (rgb.velocity.magnitude > 2.0f)
        {
            this.transform.rotation = Quaternion.LookRotation(rgb.velocity);
        }

        rgb.AddForce((sun.transform.position - this.transform.position) * atractionForce * Time.deltaTime);


        Debug.DrawLine(this.transform.position, (sun.transform.position - this.transform.position) * atractionForce, Color.black);
		
	}

    public void ActiveTrailRenderer()
    {
        tR.enabled = true;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.name == "esfera")
        rgb.isKinematic = true;
    }
}
