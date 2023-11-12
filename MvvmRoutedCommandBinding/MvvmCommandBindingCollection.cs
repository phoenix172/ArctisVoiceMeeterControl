using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace MvvmRoutedCommandBinding
{
    /// <summary>
    /// Set an element <see cref="UIElement.CommandBindings"/> with a syntax allowing to specify an
    /// <see cref="ICommand"/> instance using bindings if required.
    /// </summary>
    /// <example>
    /// &lt;u:Mvvm.CommandBindings&gt;
    ///     &lt;u:MvvmCommandBindingCollection&gt;
    ///         &lt;u:MvvmCommandBinding Command="cmd:RoutedCommands.SomeCommand"
    ///                               Target="{Binding CommandInViewModel}" /&gt;
    ///     &lt;/u:MvvmCommandBindingCollection&gt;
    /// &lt;/u:Mvvm.CommandBindings&gt;
    /// </example>
    [ContentProperty(nameof(Commands))]
    public class MvvmCommandBindingCollection : Freezable, IList
    {
        // Normally the inheritance context only goes to the logical and visual tree. But there are some additional
        // "Pointers" that exists to simplify XAML programming. The one that we use there is that the context is
        // propagated when a hierarchy of Freezable is inside a FrameworkElement.
        //
        // It is acheived by the facts that :
        //  * This class is Freezable
        //  * The collection property is a dependency property
        //  * The collection is Freezable (FreezableCollection<T> is an Animatable that is a Freezable)
        //  * The objects inside the collection are instances of Freezable

        static readonly DependencyPropertyKey CommandsPropertyReadWrite =
            DependencyProperty.RegisterReadOnly("Commands", typeof(FreezableCollection<MvvmCommandBinding>),
            typeof(MvvmCommandBindingCollection), null);

        public static readonly DependencyProperty CommandsProperty = CommandsPropertyReadWrite.DependencyProperty;

        public FreezableCollection<MvvmCommandBinding> Commands
        {
            get => (FreezableCollection<MvvmCommandBinding>)GetValue(CommandsProperty);
            private set => SetValue(CommandsPropertyReadWrite, value);
        }

        UIElement _uiElement;

        public MvvmCommandBindingCollection()
        {
            Commands = new FreezableCollection<MvvmCommandBinding>();
            ((INotifyCollectionChanged)Commands).CollectionChanged += CommandsChanged;
        }

        void CommandsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_uiElement == null) return;

            if (e.Action == NotifyCollectionChangedAction.Add
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (MvvmCommandBinding command in e.NewItems)
                {
                    command.AttachTo(_uiElement);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                || e.Action == NotifyCollectionChangedAction.Replace)
            {
                foreach (MvvmCommandBinding command in e.OldItems)
                {
                    command.DettachFrom(_uiElement);
                }
            }
        }

        internal void DettachFrom(UIElement uiDependencyObject)
        {
            if (uiDependencyObject == null) throw new ArgumentNullException(nameof(uiDependencyObject));
            WritePreamble();

            if (!ReferenceEquals(uiDependencyObject, _uiElement)) return;

            Dettach();
        }

        void Dettach()
        {
            foreach (var command in Commands)
            {
                command.DettachFrom(_uiElement);
            }

            _uiElement = null;
        }

        internal void AttachTo(UIElement uiDependencyObject)
        {
            WritePreamble();

            if (_uiElement != null)
            {
                Dettach();
            }

            _uiElement = uiDependencyObject ?? throw new ArgumentNullException(nameof(uiDependencyObject));

            foreach (var command in Commands)
            {
                command.AttachTo(_uiElement);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new MvvmCommandBindingCollection();
        }

        #region Implementation of IEnumerable

        public IEnumerator GetEnumerator()
        {
            return Commands.GetEnumerator();
        }

        #endregion

        #region Implementation of ICollection

        public void CopyTo(Array array, int index)
        {
            Commands.CopyTo((MvvmCommandBinding[])array, index);
        }

        public int Count => Commands.Count;

        public object SyncRoot => (Commands as ICollection).SyncRoot;

        public bool IsSynchronized => (Commands as ICollection).IsSynchronized;

        #endregion

        #region Implementation of IList

        public int Add(object value)
        {
            return (Commands as IList).Add(value);
        }

        public bool Contains(object value)
        {
            return (Commands as IList).Contains(value);
        }

        public void Clear()
        {
            Commands.Clear();
        }

        public int IndexOf(object value)
        {
            return (Commands as IList).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            (Commands as IList).Insert(index, value);
        }

        public void Remove(object value)
        {
            (Commands as IList).Remove(value);
        }

        public void RemoveAt(int index)
        {
            (Commands as IList).RemoveAt(index);
        }

        public object this[int index]
        {
            get => Commands[index];
            set => (Commands as IList)[index] = value;
        }

        public bool IsReadOnly => (Commands as IList).IsReadOnly;

        public bool IsFixedSize => (Commands as IList).IsFixedSize;

        #endregion
    }
}