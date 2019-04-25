using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace CloudRegisterManage.UI
{
    public class NumberBox:TextBox
    {
        public NumberBox()
        {
            this.CommandBindings.Add(new CommandBinding(
                ApplicationCommands.Paste, 
                new ExecutedRoutedEventHandler((o, arg) => arg.Handled = true)
                ));
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;

            }
            
        }

        
    }
}
