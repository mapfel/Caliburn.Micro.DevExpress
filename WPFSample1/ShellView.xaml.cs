using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFSample1
{
  public partial class ShellView
  {
    protected virtual void bNew_ItemClick(object sender, EventArgs e)
    {
      ((IShell)DataContext).New();
    }
  }
}