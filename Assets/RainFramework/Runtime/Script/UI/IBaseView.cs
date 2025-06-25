using System;

namespace Rain.UI
{
    public interface IBaseView
    {
        public void OnOpen(params object[] _params);
        public void OnClose(Action action);
    }
}
