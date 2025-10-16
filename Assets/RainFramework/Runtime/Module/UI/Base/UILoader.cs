
using Rain.Core;

namespace Rain.UI
{
    public class  UILoader : BaseLoader
    {
        private bool isLoadSuccess = false;
        public override bool LoaderSuccess => isLoadSuccess;
        public string Guid;

        public void UILoadSuccess()
        {
            isLoadSuccess = true;
            base.OnComplete();
        }
    }
}
