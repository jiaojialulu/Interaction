using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pose = HTC.UnityPlugin.PoseTracker.Pose;

public class MotionSingle : MonoBehaviour {

    public string PartId;//部件编号
    public string PartName;//部件名称
    public string PartPosition;//部件需要移动的坐标
    public string PartRotation;//部件需要移动的坐标
    public string PartScale;//部件需要移动的坐标
    public Texture2D PartIcon;
    public string PartInfo;// 部件信息

    private Vector3 MovedPos;
    private Transform OriginPos;
    private bool bLocked;
    private bool bIsRotate = false;
    public bool bIsDrag = false;
    public bool bIsSelected = false;
    private Transform tracker;
    protected Pose transformation;

    public virtual void ShowInfo()
    {
        GameObject.Find("ComIcon").GetComponent<RawImage>().texture = PartIcon;
        GameObject.Find("ComInfo").GetComponent<Text>().text = PartName;
    }
    public void rotate()
    {
        bIsRotate = !bIsRotate;
        StartCoroutine(rotateSelf());
    }

    public void zoomIn()
    {
        this.transform.localScale = this.transform.localScale * 1.01f;    
    }

    public void zoomOut()
    {
        this.transform.localScale = this.transform.localScale * 0.99f;
    }

    /// <summary>
    /// 自转
    /// </summary>
    private IEnumerator rotateSelf()
    {
        while (bIsRotate)
        {
            this.transform.Rotate(new Vector3(0, 10 * Time.deltaTime, 0), Space.World);
            yield return 0;
        }
    }

    public void Move()
    {
        TweenParms parms = new TweenParms();
        //移动
        parms.Prop("position", MovedPos);
        parms.Prop("rotation", new Vector3(0, 0, 0));
        parms.Prop("localScale", new Vector3(1, 1, 1));
        //运动的类型
        parms.Ease(EaseType.EaseInOutCirc);
        //延迟一秒
        parms.Delay(0.1f);
        //执行动画
        HOTween.To(this.transform, 0.5f, parms);
    }

    public void Reset()
    {
        TweenParms parms = new TweenParms();
        //移动
        parms.Prop("position", new Vector3(0, 0, 0));
        parms.Prop("rotation", new Vector3(0, 0, 0));
        parms.Prop("localScale", new Vector3(1, 1, 1));
        //运动的类型
        parms.Ease(EaseType.EaseInOutCirc);
        //延迟一秒
        parms.Delay(0.1f);
        //执行动画
        HOTween.To(this.transform, 0.5f, parms);
    }

	// Use this for initialization
	void Start () {
        OriginPos = this.transform;
        Collider box;
        if(!(box = GetComponent<BoxCollider>()))
            if (!(box = GetComponent<CapsuleCollider>()))
                if (!(box = GetComponent<MeshCollider>()))
                    if (!(box = GetComponent<SphereCollider>()))
                        if (!(box = GetComponent<WheelCollider>()))



        if(box == null)
        {
            Debug.LogError("Can't Find Collider!");
        }
        MovedPos = box.bounds.center;

        tracker = GameObject.Find("RightHand").GetComponent<Transform>();
	}

    void OnTriggerEnter(Collider obj)
    {
        if (obj.name == "SphereCollider")
            bIsSelected = true;
    }

    void OnTriggerExit(Collider obj)
    {
        if (bIsSelected&&obj.name == "SphereCollider")
            bIsSelected = false;
    }

    public virtual void BeginDrag()
    {
        bIsDrag = true;
        transformation = Pose.FromToPose(new Pose(tracker), new Pose(transform));
    }

    public virtual void EndDrag()
    {
        bIsDrag = false;
    }


	// Update is called once per frame
    void Update()
    {
        if (bIsDrag && tracker.gameObject.activeSelf)
        {
            var targetPose = new Pose(tracker) * transformation;// *offsetPose;
            transform.position = targetPose.pos;
            transform.rotation = targetPose.rot;
        }
        if(bIsSelected)
        {
            ShowInfo();
        }
    }
}
