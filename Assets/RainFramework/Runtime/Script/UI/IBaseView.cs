using System;

namespace Rain.UI
{
    public interface IBaseView
    {
        public void OnOpen(object[] _params = null);
        public void OnClose(Action action);
    }
}
