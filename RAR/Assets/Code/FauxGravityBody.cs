using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine.UI;
using UnityEngine;


public class FauxGravityBody : MonoBehaviour {

    public FauxGravitySun sun;
    private Transform myTransform;

    private Rigidbody rgdB;

    private Vector3 normal;

    //SLALOM TEST
    private bool slalomAvailable = true;
    private float slalomCoolDown = 0.5f;

    public float speed = 2;

    public float radius = 10.0f;

    Vector3 velocityV;
    Vector3 pene;
    public float maxSpeed = 15.0f;
    [SerializeField] private float currentSpeed = 0.0f;

    //FP Camera
    private GameObject mainCameraGO;
    public Transform headPosition;
    public float smooth = 0.1f;

    private GameObject targetBone;
    private RaycastHit hitInfo;

    private Quaternion originalHeadRotation;
    private bool lookFront = false;
    private float timeToReturnOP = 0.5f;

    //SHOOTING TEST
    public GameObject arrowReference;
    public enum ShotType
    {
        Rectilineo,
        PhCenterSphere,
        PhRect
    }
    public ShotType currentShotType;
    private GameObject flecha;
    public float shotForce = 15.0f;
    //LAPSE TEST
    public Image targetIcon;
    private Tween iconTween;
    private Tween rightHandTween;
    private Tween returningRightHandTween;
    private Tween leftHandTween;
    private Tween returningLeftHandTween;
    private Tween Charging;

    private FullBodyBipedIK bipedIk;
    private IKEffector rightHand;
    private IKEffector leftHand;
    private Vector3 initialHandTargetPosition;
    //							DOTween.To(()=> myVector, x=> myVector = x, new Vector3(3,4,8), 1);

    // Use this for initialization
    void Start () {

        rgdB = this.GetComponent<Rigidbody>();
        rgdB.constraints = RigidbodyConstraints.FreezeRotation;
        rgdB.useGravity = false;
        myTransform = transform;
        mainCameraGO = transform.Find("Camera").gameObject;
        //targetBone = head.transform.GetChild(0).gameObject;
        targetBone = GameObject.Find("Target Aim").gameObject;

        originalHeadRotation = mainCameraGO.transform.localRotation;

        arrowReference.SetActive(false);

        bipedIk = transform.GetChild(0).GetComponent<FullBodyBipedIK>();
        rightHand = bipedIk.solver.rightHandEffector;
        leftHand = bipedIk.solver.leftHandEffector;
        initialHandTargetPosition = rightHand.target.localPosition;
        //TEST

        velocityV = new Vector3();
        pene = new Vector3();
        pene = this.transform.position;

        //ARROW
        flecha = Instantiate<GameObject>(arrowReference);
        //
        flecha.transform.position = arrowReference.transform.position;
        flecha.transform.rotation = arrowReference.transform.rotation;
        
        //
        flecha.SetActive(true);
        flecha.transform.localScale = arrowReference.transform.lossyScale;
        flecha.transform.SetParent(arrowReference.transform.parent);


        //ShotTimeLapse

        returningLeftHandTween = DOTween.To(() => leftHand.positionWeight, x => leftHand.positionWeight = x, 0.0f, 0.5f);
        returningRightHandTween = DOTween.To(() => rightHand.positionWeight, x => rightHand.positionWeight = x, 0.0f, 0.5f);
    }
	
