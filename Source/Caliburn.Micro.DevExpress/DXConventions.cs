using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Xpf.PivotGrid;
using DXGroupBox = DevExpress.Xpf.LayoutControl.GroupBox;
using LayoutGroup = DevExpress.Xpf.Docking.LayoutGroup;
using DevExpress.Xpf.Charts;

namespace Caliburn.Micro.DevExpress
{
  public static class DXConventions
  {
    private static readonly ILog Log = LogManager.GetLog(typeof(DXConventions));
    private static bool Installed;

    /// <summary>
    /// Installs conventions for DevExpress controls.
    /// This is necessary to use Caliburn Micro with DevExpress controls.
    /// </summary>
    public static void Install()
    {
      if (Installed)
        return;
      //ReplaceBindProperties();
      ReplaceGetNamedElements();
      ReplaceDerivePotentialSelectionNames();
      InstallElementConventions();
      Installed = true;
    }

    /// <summary>
    /// Element conventions for DevExpress controls.
    /// </summary>
    private static void InstallElementConventions()
    {
      #region DevExpress specific

      //Grid
      ConventionManager.AddElementConvention<DataControlBase>(DataControlBase.ItemsSourceProperty, "DataContext", "Loaded");

      //PivotGrid
      ConventionManager.AddElementConvention<PivotGridControl>(PivotGridControl.DataSourceProperty, "DataContext", "Loaded");

      //LayoutControl
      ConventionManager.AddElementConvention<DataLayoutControl>(DataLayoutControl.CurrentItemProperty, "DataContext", "Loaded");

      //ChartControl
      ConventionManager.AddElementConvention<ChartControl>(ChartControl.DataSourceProperty, "DataContext", "Loaded");

      //Editors
      ConventionManager.AddElementConvention<LookUpEditBase>(LookUpEditBase.ItemsSourceProperty, "SelectedItem", "SelectedIndexChanged")
        .ApplyBinding = (viewModelType, path, property, element, convention) =>
        {
          var bindableProperty = convention.GetBindableProperty(element);
          if (!ConventionManager.SetBindingWithoutBindingOrValueOverwrite(viewModelType, path, property, element, convention, bindableProperty))
            return false;
          ConventionManager.ConfigureSelectedItem(element, LookUpEditBase.SelectedItemProperty, viewModelType, path);
          return true;
        };

      //Docking
      ConventionManager.AddElementConvention<DocumentGroup>(DocumentGroup.ItemsSourceProperty, "ItemsSource", "SelectedItemChanged")
    .ApplyBinding = (viewModelType, path, property, element, convention) =>
    {
      var bindableProperty = convention.GetBindableProperty(element);
      if (!ConventionManager.SetBindingWithoutBindingOverwrite(viewModelType, path, property, element, convention, bindableProperty))
        return false;

      var documentGroup = (DocumentGroup)element;
      if (documentGroup.ItemContentTemplate == null
          && documentGroup.ItemContentTemplateSelector == null
          && property.PropertyType.IsGenericType)
      {
        var itemType = property.PropertyType.GetGenericArguments().First();
        if (!itemType.IsValueType && !typeof(string).IsAssignableFrom(itemType))
        {
          documentGroup.ItemContentTemplate = ConventionManager.DefaultItemTemplate;
          Log.Info("ContentTemplate applied to {0}.", documentGroup.Name);
          if (documentGroup.ItemStyle == null)
          {
            Style style = new Style(typeof(DocumentPanel));
            style.Setters.Add(new Setter(DocumentPanel.CloseCommandProperty, new Binding("CloseCommand")));
            documentGroup.ItemStyle = style;
            Log.Info("ItemStyle applied to {0}.", documentGroup.Name);
          }
        }
      }

      ConventionManager.ConfigureSelectedItem(element, DocumentGroup.SelectedTabIndexProperty, viewModelType, path);

      ConventionManager.ApplyHeaderTemplate(documentGroup, DocumentGroup.ItemCaptionTemplateProperty, DocumentGroup.ItemCaptionTemplateSelectorProperty, viewModelType);

      return true;
    };

      #endregion DevExpress specific
    }

