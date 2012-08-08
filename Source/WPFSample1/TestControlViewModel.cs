using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace WPFSample1
{
  public class TestControlViewModel : PropertyChangedBase
  {
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
  }
}