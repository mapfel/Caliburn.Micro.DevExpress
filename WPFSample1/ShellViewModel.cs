namespace WPFSample1
{
  using System.ComponentModel.Composition;
  using Caliburn.Micro;
  using Caliburn.Micro.DevExpress;

  [Export(typeof(IShell))]
  public class ShellViewModel : DevExpressConductorOneActive<IScreen>, IShell
  {
    private int count = 1;

    public void New()
    {
      ActivateItem(new InnerDocumentViewModel() { DisplayName = "Inner document " + count++, CloseAction = this.CloseAction });
    }
  }
}