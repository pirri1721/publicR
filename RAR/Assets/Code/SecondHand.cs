using System.Collections;
using System.Collections.Generic;
using RootMotion.FinalIK;
using UnityEngine;

public class SecondHand : MonoBehaviour {

    public AimIK aim;
    public FullBodyBipedIK ik;
    public LookAtIK look;

    private IKEffector leftHand { get { return ik.solver.leftHandEffector; } }
    private IKEffector rightHand { get { return ik.solver.rightHandEffector; } }

    private Quaternion rightHandRotationRelative;

    // Use this for initialization
    void Start () {
        aim.Disable();
        ik.Disable();
        look.Disable();

        ik.solver.OnPostUpdate += OnPostFBBIK;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        /*
         *  //Find out how the left Hand is positioned relativo to the right Hand
        Vector3 toLeftHand = leftHand.bone.position - rightHand.bone.position;
        Vector3 toLeftHandRelative = rightHand.bone.InverseTransformDirection(toLeftHand);

        aim.solver.Update();

        leftHand.position = rightHand.bone.position + rightHand.bone.TransformDirection(toLeftHandRelative);
         * 
         */

        //Find out how the right Hand is positioned relativo to the left Hand
        Vector3 toRightHand = rightHand.bone.position - leftHand.bone.position;
        Vector3 toRightHandRelative = leftHand.bone.InverseTransformDirection(toRightHand);

        aim.solver.IKPosition = look.solver.IKPosition;

        aim.solver.Update();

        rightHand.position = leftHand.bone.position + leftHand.bone.TransformDirection(toRightHandRelative);
        rightHand.positionWeight = 1f;

        //leftHand.position = rightHand.bone.position;
        //leftHand.positionWeight = 1f;
        //ik.solver.GetLimbMapping(FullBodyBipedChain.LeftArm).maintainRotationWeight = 1f;

        ik.solver.Update();


        look.solver.IKPosition = aim.solver.IKPosition;
        look.solver.Update();
	}

    private void OnPostFBBIK()
    {
        rightHand.bone.rotation = leftHand.bone.rotation * rightHandRotationRelative;
    }
}
