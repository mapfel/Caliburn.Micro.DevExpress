using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caliburn.Micro.DevExpress
{
  //http://blogs.msdn.com/b/jaredpar/archive/2008/05/16/switching-on-types.aspx
  internal static class TypeSwitch
  {
    public class CaseInfo
    {
      public bool IsDefault { get; set; }

      public Type Target { get; set; }

      public Action<object> Action { get; set; }
    }

    public static void Do(object source, params CaseInfo[] cases)
    {
      var type = source.GetType();
      foreach (var entry in cases)
      {
        if (entry.IsDefault || type == entry.Target)
        {
          entry.Action(source);
          break;
        }
      }
    }

    public static CaseInfo Case<T>(System.Action action)
    {
      return new CaseInfo()
      {
        Action = x => action(),
        Target = typeof(T)
      };
    }

    public static CaseInfo Case<T>(System.Action<T> action)
    {
      return new CaseInfo()
      {
        Action = (x) => action((T)x),
        Target = typeof(T)
      };
    }

    public static CaseInfo Default(System.Action action)
    {
      return new CaseInfo()
      {
        Action = x => action(),
        IsDefault = true
      };
    }
  }
}