using System.ComponentModel.Composition;
using Caliburn.Micro;
using Caliburn.Micro.DevExpress;

namespace WPFSample
{
  [Export(typeof(IShell))]
  public class MainViewModel : DevExpressConductorOneActive<IScreen>, IShell
  {
    private int count = 1;

    public void New()
    {
      ActivateItem(new DocumentViewModel() { DisplayName = "Document " + count++, CloseAction = this.CloseAction });
    }
  }
}