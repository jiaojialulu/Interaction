using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.ColliderEvent;

namespace WristBand
{
    [RequireComponent(typeof(AudioSource))]
    public class MySingleGrabble : MyGrabble
        , IWithAnimation
        , IColliderEventJiaoHandler
    {
        public AudioClip moveAudioClip;
        public AudioClip resetAudioClip;
        public Vector3 movedPos
        {
            get { return m_movedPos; }
            set { m_movedPos = value; }
        }
        private Vector3 m_movedPos;
        public void move()
        {
            TweenParms parms = new TweenParms();
            //移动
            parms.Prop("localPosition", movedPos);
            parms.Prop("localRotation", new Vector3(0, 0, 0));
            parms.Prop("localScale", new Vector3(1, 1, 1));
            //运动的类型
            parms.Ease(EaseType.EaseInOutCirc);
            //延迟一秒
            parms.Delay(0.1f);
            //执行动画
            HOTween.To(this.transform, 0.5f, parms);
            GetComponent<AudioSource>().clip = moveAudioClip;
            GetComponent<AudioSource>().Play();
        }

        public void reset()
        {
            TweenParms parms = new TweenParms();
            //移动
            parms.Prop("localPosition", new Vector3(0, 0, 0));
            parms.Prop("localRotation", new Vector3(0, 0, 0));
            parms.Prop("localScale", new Vector3(1, 1, 1));
            //运动的类型
            parms.Ease(EaseType.EaseInOutCirc);
            //延迟一秒
            parms.Delay(0.1f);
            //执行动画
            HOTween.To(this.transform, 0.5f, parms);
            GetComponent<AudioSource>().clip = resetAudioClip;
            GetComponent<AudioSource>().Play();
        }

        void Start()
        {
            base.Start();
            Collider box;
            if (!(box = GetComponent<BoxCollider>()))
                if (!(box = GetComponent<CapsuleCollider>()))
                    if (!(box = GetComponent<MeshCollider>()))
                        if (!(box = GetComponent<SphereCollider>()))
                            if (!(box = GetComponent<WheelCollider>()))



                                if (box == null)
                                {
                                    Debug.LogError("Can't Find Collider!");
                                }
            movedPos = box.bounds.center;

        }

        public virtual void OnColliderEventJiao(WristBand.JiaoEventData eventData)
        {
            reset();
        }
    }
}
