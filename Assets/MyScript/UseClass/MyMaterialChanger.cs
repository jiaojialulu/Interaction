//========= Copyright 2016-2017, HTC Corporation. All rights reserved. ===========

using HTC.UnityPlugin.ColliderEvent;
using HTC.UnityPlugin.Utility;
using HTC.UnityPlugin.Vive;
using System;
using System.Collections.Generic;
using UnityEngine;

// This component shows the status that interacting with ColliderEventCaster
public class MyMaterialChanger : MonoBehaviour
    , IColliderEventHoverEnterHandler
    , IColliderEventHoverExitHandler
    , IColliderEventPressEnterHandler
    , IColliderEventPressExitHandler
{
    private readonly static List<Renderer> s_rederers = new List<Renderer>();

    [NonSerialized]
    private Material currentMat;

    public Material Normal;
    public Material Heightlight;
    public Material Pressed;
    public Material dragged;

    public ColliderButtonEventData.InputButton heighlightButton = ColliderButtonEventData.InputButton.Trigger;

    private HashSet<ColliderHoverEventData> hovers = new HashSet<ColliderHoverEventData>();
    private HashSet<ColliderButtonEventData> presses = new HashSet<ColliderButtonEventData>();
    private IndexedSet<ColliderButtonEventData> drags = new IndexedSet<ColliderButtonEventData>();

    // jiao
    private Material[] ArrayOfChildrenMaterial;
    private bool bCollidered = false;
    private void Start()
    {
        // jiao--保存原始材质球
        GetComponentsInChildren(true, s_rederers);
        ArrayOfChildrenMaterial = new Material[s_rederers.Count];
        for (int i = s_rederers.Count - 1; i >= 0; --i)
        {
            ArrayOfChildrenMaterial[i] = s_rederers[i].material;
        }

        UpdateMaterialState();
    }

    void Update()
    {
        //if(bCollidered)
        //{
        //    if (ViveInput.GetPressDown(HandRole.RightHand, ControllerButton.Menu))
        //    {
        //        this.GetComponent<MotionSingle>().rotate();
        //    }

        //    if(ViveInput.GetPadPressAxis(HandRole.RightHand).y >0.2)
        //    {
        //        this.GetComponent<MotionSingle>().zoomIn();
        //    }
        //    if (ViveInput.GetPadPressAxis(HandRole.RightHand).y < -0.2)
        //    {
        //        this.GetComponent<MotionSingle>().zoomOut();
        //    }
        //    this.GetComponent<MotionSingle>().ShowInfo();

        //}
    }

    public void OnColliderEventHoverEnter(ColliderHoverEventData eventData)
    {
        hovers.Add(eventData);
        bCollidered = true;
        

        UpdateMaterialState();
    }

    public void OnColliderEventHoverExit(ColliderHoverEventData eventData)
    {
        hovers.Remove(eventData);
        bCollidered = false;
        UpdateMaterialState();
    }

    public void OnColliderEventPressEnter(ColliderButtonEventData eventData)
    {
        if (eventData.button != heighlightButton) { return; }

        presses.Add(eventData);

        // check if this evenData is dragging me(or ancestry of mine)
        for (int i = eventData.draggingHandlers.Count - 1; i >= 0; --i)
        {
            if (transform.IsChildOf(eventData.draggingHandlers[i].transform))
            {
                drags.AddUnique(eventData);
                break;
            }
        }

        UpdateMaterialState();
    }

    public void OnColliderEventPressExit(ColliderButtonEventData eventData)
    {
        presses.Remove(eventData);

        UpdateMaterialState();
    }

    private void LateUpdate()
    {
        UpdateMaterialState();
    }

    private void OnDisable()
    {
        hovers.Clear();
        presses.Clear();
        drags.Clear();
    }

    private void UpdateMaterialState()
    {
        var targetMat = default(Material);

        if (drags.Count > 0)
        {
            drags.RemoveAll(e => !e.isDragging);
        }

        if (drags.Count > 0)
        {
            targetMat = dragged;
        }
        else if (presses.Count > 0)
        {
            targetMat = Pressed;
        }
        else if (hovers.Count > 0)
        {
            targetMat = Heightlight;
        }
        else
        {
            targetMat = Normal;
        }

        if (ChangeProp.Set(ref currentMat, targetMat))
        {
            SetChildRendererMaterial(targetMat);
        }
    }

    private void SetChildRendererMaterial(Material targetMat)
    {
        GetComponentsInChildren(true, s_rederers);

        if (s_rederers.Count > 0)
        {
            if (targetMat == Normal)
            {
                for (int i = s_rederers.Count - 1; i >= 0; --i)
                {
                    s_rederers[i].sharedMaterial = ArrayOfChildrenMaterial[i];
                }
            }
            else
            {
                for (int i = s_rederers.Count - 1; i >= 0; --i)
                {
                    s_rederers[i].sharedMaterial = targetMat;
                }
            }

            s_rederers.Clear();
        }
    }

    void OnTriggerStay(Collider obj)
    {
        if (obj.gameObject.tag =="Tracker")
        {
            var targetMat = default(Material);
            targetMat = Heightlight;
            SetChildRendererMaterial(targetMat);
        }
        

    }

    void OnTriggerExit(Collider obj)
    {
        if (obj.name == "SphereCollider")
        {
            var targetMat = default(Material);
            targetMat = Normal;
            SetChildRendererMaterial(targetMat);
        }
        
    }
}
