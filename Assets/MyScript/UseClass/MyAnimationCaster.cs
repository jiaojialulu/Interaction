using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTC.UnityPlugin.Utility;
using UnityEngine.EventSystems;
using HTC.UnityPlugin.ColliderEvent;


namespace WristBand
{
    [RequireComponent(typeof(Rigidbody))]
    public class MyAnimationCaster : MonoBehaviour
    {
        private static HashSet<int> s_gos = new HashSet<int>();

        private bool isDisabled;
        private bool isUpdating;
        private JiaoEventData eventData;
        private IndexedSet<Collider> stayingColliders = new IndexedSet<Collider>();

        protected readonly List<JiaoEventData> buttonEventDataList = new List<JiaoEventData>();

        protected class jiaoHandlers
        {
            public List<GameObject> jiaoColliderHandlers = new List<GameObject>();
        }

        private List<jiaoHandlers> buttonEventHandlerList = new List<jiaoHandlers>();


        protected virtual void OnEnable()
        {
            isDisabled = false;
        }
            // Use this for initialization
        void Start()
        {
            eventData = new JiaoEventData(this);
            buttonEventDataList.Add(new JiaoEventData(this));

        }

        protected virtual void OnTriggerStay(Collider other)
        {
            // 相当于set，只有唯一的成员，是用字典实现的
            stayingColliders.AddUnique(other);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            isUpdating = true;

            // process button events
            for (int i = 0, imax = buttonEventDataList.Count; i < imax; ++i)
            {
                // jiao-需要一个eventData的list和一个handlers的list
                var eventData = buttonEventDataList[i];
                var handlers = GetButtonHandlers(i);

                // process button press
                if (!eventData.isMoved)
                {
                    if (eventData.GetWork())
                    {
                        GetComponentInChildren<Animation>().Play();
                        GetComponentInChildren<AudioSource>().Play();
                        ProcessPressDown(eventData, handlers);
                        //ProcessPressing(eventData, handlers);
                    }
                }
                else
                {
                    if (eventData.GetWork())
                    {
                        //ProcessPressing(eventData, handlers);
                    }
                    else
                    {
                        ProcessPressUp(eventData, handlers);
                    }
                }

            }
       
            ExecuteAllEvents();

            if (isDisabled)
            {
                CleanUp();
            }

            isUpdating = false;
        }

        protected void ProcessPressDown(JiaoEventData eventData, jiaoHandlers handlers)
        {
            eventData.isMoved = true;
            eventData.pressPosition = transform.position;
            eventData.pressRotation = transform.rotation;

            // drag start
            GetEventHandlersFromHoveredColliders<IColliderEventJiaoHandler>(eventData.callingHandlers, handlers.jiaoColliderHandlers);
        }


        //protected void ProcessPressing(JiaoEventData eventData, jiaoHandlers handlers)
        //{
        //    // dragging
        //    for (int i = eventData.callingHandlers.Count - 1; i >= 0; --i)
        //    {
        //        handlers.jiaoColliderHandlers.Add(eventData.callingHandlers[i]);
        //    }
        //}

        protected void ProcessPressUp(JiaoEventData eventData, jiaoHandlers handlers)
        {
            eventData.isMoved = false;

            eventData.callingHandlers.Clear();
        }
        protected virtual void OnDisable()
        {
            isDisabled = true;

            if (!isUpdating)
            {
                CleanUp();
            }
        }

        protected virtual void FixedUpdate()
        {
            isUpdating = true;

            //ExecuteAllEvents();

            if (isDisabled)
            {
                CleanUp();
            }

            stayingColliders.Clear();

            isUpdating = false;
        }


        private void CleanUp()
        {
            // release all
            for (int i = 0, imax = buttonEventDataList.Count; i < imax; ++i)
            {
                var eventData = buttonEventDataList[i];
                var handlers = GetButtonHandlers(i);

                if (eventData.isMoved)
                {
                    ProcessPressUp(eventData, handlers);
                }

                eventData.callingHandlers.Clear();
            }


            stayingColliders.Clear();

            ExecuteAllEvents();
        }

        private jiaoHandlers GetButtonHandlers(int i)
        {
            // jiao-相当于初始化？先添加为null，输出时new
            while (i >= buttonEventHandlerList.Count) { buttonEventHandlerList.Add(null); }
            return buttonEventHandlerList[i] ?? (buttonEventHandlerList[i] = new jiaoHandlers());
        }

        private void GetEventHandlersFromHoveredColliders<T>(params IList<GameObject>[] appendHandlers) where T : IEventSystemHandler
        {
            for (int i = stayingColliders.Count - 1; i >= 0; --i)
            {
                var collider = stayingColliders[i];

                if (collider == null) { continue; }

                var handler = ExecuteEvents.GetEventHandler<T>(collider.gameObject);

                if (ReferenceEquals(handler, null)) { continue; }

                if (!s_gos.Add(handler.GetInstanceID())) { continue; }

                foreach (var handlers in appendHandlers)
                {
                    handlers.Add(handler);
                }
            }

            s_gos.Clear();
        }

        private void ExecuteAllEvents()
        {
            for (int i = buttonEventHandlerList.Count - 1; i >= 0; --i)
            {
                if (buttonEventHandlerList[i] == null) { continue; }

                ExcuteHandlersEvents(buttonEventHandlerList[i].jiaoColliderHandlers, buttonEventDataList[i], ExecuteColliderEvents.JiaoHandler);
            }

        }

        private void ExcuteHandlersEvents<T>(List<GameObject> handlers, BaseEventData eventData, ExecuteEvents.EventFunction<T> functor) where T : IEventSystemHandler
        {
            for (int i = handlers.Count - 1; i >= 0; --i)
            {
                ExecuteEvents.Execute(handlers[i], eventData, functor);
            }

            handlers.Clear();
        }

    }

}