	// Update is called once per frame
	void Update () {

        mainCameraGO.transform.position = Vector3.Lerp(mainCameraGO.transform.position, headPosition.position, smooth);

        //Debug.Log();
        //velocity Vector
        Debug.DrawLine(this.transform.position, this.transform.position + rgdB.velocity, Color.green);
        

        velocityV = (this.transform.position - (this.transform.position + rgdB.velocity));

        currentSpeed = Vector3.Magnitude(rgdB.velocity);  // test current object speed

        normal = -this.transform.up * currentSpeed;
        /////////////////////////////////////////////////
        if (Input.GetKey(KeyCode.W))
        {

            

            
            if (slalomAvailable)
            {
                float pushingForce = speed;
                //rgdB.AddForce(transform.TransformDirection(moveDir * movSpeed), ForceMode.Force); 
                rgdB.AddForce(this.transform.forward * pushingForce, ForceMode.Impulse);

                StartCoroutine(SlalomCoolDown());
            }

            
            
            if (currentSpeed > maxSpeed)
            {
                Debug.Log("Stopping");

                float brakeSpeed = currentSpeed - maxSpeed;  // calculate the speed decrease

                Vector3 normalisedVelocity = rgdB.velocity.normalized;
                Vector3 brakeVelocity = normalisedVelocity * brakeSpeed;  // make the brake Vector3 value

                rgdB.AddForce(-brakeVelocity);  // apply opposing brake force
            }
        }

        
        if (Input.GetKey(KeyCode.A))
        {
            //this.transform.RotateAroundLocal(this.transform.up, -0.5f);
            this.transform.RotateAroundLocal(this.transform.up, -0.5f * Time.deltaTime);

            
            if (true) //(Vector3.Angle(velocityV, this.transform.forward-this.transform.position) > 4.0f)
            {
                pene = new Vector3(velocityV.y*normal.z-velocityV.z-normal.y, velocityV.z*normal.x- velocityV.x*normal.z, velocityV.y*normal.x-velocityV.x*normal.y);
                this.rgdB.AddForce( pene * Time.deltaTime ,ForceMode.Force);
            }
            
            

            //rgdBody.AddForce(-this.transform.right * 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            //this.transform.RotateAroundLocal(this.transform.up, 0.5f);
            this.transform.RotateAroundLocal(this.transform.up, 0.5f * Time.deltaTime);
            

            if (true)//(Vector3.Angle(rgdB.velocity, this.transform.forward) > 4.0f)
            {
                pene = new Vector3(velocityV.y * normal.z - velocityV.z - normal.y, velocityV.z * normal.x - velocityV.x * normal.z, velocityV.y * normal.x - velocityV.x * normal.y);
                this.rgdB.AddForce(-pene * Time.deltaTime, ForceMode.Force);
            }

            //rgdBody.AddForce(this.transform.right * 1);
        }


        
        Debug.DrawLine(this.transform.position, this.transform.position-pene, Color.white);
        //////////////////////////////////////////////////


        


        if (Input.GetKeyDown(KeyCode.Space))
        {
            //head.transform.localRotation = originalHeadRotation;

            mainCameraGO.transform.DOLocalRotateQuaternion(originalHeadRotation, timeToReturnOP);
            StartCoroutine(doingTheNoob(timeToReturnOP));

        }
        if (Input.GetKey(KeyCode.Space) && lookFront)
        {
            mainCameraGO.transform.localRotation = originalHeadRotation;
            
        }
        else
        {
            lookFront = false;

            //float rightX = Input.GetAxis("Mouse X") * 2 + Input.GetAxis("HorizontalR") * 75.0f * Time.deltaTime;
            //float rightY = Input.GetAxis("Mouse Y") * 2 + Input.GetAxis("VerticalR") * 75.0f * Time.deltaTime;
            
            float rightX = Input.GetAxis ("Mouse X") * 2.0f ;
            float rightY = Input.GetAxis ("Mouse Y") * 2.0f ;

            mainCameraGO.transform.Rotate(-rightY, rightX, 0);
        }

        Physics.Raycast(mainCameraGO.transform.position, mainCameraGO.transform.forward, out hitInfo);
        targetBone.transform.position = hitInfo.point;
        //Debug.Log("VelocityV magnitude; "+velocityV.magnitude);


        if (Input.GetMouseButtonDown(0))
        {

            StartShotLapse();

            //flecha.transform.DOBlendableLocalMoveBy(flecha.transform.TransformDirection(sun.transform.position)* Time.deltaTime, 1.0f);
            /*
#region
            flecha.transform.parent = null;

            Shot();


            flecha = Instantiate<GameObject>(arrowReference);
            //
            flecha.transform.position = arrowReference.transform.position;
            flecha.transform.rotation = arrowReference.transform.rotation;
            
            //
            flecha.SetActive(true);
            flecha.transform.localScale = arrowReference.transform.lossyScale;
            flecha.transform.SetParent(arrowReference.transform.parent);
#endregion
*/
        }

        if (Input.GetMouseButtonUp(0))
        {
            LeaveShotLapse();
        }
    }