    /// <summary>
    /// Replacing ConventionManager.DerivePotentialSelectionNames to support DevExpressConductorOneActive<T>.ActiveItemIndex binding
    /// because ActiveItem cannot be bound to LayoutGroup.SelectedItem (see http://www.devexpress.com/Support/Center/Question/Details/Q401650)
    /// </summary>
    private static void ReplaceDerivePotentialSelectionNames()
    {
      ConventionManager.DerivePotentialSelectionNames = name =>
      {
        var singular = ConventionManager.Singularize(name);
        return new[] {
                String.Format("Active{0}Index", singular),
                "Active" + singular,
                "Selected" + singular,
                "Current" + singular
            };
      };
    }

    /// <summary>
    /// Replacing BindingScope.GetNamedElements to support BaseLayoutItem.Name and DevExpress controls which behave like ContentControl or ItemsControl
    /// </summary>
    private static void ReplaceGetNamedElements()
    {
      BindingScope.GetNamedElements = elementInScope =>
      {
        var root = elementInScope;
        var previous = elementInScope;
        DependencyObject contentPresenter = null;
        var routeHops = new Dictionary<DependencyObject, DependencyObject>();

        while (true)
        {
          if (root == null)
          {
            root = previous;
            break;
          }

          if (root is UserControl)
            break;
#if !SILVERLIGHT
          if (root is Page)
          {
            root = ((Page)root).Content as DependencyObject ?? root;
            break;
          }
#endif
          if ((bool)root.GetValue(View.IsScopeRootProperty))
            break;

          if (root is ContentPresenter)
            contentPresenter = root;
          else if (root is ItemsPresenter && contentPresenter != null)
          {
            routeHops[root] = contentPresenter;
            contentPresenter = null;
          }

          previous = root;
          root = VisualTreeHelper.GetParent(previous);
        }

        var descendants = new List<FrameworkElement>();
        var queue = new Queue<DependencyObject>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
          var current = queue.Dequeue();
          var currentElement = current as FrameworkElement;

          if (currentElement != null && !string.IsNullOrEmpty(currentElement.Name))
            descendants.Add(currentElement);

          #region DevExpress specific

          else
          {
            //BaseLayoutItem defines it's own Name property thus hiding FrameworkElement.Name
            var currentLayoutElement = current as BaseLayoutItem;
            if (currentLayoutElement != null && !string.IsNullOrEmpty(currentLayoutElement.Name))
            {
              descendants.Add(currentLayoutElement);

              //Setting FrameworkElement.Name to BaseLayoutItem.Name so later Caliburn can reference the name and so no need to replace ViewModelBinder.BindProperties
              currentElement.Name = currentLayoutElement.Name;
            }
          }

          #endregion DevExpress specific

          if (current is UserControl && current != root)
            continue;

          if (routeHops.ContainsKey(current))
          {
            queue.Enqueue(routeHops[current]);
            continue;
          }

#if NET
                var childCount = (current is UIElement || current is UIElement3D || current is ContainerVisual ? VisualTreeHelper.GetChildrenCount(current) : 0);
#else
          var childCount = VisualTreeHelper.GetChildrenCount(current);
#endif
          if (childCount > 0)
          {
            for (var i = 0; i < childCount; i++)
            {
              var childDo = VisualTreeHelper.GetChild(current, i);
              queue.Enqueue(childDo);
            }
          }
          else
          {
            var contentControl = current as ContentControl;
            if (contentControl != null)
            {
              if (contentControl.Content is DependencyObject)
                queue.Enqueue(contentControl.Content as DependencyObject);
#if !SILVERLIGHT
              var headeredControl = contentControl as HeaderedContentControl;
              if (headeredControl != null && headeredControl.Header is DependencyObject)
                queue.Enqueue(headeredControl.Header as DependencyObject);
#endif
            }
            else
            {
              var itemsControl = current as ItemsControl;
              if (itemsControl != null)
              {
                itemsControl.Items.OfType<DependencyObject>()
                    .Apply(queue.Enqueue);
#if !SILVERLIGHT
                var headeredControl = itemsControl as HeaderedItemsControl;
                if (headeredControl != null && headeredControl.Header is DependencyObject)
                  queue.Enqueue(headeredControl.Header as DependencyObject);
#endif
              }

              #region DevExpress specific

              //Handling DevExpress controls which behave like ContentControl or ItemsControl

              else
              {
                TypeSwitch.Do(current,
                    TypeSwitch.Case<DockLayoutManager>(x =>
                    {
                      if (x.LayoutRoot != null) queue.Enqueue(x.LayoutRoot);
                    }),
                    TypeSwitch.Case<LayoutGroup>(x =>
                    {
                      if (x.Items != null) x.Items.OfType<DependencyObject>().Apply(queue.Enqueue);
                    }),
                    TypeSwitch.Case<LayoutPanel>(x =>
                    {
                      if (x.Control != null) queue.Enqueue(x.Control);
                    }),
                    TypeSwitch.Case<LayoutItem>(x =>
                    {
                      if (x.Content != null) queue.Enqueue(x.Content);
                    }),
                    TypeSwitch.Case<GridControl>(x =>
                    {
                      if (x.View != null && !string.IsNullOrWhiteSpace(x.View.Name)) queue.Enqueue(x.View);
                    }),
                    TypeSwitch.Case<DXGroupBox>(x =>
                    {
                      var groupContent = x.Content as DependencyObject;
                      if (groupContent != null) queue.Enqueue(groupContent);
                    }),
                    TypeSwitch.Default(() =>
                    {
                      Log.Info("DX Convention missing for {0}.", current.GetType().Name);
                    }));
              }

              #endregion DevExpress specific
            }
          }
        }

        return descendants;
      };
    }

