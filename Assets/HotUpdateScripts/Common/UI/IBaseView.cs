using System;

namespace Rain.UI
{
    public interface IBaseView
    {
        public void OnOpen(IViewParam viewParam = null);
        public void OnClose(Action action);
    }
}
