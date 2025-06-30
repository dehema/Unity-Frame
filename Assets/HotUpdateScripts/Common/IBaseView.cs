using System;

namespace Rain.UI
{
    public interface IBaseView
    {
        public void OnOpen(IViewParams viewParams = null);
        public void OnClose(Action action);
    }
}
