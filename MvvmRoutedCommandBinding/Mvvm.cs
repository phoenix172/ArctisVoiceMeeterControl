using System.Windows;

namespace MvvmRoutedCommandBinding
{
    public static class Mvvm
    {
        internal static readonly DependencyProperty CommandBindingsProperty = DependencyProperty.RegisterAttached(
            "CommandBindingsInternal", typeof(MvvmCommandBindingCollection), typeof(Mvvm),
            new PropertyMetadata(OnCommandBindingsChanged));

        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static MvvmCommandBindingCollection GetCommandBindings(UIElement target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var commandBindings = (MvvmCommandBindingCollection)target.GetValue(CommandBindingsProperty);
            if (commandBindings != null) return commandBindings;

            commandBindings = new MvvmCommandBindingCollection();
            target.SetValue(CommandBindingsProperty, commandBindings);

            return commandBindings;
        }

        public static void SetCommandBindings(UIElement target, MvvmCommandBindingCollection commandBindings)
        {
            if(target == null ) throw new ArgumentNullException(nameof(target));
            target.SetValue(CommandBindingsProperty, commandBindings);
        }

        private static void OnCommandBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is UIElement uiDependencyObject)) return;

            if (e.OldValue is MvvmCommandBindingCollection oldValue)
            {
                oldValue.DettachFrom(uiDependencyObject);
            }

            if (e.NewValue is MvvmCommandBindingCollection newValue)
            {
                newValue.AttachTo(uiDependencyObject);
            }
        }
    }
}