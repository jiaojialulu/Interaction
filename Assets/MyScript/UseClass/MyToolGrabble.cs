using Holoville.HOTween;
using HTC.UnityPlugin.ColliderEvent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WristBand
{
    public class MyToolGrabble : MyGrabble
        , IWithAnimation
    {
        private Vector3 oriPosition;
        private Quaternion oriRotation;
        private Vector3 oriScale;
        // Use this for initialization
        void Start()
        {
            oriPosition = transform.localPosition;
            oriRotation = transform.localRotation;
            oriScale = transform.localScale;        
        }


        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.K))
            //    GetComponentInChildren<Animation>().Play();
        }

        public Vector3 movedPos
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public void move()
        {
            throw new System.NotImplementedException();
        }

        public void reset()
        {
            TweenParms parms = new TweenParms();
            //移动
            parms.Prop("localPosition", oriPosition);
            parms.Prop("localRotation", oriRotation);
            parms.Prop("localScale", oriScale);
            //运动的类型
            parms.Ease(EaseType.EaseInOutCirc);
            //延迟一秒
            parms.Delay(0.1f);
            //执行动画
            HOTween.To(this.transform, 0.5f, parms);
        }

        void OnTriggerEnter(Collider obj)
        {
            //base.DoDrop();
            if (obj.gameObject.name == "ToolBoxFrame")
                reset();
            
        }
    }
}