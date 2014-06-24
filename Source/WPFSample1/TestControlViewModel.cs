using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace WPFSample1
{
  public class TestControlViewModel : PropertyChangedBase
  {
    public TestControlViewModel()
    {
      Products = Model.Product.GetProductList();
    }

    public BindableCollection<Model.Product> Products { get; set; }

    private string text;

    public string Text
    {
      get { return text; }
      set
      {
        if (value != text)
        {
          text = value;
          NotifyOfPropertyChange(() => Text);
        }
      }
    }

    public void Something()
    {
      Text = "Something";
    }

    private double progress;

    public double Progress
    {
      get { return progress; }
      set
      {
        if (value != progress)
        {
          progress = value;
          NotifyOfPropertyChange(() => Progress);
        }
      }
    }

    private decimal spin;

    public decimal Spin
    {
      get { return spin; }
      set
      {
        if (value != spin)
        {
          spin = value;
          NotifyOfPropertyChange(() => Spin);
        }
      }
    }

  }
}