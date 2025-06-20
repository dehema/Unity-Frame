using Rain.Core;
using UnityEngine;

namespace Rain.Tests
{
    public class MultiLayoutSample : MonoBehaviour
    {
        private MultiLayout multiLayout = null;
        private int layout = 0;

        private void Awake()
        {
            multiLayout = GetComponent<MultiLayout>();
        }

        public void ChangeLayout()
        {
            multiLayout.SelectLayout(layout++ % multiLayout.layout.count);
        }
    }
}