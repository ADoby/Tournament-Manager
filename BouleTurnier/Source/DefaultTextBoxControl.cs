using System;
using System.Windows.Controls;
using System.Windows.Input;
public class DefaultTextBoxControl : TextBox
{
    public event EventHandler<EventArgs> DefaultAction = delegate { };

    public DefaultTextBoxControl()
    {
        PreviewKeyUp += DefaultTextBoxControl_PreviewKeyUp;
    }

    void DefaultTextBoxControl_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }
        DefaultAction(this, EventArgs.Empty);
    }
}
