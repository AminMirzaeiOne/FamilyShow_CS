using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FamilyShow
{
    public class BindableExtender
    {
        public static string GetBindableText(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableTextProperty);
        }

        public static void SetBindableText(DependencyObject obj,
           string value)
        {
            obj.SetValue(BindableTextProperty, value);
        }

        public static readonly DependencyProperty BindableTextProperty =
           DependencyProperty.RegisterAttached("BindableText",
               typeof(string),
               typeof(BindableExtender),
               new UIPropertyMetadata(null,
                   BindableTextProperty_PropertyChanged));
    }
}
