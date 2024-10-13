using System.Windows;

namespace MyVPN.Core
{
    internal class Extensions
    {
        public static readonly DependencyProperty Icon = 
            DependencyProperty.RegisterAttached("Icon", typeof(string), typeof(Extensions), new PropertyMetadata(default(string)));
        
        public static void SetIcon(UIElement element, string val)
        {
            element.SetCurrentValue(Icon, val);
        }

        public static string GetIcon(UIElement element)
        {
            return (string)element.GetValue(Icon);
        }
    }
}
