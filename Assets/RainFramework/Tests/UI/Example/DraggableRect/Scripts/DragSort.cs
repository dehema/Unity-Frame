using UnityEngine;

namespace Rain.Tests
{
    public class DragSort : MonoBehaviour
    {
        public void SortChange()
        {
            transform.SetAsLastSibling();
        }
    }
}