    public void LateUpdate()
    {
        sun.Orbit(myTransform);
    }
    private void FixedUpdate()
    {

        //rgdB.MovePosition(rgdB.transform.position + transform.TransformDirection(moveDir * movSpeed * Time.deltaTime));
        /* if (rgdB.drag > 0.0f)
         {
             if ((velocityV.x < -0.5f) || (velocityV.x > 0.5f))
             {
                 rgdB.drag = rgdB.drag + 0.01f;
             }
             else
             {
                 rgdB.drag = rgdB.drag - 0.02f;
             }
         }*/
        
    }

    public void Shot()
    {
        flecha.GetComponent<Arrow>().ActiveTrailRenderer();

        switch(currentShotType){

            case ShotType.Rectilineo:
                flecha.transform.DOMove(targetBone.transform.position, 1.0f);
                break;
            case ShotType.PhRect:
                flecha.GetComponent<Rigidbody>().isKinematic = false;
                flecha.GetComponent<Rigidbody>().AddForce(flecha.transform.forward * shotForce);
                break;
            case ShotType.PhCenterSphere:
                flecha.GetComponent<Rigidbody>().isKinematic = false;
                break;
        }
    }

    public IEnumerator SlalomCoolDown()
    {
        slalomAvailable = false;
        yield return new WaitForSeconds(slalomCoolDown);
        slalomAvailable = true;

    }
    public IEnumerator doingTheNoob(float pene)
    {
        yield return new WaitForSeconds(pene);
        lookFront = true;
    }


    public void StartShotLapse()
    {
        Vector3 endScale = new Vector3(0.2f, 0.2f, 0.2f);
        iconTween = targetIcon.transform.DOScale(endScale, 1f);
        iconTween.Play<Tween>();

        leftHandTween = DOTween.To(() => leftHand.positionWeight , x => leftHand.positionWeight = x, 0.5f, 0.5f);
        rightHandTween = DOTween.To(() => rightHand.positionWeight, x => rightHand.positionWeight = x, 0.8f, 0.5f);

        leftHandTween.Restart();
        rightHandTween.Restart();
        leftHandTween.OnComplete<Tween>(new TweenCallback(ChargingArrow));
    }

    public void ChargingArrow()
    {
        returningLeftHandTween = DOTween.To(() => leftHand.positionWeight, x => leftHand.positionWeight = x, 0.0f, 0.5f);
        //returningRightHandTween = DOTween.To(() => rightHand.positionWeight, x => rightHand.positionWeight = x, 0.0f, 0.5f);
        returningLeftHandTween.Restart();
        //returningRightHandTween.Restart();

        Charging = rightHand.target.transform.DOLocalMoveX(-0.15f, 0.5f);
        Charging.Restart();

    }

    public void LeaveShotLapse()
    {
        iconTween.Kill();
        leftHandTween.Kill();
        returningLeftHandTween = DOTween.To(() => leftHand.positionWeight, x => leftHand.positionWeight = x, 0.0f, 0.5f);
        returningLeftHandTween.Restart();

        Vector3 endScale = new Vector3(1f, 1f, 1f);
        iconTween = targetIcon.transform.DOScale(endScale, 0.2f);
        iconTween.Play<Tween>();

        returningRightHandTween = DOTween.To(() => rightHand.positionWeight, x => rightHand.positionWeight = x, 0.0f, 0.5f);
        returningRightHandTween.Restart();

        DOTween.To(() => rightHand.target.transform.localPosition, x => rightHand.target.transform.localPosition = x, initialHandTargetPosition, 0.3f).Play<Tween>();

        //rightHand.target.transform.localPosition = initialHandTargetPosition;
    }



}

/*
 * // Tween a Vector3 called myVector to 3,4,8 in 1 second
							DOTween.To(()=> myVector, x=> myVector = x, new Vector3(3,4,8), 1);
							// Tween a float called myFloat to 52 in 1 second
							DOTween.To(()=> myFloat, x=> myFloat = x, 52, 1);
 * 
 * */
