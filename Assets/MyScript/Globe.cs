using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Globe : MonoBehaviour {

    public static bool RedPointState = true;//红点状态

    public static bool UI_GreenPointState = true;//绿点状态

    public static List<string> EngineData = new List<string>();//发动机数据

    public static int NowDisassembleId = 1;//当前拆机部件编号

    public static bool DisassembleState;
}
