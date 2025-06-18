using UnityEngine.EventSystems;

namespace Rain.Core
{
    public class TabButton : Tab, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

    }
}
