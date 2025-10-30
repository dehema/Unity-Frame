using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rain.Core
{
    public static class UGUIExtension
    {
        #region Button
        public static Button AddButtonClickListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddButtonDownListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerDown);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddButtonUpListener(this Button @this, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == EventTriggerType.PointerUp);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }
        public static Button AddListener(this Button @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            if (entry == null)
            {
                entry = new EventTrigger.Entry { eventID = triggerType };
                eventTrigger.triggers.Add(entry);
            }
            entry.callback.AddListener(handle);
            return @this;
        }

        // 设置按钮点击事件
        public static void SetButton(this Button _button, Action _action)
        {
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() =>
            {
                //AudioMgr.Ins.PlaySound(_music);
                _action();
            });
        }

        public static void SetDebugButton(this Button _button, Action _action)
        {
            if (!Application.isEditor)
            {
                _button.gameObject.SetActive(false);
                return;
            }
            _button.image.color = new Color(0.03f, 0.94f, 1);
            SetButton(_button, _action);
        }

        public static void SetToggle(this Toggle _toggle, Action<bool> _action = null)
        {
            _toggle.onValueChanged.AddListener((_ison) =>
            {
                //AudioMgr.Ins.PlaySound(_music);
                _action?.Invoke(_ison);
            });
        }

        public static void SetDebugToggle(this Toggle _toggle, Action<bool> _action = null)
        {
            if (!Application.isEditor)
            {
                _toggle.gameObject.SetActive(false);
                return;
            }
            _toggle.image.color = new Color(0.03f, 0.94f, 1);
            SetToggle(_toggle, _action);
        }

        public static Button RemoveListener(this Button @this, EventTriggerType triggerType, UnityAction<BaseEventData> handle)
        {
            var eventTrigger = @this.gameObject.GetOrAddComponent<EventTrigger>();
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveListener(handle);
            return @this;
        }

        public static Button RemoveAllListeners(this Button @this, EventTriggerType triggerType)
        {
            var eventTrigger = @this.GetComponent<EventTrigger>();
            if (eventTrigger == null)
                throw new ArgumentNullException(nameof(eventTrigger));
            EventTrigger.Entry entry = eventTrigger.triggers.Find(e => e.eventID == triggerType);
            entry?.callback.RemoveAllListeners();
            return @this;
        }
        #endregion

        #region Image
        public static void EnableImage(this Image @this)
        {
            if (@this != null)
            {
                var c = @this.color;
                @this.color = new Color(c.r, c.g, c.b, 1);
            }
        }
        public static void DisableImage(this Image @this)
        {
            if (@this != null)
            {
                var c = @this.color;
                @this.sprite = null;
                @this.color = new Color(c.r, c.g, c.b, 0);
            }
        }
        #endregion
    }
}
