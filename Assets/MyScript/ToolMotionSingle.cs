using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pose = HTC.UnityPlugin.PoseTracker.Pose;

public class ToolMotionSingle : MotionSingle
{
    private Vector3 oriPosition;
    private Transform oriParentTransform;
    private Quaternion oriRotation;
    public Transform fafd;

	// Use this for initialization
	void Start () {
        tracker = GameObject.Find("RightHand").GetComponent<Transform>();
        oriPosition = transform.localPosition;
        oriParentTransform = GetComponentInParent<Transform>();
        oriRotation = transform.localRotation;
	}
    public override void ShowInfo()
    {
    }

    public override void Reset()
    {
        this.transform.localPosition = oriPosition;
        this.transform.localRotation = oriRotation;
    }

    IEnumerator PlayOrStopAnimation(float time)
    {
        var ani = GetComponentInChildren<Animator>();
        if(ani == null)
        {
            Debug.LogError("Can't Find Animator!");
            yield return new WaitForSeconds(0);
        }
        else
        {
            var temp = ani.GetBool("bBegin");
            ani.applyRootMotion = temp;
            ani.SetBool("bBegin", !temp);
        }
        yield return new WaitForSeconds(time);
        this.Reset();
        if (ani == null)
        {
            Debug.LogError("Can't Find Animator!");
            yield return new WaitForSeconds(0);
        }
        else
        {
            var temp = ani.GetBool("bBegin");
            ani.applyRootMotion = temp;
            ani.SetBool("bBegin", !temp);
        }
    }

    public void Work(MotionSingle single)
    {

        StartCoroutine(PlayOrStopAnimation(2.0f));
        single.Reset();
        //StartCoroutine(PlayOrStopAnimation(0));
        bIsSelected = false;
        bIsDrag = false;

    }
    void OnTriggerEnter(Collider obj)
    {
        if (obj.name == "SphereCollider")
            bIsSelected = true;
        //else
        //{
        //    var script = obj.gameObject.GetComponent<MotionSingle>();
        //    if(script)
        //    {
        //        Work(script); 
        //    }
        //}
    }

    void OnTriggerExit(Collider obj)
    {
        if (bIsSelected && obj.name == "SphereCollider")
            bIsSelected = false;
    }

    public override void BeginDrag()
    {
        base.BeginDrag();
    }

    public override void EndDrag()
    {
        base.EndDrag();
    }


	// Update is called once per frame
	void Update () {
        base.Update();

        if (Input.GetKeyDown(KeyCode.A))
            Reset();
	}
}
