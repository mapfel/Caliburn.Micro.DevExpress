using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using Caliburn.Micro.DevExpress;

namespace WPFSample1
{
  public class InnerDocumentViewModel : DXDockingScreen
  {
    public InnerDocumentViewModel()
    {
      Products = Model.Product.GetProductList();
      TestControl = new TestControlViewModel();
    }

    public BindableCollection<Model.Product> Products { get; set; }

    public Model.Product SelectedProduct { get; set; }

    public TestControlViewModel TestControl { get; set; }

    public void CheckState()
    {
    }

    public void Test()
    {
    }
  }
}