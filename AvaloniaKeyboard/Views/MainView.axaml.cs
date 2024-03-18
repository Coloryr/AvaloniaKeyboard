using Avalonia.Controls;

namespace AvaloniaKeyboard.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();

        Input1.SetTextBox(Text1);
    }
}
