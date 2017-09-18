using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Vive;
using HTC.UnityPlugin.ColliderEvent;

namespace WristBand
{
    public class MyTrackerColliderButtonEventData : ColliderButtonEventData
    {
        public MyTrackerColliderEventCaster viveEventCaster { get; private set; }
        public ControllerButton viveButton { get; private set; }

        public ViveRoleProperty viveRole { get { return viveEventCaster.viveRole; } }

        public MyTrackerColliderButtonEventData(MyTrackerColliderEventCaster eventCaster, ControllerButton viveButton, InputButton button)
            : base(eventCaster, button)
        {
            this.viveEventCaster = eventCaster;
            this.viveButton = viveButton;
        }

        // jiao--要重新写
        public override bool GetPress() { WristBandInput.Initialize(); return WristBandInput.bIsDraging; }

        public override bool GetPressDown() { WristBandInput.Initialize(); return WristBandInput.bBeginDrag; }

        public override bool GetPressUp() { WristBandInput.Initialize(); return WristBandInput.bEndDrag; }
    }
}