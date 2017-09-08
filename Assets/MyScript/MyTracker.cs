using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pose = HTC.UnityPlugin.PoseTracker.Pose;

public class MyTracker : MonoBehaviour {

    //public Transform controlObj;

    public Collider colliderObj
    {
        get { return m_colliderObj; }
        set { m_colliderObj = value; }
    }
    [SerializeField]
    private Collider m_colliderObj;
    public Vector3 offsetPos;

	// Use this for initialization
	void Start () {
        //controlObj = GameObject.Find("Machine").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider obj)
    {
        if (colliderObj == null)
            colliderObj = obj;
        else
        {
            if (colliderObj.gameObject.tag == "Tool" && obj.gameObject.tag == "Single")
            {
                var scriptSingle = obj.gameObject.GetComponent<MotionSingle>();
                var scriptTool = colliderObj.gameObject.GetComponent<ToolMotionSingle>();
                if(scriptSingle&&scriptTool)
                {
                    scriptTool.Work(scriptSingle);
                }
                colliderObj = null;
                print("1");
            }
        }
    }

    void OnTriggerStay(Collider obj)
    {
    }

    void OnTriggerExit(Collider obj)
    {
        if (obj == colliderObj)
            colliderObj = null;
        print("2");

    }



    void OnCollisionEnter(Collision ctl)
    {

    }
}
