using System.Collections;
using System.Collections.Generic;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace WristBand
{
    public class NodeInfo
    {
        //public string PartId;//部件编号
        public string PartName;//部件名称
        public string PartPosition;//部件需要移动的坐标
        public string PartRotation;//部件需要移动的坐标
        public string PartScale;//部件需要移动的坐标
        public Texture2D PartIcon;
        public string PartInfo;// 部件信息

        public NodeInfo(string partName = null, string partPosition = null, string partRotation = null, string partScale = null, Texture2D partIcon = null, string partInfo = null)
        {
            PartName = partName;
            PartRotation = partRotation;
            PartScale = partScale;
            PartIcon = partIcon;
            PartInfo = partInfo;
        }
    }
    /// <summary>
    /// jiao--单例
    /// </summary>
    public class WristBandInput : SingletonBehaviour<WristBandInput>
    {
        public static Dictionary<string,NodeInfo> dicNodeInfo = new Dictionary<string,NodeInfo>();
        public static bool bBeginDrag = false;
        public static bool bIsDraging = false;
        public static bool bEndDrag = false;

        public static bool bBeginMove = false;
        public static bool bIsMoving = false;
        public static bool bEndMove = false;

        public static bool bWork = false;
        public static bool bGetPressDown = false;
        public static bool bToolBox = false;
        public static bool bDisAssemble = false;
        private static UnityEvent disAssemble = new UnityEvent();
        private static UnityEvent callToolBox = new UnityEvent();
        private void Update()
        {
            // 必须要先初始化initial（）temparary
            if(bBeginDrag = ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
            {
                bIsDraging = true;
            }
            if(bEndDrag = ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger))
            {
                bIsDraging = false;
            }
            if (bBeginMove = Input.GetKeyDown(KeyCode.Space))//ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Trigger))
            {
                bIsMoving = true;
            }
            if (bEndMove = Input.GetKeyUp(KeyCode.Space))//ViveInput.GetPressUp(HandRole.RightHand, ControllerButton.Trigger))
            {
                bIsMoving = false;
            }

            bWork = ViveInput.GetPress(HandRole.RightHand, ControllerButton.Menu);

            if(bDisAssemble = Input.GetKeyDown(KeyCode.D))
            {
                disAssemble.Invoke();
            }
            if (bToolBox = Input.GetKeyDown(KeyCode.T))
            {
                callToolBox.Invoke();
            }
            //print("singleton");
        }

        public static void disAssembleAddListener(UnityAction action)
        {
            disAssemble.AddListener(action);
        }
        public static void disAssembleRemoveListener(UnityAction action)
        {
            disAssemble.RemoveListener(action);
        }

        public static void callToolBoxAddListener(UnityAction action)
        {
            callToolBox.AddListener(action);
        }
        public static void callToolBoxRemoveListener(UnityAction action)
        {
            callToolBox.RemoveListener(action);
        }
    }
}
