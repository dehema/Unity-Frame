using System;

namespace Rain.UI
{
    public interface IBaseView
    {
        public void OnOpen(IViewParam viewParams = null);
        public void OnClose(Action action);
    }
}
