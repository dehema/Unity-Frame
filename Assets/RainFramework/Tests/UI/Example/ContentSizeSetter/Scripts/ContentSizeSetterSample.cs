﻿using UnityEngine;
using UnityEngine.UI;

namespace Rain.Tests
{
    public class ContentSizeSetterSample : MonoBehaviour
    {
        public Entity entry;

        public InputField input;
        public RectTransform root;

        public void AddEntity(RectTransform parent)
        {
            if (parent == null)
            {
                parent = root;
            }

            Entity add = GameObject.Instantiate<Entity>(entry, parent);
            add.Init(input.text, this);
        }
    }
}