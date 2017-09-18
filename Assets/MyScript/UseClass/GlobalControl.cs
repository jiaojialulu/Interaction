using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using Pose = HTC.UnityPlugin.PoseTracker.Pose;
using WristBand;
using UnityEngine.Events;
using HTC.UnityPlugin.Utility;
using System.Xml;
using UnityEngine.UI;
public class GlobalControl : MonoBehaviour {

    public AnimationClip toolboxClip;
    public AnimationClip disassembleClip;
    public IndexedSet<Collider> stayingColliders = new IndexedSet<Collider>();
    public RawImage rawImageSingle;
    public Text textSingleName;
    public Text textSingleInfo;

    private Transform Engine;
    private Vector3 oriTransOfTracker;
    private bool bIsMove = false;
    private bool bIsDisassembled = false;
    private Pose trackerPose;
    private Animation ani;
    /// <summary>
    /// 加载XML数据
    /// </summary>
    /// <param name="_xmllodepath"></param>
    /// <returns></returns>
    public static XmlDocument LoadXML(string _xmllodepath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(_xmllodepath);
        return doc;
    }
    /// <summary>
    /// 解析XML数据
    /// </summary>
    public void ReadXml()
    {
        XmlDocument xmlDoc = LoadXML(Application.dataPath + "/Resources/Data/Data.xml");
        XmlNode xmlNodes = xmlDoc.SelectSingleNode("Nodes");
        foreach (XmlNode node in xmlNodes)
        {
            XmlElement mNode = (XmlElement)node;
            WristBandInput.dicNodeInfo.Add(mNode.GetAttribute("id"), new NodeInfo(
                mNode.GetAttribute("name"),
                mNode.GetAttribute("pos"),
                mNode.GetAttribute("rot"),
                mNode.GetAttribute("scale"),
                Resources.Load(mNode.GetAttribute("pic")) as Texture2D,
                mNode.GetAttribute("info")
                ));
        }
    }

    private void Awake()
    {
        // jiao-temporary 
        //ViveInput.AddPressDown(HandRole.RightHand, ControllerButton.Trigger, OnMoveBegin);
        //ViveInput.AddPressUp(HandRole.RightHand, ControllerButton.Trigger, OnMoveEnd);
        Engine = GameObject.Find("Engine").GetComponent<Transform>();
        ani = GetComponent<Animation>();
    }

    private void Ondestory()
    {
        // jiao-temporary
        ViveInput.RemovePress(HandRole.RightHand, ControllerButton.Trigger, OnMoveEnd);
        WristBandInput.disAssembleRemoveListener(new UnityAction(AllDisassemble));

        stayingColliders.Clear();
    }
	// Use this for initialization
	void Start () {
        WristBandInput.disAssembleAddListener(new UnityAction(AllDisassemble));
        ReadXml(); 
	}
	
	// Update is called once per frame
	void Update () {
        trackerPose = VivePose.GetPose(HandRole.RightHand);

        // Engine move
        if(WristBandInput.bBeginMove)
        {
            OnMoveBegin();
        }
        if(WristBandInput.bEndMove)
        {
            OnMoveEnd();
        }
        if (WristBandInput.bIsMoving && stayingColliders.Count == 0)
        {
            Engine.position += trackerPose.pos - oriTransOfTracker;
            oriTransOfTracker = trackerPose.pos;
        }

        // Call ToolBox
        if(WristBandInput.bToolBox)
        {
            ani.clip = toolboxClip;
            ani.Play();
        }

        // DisAssemble
        if(WristBandInput.bDisAssemble)
        {
            ani.clip = disassembleClip;
            ani.Play();
        }

        
	}

    private void OnMoveBegin()
    {
        if (stayingColliders.Count == 0)
        {
            bIsMove = true;
            oriTransOfTracker = trackerPose.pos;
            //print("MoveBegin!");
        }
    }

    private void OnMoveEnd()
    {
        if (stayingColliders.Count == 0)
        {
            bIsMove = false;
            //print("MoveEnd!");
        }
    }

    private void OnTriggerEnter(Collider obj)
    {
        stayingColliders.AddUnique(obj);
    }

    private void OnTriggerExit(Collider obj)
    {
        stayingColliders.Remove(obj);
    }

    private void OnTriggerStay(Collider obj)
    {
        stayingColliders.AddUnique(obj);
        if(obj.tag == "Single")
        {
            ShowInfo(WristBandInput.dicNodeInfo[obj.name]);
        }
    }

    public void AllDisassemble()
    {
        if(!bIsDisassembled)
        {
            StartCoroutine(waitAllDisassemble(0.5F));
            bIsDisassembled = !bIsDisassembled;
        }else
        {
            StartCoroutine(waitAllUnDisassemble(0.5F));
            bIsDisassembled = !bIsDisassembled;
        }
    }
    IEnumerator waitAllUnDisassemble(float time)
    {
        MySingleGrabble[] listMySingleGrabble = Engine.gameObject.GetComponentsInChildren<MySingleGrabble>();
        for (int i = 0; i < listMySingleGrabble.Length; i++)
        {
            listMySingleGrabble[i].reset();
            yield return new WaitForSeconds(time);
        }
    }

    IEnumerator waitAllDisassemble(float time)
    {
        MySingleGrabble[] listMySingleGrabble = Engine.gameObject.GetComponentsInChildren<MySingleGrabble>();
        for (int i = 0; i < listMySingleGrabble.Length; i++)
        {
            listMySingleGrabble[i].move();
            yield return new WaitForSeconds(time);
        }
    }

    public void ShowInfo(NodeInfo nodeInfo)
    {
        rawImageSingle.texture = nodeInfo.PartIcon;
        textSingleInfo.text = nodeInfo.PartInfo;
        textSingleName.text = nodeInfo.PartName;
    }
}
