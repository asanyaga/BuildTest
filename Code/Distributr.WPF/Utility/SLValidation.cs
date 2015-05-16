using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace Distributr.WPF.UI.Utility
{
    public static class SLValidation
    {

        public static bool AllowNumberOnlyOnKeyDown(KeyEventArgs e)
        {
            bool result = false;
            ModifierKeys keys = Keyboard.Modifiers;
            bool shiftKey = (keys & ModifierKeys.Shift) != 0;
            if (e.Key < Key.D0 || e.Key > Key.D9)
            {

                if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                {
                    if (e.Key != Key.Back && e.Key != Key.RightShift && e.Key != Key.LeftShift )
                    {
                        result = true;
                    }
                }
            }
            if (shiftKey)
                result = true;
            return result;
        }

        public static void InvalidateSpecialCharactersOnKeyUp(object sender)
        {
            TextBox txtBox = sender as TextBox;
            string input = txtBox.Text;

            const string specialCharacters = "!@#$%^&*()+=-[]\\;,./{}|\":<>?";//\'
            //foreach (char t in specialCharacters)
            //{
            //    for (int j = 0; j < input.Length; j++)
            //    {
            //        if (input[j] != t) continue;
            //        input = input.Remove(input.IndexOf(t, 0), 1);
            //        break;
            //    }
            //}

            foreach (char t in input)
            {
                if (specialCharacters.Contains(t.ToString()))
                    input = input.Remove(input.IndexOf(t, 0), 1);
            }

            if (input != txtBox.Text)
            {
                txtBox.Text = input;
                txtBox.SelectionStart = txtBox.Text.Length;
            }
        }

        public static void InvalidateSpecialCharactersOnKeyUp(object sender, string specialCharacters)
        {
            TextBox txtBox = sender as TextBox;
            string input = txtBox.Text;

            //foreach (char t in specialCharacters)
            //{
            //    for (int j = 0; j < input.Length; j++)
            //    {
            //        if (input[j] != t) continue;
            //        input = input.Remove(input.IndexOf(t, 0), 1);
            //        break;
            //    }
            //}

            foreach (char t in input)
            {
                if (specialCharacters.Contains(t.ToString()))
                    input = input.Remove(input.IndexOf(t, 0), 1);
            }

            if (input != txtBox.Text)
            {
                txtBox.Text = input;
                txtBox.SelectionStart = txtBox.Text.Length;
            }
        }
    }
}
