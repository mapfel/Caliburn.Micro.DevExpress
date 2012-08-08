using System.Windows;

namespace WPFSample1
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      InitializeComponent();
      DevExpress.Xpf.Core.ThemeManager.ApplicationThemeName = "MetropolisDark";// "Office2010Black";
    }
  }
}