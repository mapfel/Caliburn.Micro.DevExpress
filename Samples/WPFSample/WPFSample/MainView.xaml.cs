using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Ribbon;

namespace WPFSample
{
  public partial class MainView : DXRibbonWindow
  {
    public MainView()
    {
      InitializeComponent();
    }

    private void bNew_ItemClick(object sender, ItemClickEventArgs e)
    {
      ((IShell)DataContext).New();
    }
  }
}