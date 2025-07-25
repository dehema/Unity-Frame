﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Rain.Core
{
    public class ResizeLayoutHandler : UIBehaviour
    {
        public ResizeLayoutHandlerEvent onChange = new ResizeLayoutHandlerEvent();

        protected override void OnRectTransformDimensionsChange()
        {
            onChange.Invoke((RectTransform)transform);
        }

        [System.Serializable]
        public class ResizeLayoutHandlerEvent : UnityEvent<RectTransform>
        {
            public ResizeLayoutHandlerEvent()
            {
            }
        }
    }
}