    ///// <summary>
    ///// Replacing ViewModelBinder.BindProperties to support BaseLayoutItem.Name (see http://www.devexpress.com/Support/Center/Question/Details/Q401308)
    ///// </summary>
    //private static void ReplaceBindProperties()
    //{
    //  ViewModelBinder.BindProperties = (namedElements, viewModelType) =>
    //  {
    //    var unmatchedElements = new List<FrameworkElement>();

    //    foreach (var element in namedElements)
    //    {
    //      #region DevExpress specific

    //      //BaseLayoutItem defines it's own Name property thus hiding FrameworkElement.Name
    //      //name variable is used to store the name of the element being processed (and all element.Name references are changed to name)
    //      string name;
    //      if (string.IsNullOrWhiteSpace(element.Name))
    //      {
    //        var layoutelement = element as BaseLayoutItem;
    //        if (layoutelement != null)
    //        {
    //          name = layoutelement.Name;
    //        }
    //        else
    //          continue;
    //      }
    //      else
    //        name = element.Name;

    //      #endregion DevExpress specific

    //      var cleanName = name.Trim('_');
    //      var parts = cleanName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

    //      var property = viewModelType.GetPropertyCaseInsensitive(parts[0]);
    //      var interpretedViewModelType = viewModelType;

    //      for (int i = 1; i < parts.Length && property != null; i++)
    //      {
    //        interpretedViewModelType = property.PropertyType;
    //        property = interpretedViewModelType.GetPropertyCaseInsensitive(parts[i]);
    //      }

    //      if (property == null)
    //      {
    //        unmatchedElements.Add(element);
    //        Log.Info("Binding Convention Not Applied: Element {0} did not match a property.", name);
    //        continue;
    //      }

    //      var convention = ConventionManager.GetElementConvention(element.GetType());
    //      if (convention == null)
    //      {
    //        unmatchedElements.Add(element);
    //        Log.Warn("Binding Convention Not Applied: No conventions configured for {0}.", element.GetType());
    //        continue;
    //      }

    //      var applied = convention.ApplyBinding(
    //          interpretedViewModelType,
    //          cleanName.Replace('_', '.'),
    //          property,
    //          element,
    //          convention
    //          );

    //      if (applied)
    //      {
    //        Log.Info("Binding Convention Applied: Element {0}.", name);
    //      }
    //      else
    //      {
    //        Log.Info("Binding Convention Not Applied: Element {0} has existing binding.", name);
    //        unmatchedElements.Add(element);
    //      }
    //    }

    //    return unmatchedElements;
    //  };
    //}

    //    /// <summary>
    //    /// The default DataTemplate used for ItemsControls when required.
    //    /// </summary>
    //    public static DataTemplate DefaultItemTemplate = (DataTemplate)
    //#if SILVERLIGHT
    //          XamlReader.Load(
    //#else
    //XamlReader.Parse(
    //#endif
    //"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
    //                      "xmlns:cal='clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro' " +
    //                      "xmlns:cald='clr-namespace:Caliburn.Micro.DevExpress;assembly=Caliburn.Micro.DevExpress'> " +
    //            "<ContentControl cal:View.Model=\"{Binding}\" cald:DevExpressView.ActiveItem=\"{Binding}\" VerticalContentAlignment=\"Stretch\" HorizontalContentAlignment=\"Stretch\" IsTabStop=\"False\" />" +
    //        "</DataTemplate>"
    //        );
  }
}