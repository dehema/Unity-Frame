﻿using UnityEngine;

namespace Rain.Core
{
    public class LogViewBase : MonoBehaviour
    {
        public virtual void InitializeView()
        {
        }

        public virtual void CloseView()
        {
        }

        public virtual void SetOrientation(ScreenOrientation orientataion)
        {
        }

        public virtual void UpdateResolution()
        {
        }
    }
}