using UnityEngine;
using System.Collections;
using System.Xml;
using HTC.UnityPlugin.Vive;

public class MainControl : MonoBehaviour
{
    [SerializeField]
    public Transform EngineNode;    //发动机主节点
    [SerializeField]
    private Transform Engine;       // 发动机
    public Transform tracker;       // 追踪器
    public Transform ToolBox;       // 工具箱

    private bool bIsMove = false;
    private Vector3 oriTransOfEngine;
    private Vector3 oriTransOfTracker;

    public void CallToolBox()
    {
        if (ToolBox.gameObject.activeSelf)
            ToolBox.gameObject.SetActive(false);
        else
            ToolBox.gameObject.SetActive(true);

    }

    public void Start()
    {
        ReadXml();//解析XML数据
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void BeginMove()
    {
        bIsMove = true;
        oriTransOfTracker = tracker.position;
    }

    public void EndMove()
    {
        bIsMove = false;
    }

    public void Update()
    {
        if(bIsMove&&tracker.gameObject.activeSelf)
        {
            Engine.position += tracker.position - oriTransOfTracker;
            oriTransOfTracker = tracker.position;
        }

        // 判断手势
        if(ViveInput.GetPressDown(HandRole.RightHand,ControllerButton.Trigger))
        {
            var control = tracker.GetComponent<MyTracker>().colliderObj;
            if(control != null)
            {
                var motionSingle = control.gameObject.GetComponent<MotionSingle>();
                if(motionSingle)
                    control.gameObject.GetComponent<MotionSingle>().BeginDrag();
            }else
            {
                BeginMove();
            }
        }
        if(ViveInput.GetPressUp(HandRole.RightHand,ControllerButton.Trigger))
        {
            var control = tracker.GetComponent<MyTracker>().colliderObj;
            if (control != null)
            {
                var motionSingle = control.gameObject.GetComponent<MotionSingle>();
                if (motionSingle)
                    control.gameObject.GetComponent<MotionSingle>().EndDrag();
            }
            else
            {
                EndMove();
            }
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            CallToolBox();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            AllDisassemble();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            AllUnDisassemble();
        }
    }

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
            Globe.EngineData.Add(mNode.GetAttribute("id") + "*" + mNode.GetAttribute("name") + "*" +
                                              mNode.GetAttribute("step") + "*" + 
                                              mNode.GetAttribute("sroce") + "*" +
                                              mNode.GetAttribute("pos") + "*" +
                                              mNode.GetAttribute("rot") + "*" +
                                              mNode.GetAttribute("scale") + "*" +
                                              mNode.GetAttribute("pic"));
        }

        for (int i = 0; i < Globe.EngineData.Count; i++)
        {
            //从集合内获取到数据并分割
            string[] str = Globe.EngineData[i].Split('*');
            EngineNode.GetChild(i).gameObject.AddComponent<MotionSingle>();//给每个部件添加脚本
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().PartId = str[0];//部件ID
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().PartName = str[1];//部件名称
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().PartPosition = str[4];//部件移动坐标
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().PartRotation = str[5];//传ScriptData到PartScript.cs
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().PartScale = str[6];//传Label控件到PartScript.cs
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().PartIcon = Resources.Load(str[7]) as Texture2D;
        }
    }

    public void AllDisassemble()
    {
        StartCoroutine(waitAllDisassemble(0.5F));
    }

    public void AllUnDisassemble()
    {
        StartCoroutine(waitAllUnDisassemble(0.5F));
    }

    IEnumerator waitAllUnDisassemble(float time)
    {
        for (int i = Globe.EngineData.Count-1 ;i>=0 ; i--)
        {
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().Reset();
            yield return new WaitForSeconds(time);

        }
    }

    IEnumerator waitAllDisassemble(float time)
    {
        for (int i = 0; i < Globe.EngineData.Count; i++)
        {
            EngineNode.GetChild(i).gameObject.GetComponent<MotionSingle>().Move();
            yield return new WaitForSeconds(time);

        }
    }
}
