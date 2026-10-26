using MvvmDemo.ViewModels;
using System.Windows;

namespace MvvmDemo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    protected override void OnClosed(EventArgs e)
    {
        // Window lifecycle belongs to the view; data/actions live in the view model.
        (DataContext as MainViewModel)?.Dispose();
        base.OnClosed(e);
    }
}


