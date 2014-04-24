using System;

namespace Caliburn.Micro.DevExpress
{
  /// <summary>
  /// Conductor to support DevExpress DocumentGroup binding. It uses the new ActiveItemIndex property to select the active item
  /// and CloseAction to provide a way to close document panels.
  /// </summary>
  /// <typeparam name="T">Conducted type</typeparam>
  public class DevExpressConductorOneActive<T> : Conductor<T>.Collection.OneActive where T : class
  {
    public DevExpressConductorOneActive()
      : base()
    {
      ActivationProcessed += DXActivationProcessed;
    }

    /// <summary>
    /// Action to support closing bound viewmodels.
    /// </summary>
    public Action<T> CloseAction
    {
      get
      {
        return OnClose;
      }
    }

    /// <summary>
    /// Close the viewmodel, and then activate the viewmodel left to the closed one.
    /// </summary>
    /// <param name="param">The viewmodel to be closed.</param>
    private void OnClose(T param)
    {
      int active = ActiveItemIndex;
      DeactivateItem(param, true);
      if (active > 0)
        if (active == Items.Count)
          ActiveItemIndex = Items.Count - 1;
        else
          ActiveItemIndex = active - 1;
    }

    /// <summary>
    /// Gets or sets the active item's index.
    /// </summary>
    public int ActiveItemIndex
    {
      get { return Items.IndexOf(ActiveItem); }
      set
      {
        if (Items.Count > value)
        {
          ActivateItem(Items[value]);
          NotifyOfPropertyChange(() => ActiveItemIndex);
        }
      }
    }

    private void DXActivationProcessed(object sender, ActivationProcessedEventArgs e)
    {
      NotifyOfPropertyChange(() => ActiveItemIndex);
    }
  }
}