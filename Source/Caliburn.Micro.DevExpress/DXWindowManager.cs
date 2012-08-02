using System.Windows;
using DevExpress.Xpf.Core;

namespace Caliburn.Micro.DevExpress
{
  public class DXWindowManager : WindowManager
  {
    /// <summary>
    /// Makes sure the view is a DX window or is wrapped by one.
    /// Same as the original version of WindowManager.EnsureWindow axcept that it creates a DXWindow instead of Window.
    /// </summary>
    /// <param name="model">The view model.</param>
    /// <param name="view">The view.</param>
    /// <param name="isDialog">Whethor or not the window is being shown as a dialog.</param>
    /// <returns>The window.</returns>
    protected override Window EnsureWindow(object model, object view, bool isDialog)
    {
      var window = view as Window;

      if (window == null)
      {
        window = new DXWindow
        {
          Content = view,
          SizeToContent = SizeToContent.WidthAndHeight
        };

        window.SetValue(View.IsGeneratedProperty, true);

        var owner = InferOwnerOf(window);
        if (owner != null)
        {
          window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
          window.Owner = owner;
        }
        else
        {
          window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
      }
      else
      {
        var owner = InferOwnerOf(window);
        if (owner != null && isDialog)
        {
          window.Owner = owner;
        }
      }

      return window;
    }
  }
}