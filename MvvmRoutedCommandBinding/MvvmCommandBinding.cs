using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MvvmRoutedCommandBinding
{
    public class MvvmCommandBinding : Freezable
    {
        UIElement _uiElement;

        readonly CommandBinding _commandBinding;

        public MvvmCommandBinding()
        {
            _commandBinding = new CommandBinding();

            _commandBinding.CanExecute += OnCanExecute;
            _commandBinding.Executed += OnExecute;
        }

        #region Command

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(MvvmCommandBinding),
            new PropertyMetadata(null, OnCommandChanged));

        static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MvvmCommandBinding)d).OnCommandChanged((ICommand)e.NewValue);
        }

        void OnCommandChanged(ICommand newValue)
        {
            _commandBinding.Command = newValue;
        }

        [Bindable(true)]
        public ICommand Command
        {
            get => (ICommand) GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        #endregion

        #region Target

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target", typeof(ICommand), typeof(MvvmCommandBinding),
            new PropertyMetadata(null, OnTargetChanged));

        [Bindable(true)]
        public ICommand Target
        {
            get => (ICommand)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MvvmCommandBinding)d).OnTargetChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        void OnTargetChanged(ICommand oldValue, ICommand newValue)
        {
            if (oldValue != null)
            {
                oldValue.CanExecuteChanged -= OnTargetCanExecuteChanged;
            }

            if (newValue != null)
            {
                newValue.CanExecuteChanged += OnTargetCanExecuteChanged;
            }

            CommandManager.InvalidateRequerySuggested();
        }

        #endregion

        #region RoutedCommandTarget

        public static readonly DependencyProperty RoutedCommandTargetProperty = DependencyProperty.Register(
            "RoutedCommandTarget", typeof(IInputElement), typeof(MvvmCommandBinding),
            new PropertyMetadata(null));

        [Bindable(true)]
        public IInputElement RoutedCommandTarget
        {
            get => (IInputElement)GetValue(RoutedCommandTargetProperty);
            set => SetValue(RoutedCommandTargetProperty, value);
        }

        #endregion

        #region CanExecuteChangedSuggestRequery

        public static readonly DependencyProperty CanExecuteChangedSuggestRequeryProperty
            = DependencyProperty.Register(
                "CanExecuteChangedSuggestRequery", typeof(bool), typeof(MvvmCommandBinding),
                new PropertyMetadata(false, OnCanExecuteChangedSuggestRequeryChanged));

        [Bindable(true)]
        public bool CanExecuteChangedSuggestRequery
        {
            get => (bool)GetValue(CanExecuteChangedSuggestRequeryProperty);
            set => SetValue(CanExecuteChangedSuggestRequeryProperty, value);
        }

        static void OnCanExecuteChangedSuggestRequeryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MvvmCommandBinding)d).OnCanExecuteChangedSuggestRequeryChanged((bool)e.NewValue);
        }

        void OnCanExecuteChangedSuggestRequeryChanged(bool newValue)
        {
            if (newValue)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        #endregion

        #region On event

        void OnTargetCanExecuteChanged(object sender, EventArgs e)
        {
            if (CanExecuteChangedSuggestRequery)
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }

        void OnExecute(object sender, ExecutedRoutedEventArgs e)
        {
            if (Target == null) return;

            if (Target is RoutedCommand routedCommand)
            {
                routedCommand.Execute(e.Parameter, RoutedCommandTarget);
            }
            else
            {
                Target.Execute(e.Parameter);
            }

            e.Handled = true;
        }

        void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (Target == null) return;

            e.Handled = true;
            e.CanExecute = false;

            if (Target is RoutedCommand routedCommand)
            {
                e.CanExecute = routedCommand.CanExecute(e.Parameter, RoutedCommandTarget);
            }
            else
            {
                e.CanExecute = Target.CanExecute(e.Parameter);
            }
        }

        #endregion

        #region Attach / Dettach

        internal void DettachFrom(UIElement uiDependencyObject)
        {
            if (uiDependencyObject == null) throw new ArgumentNullException(nameof(uiDependencyObject));
            WritePreamble();

            if (!ReferenceEquals(uiDependencyObject, _uiElement)) return;

            Dettach();
        }

        void Dettach()
        {
            _uiElement.CommandBindings.Remove(_commandBinding);
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
            uiDependencyObject.CommandBindings.Add(_commandBinding);
        }

        #endregion

        protected override Freezable CreateInstanceCore()
        {
            return new MvvmCommandBinding();
        }
    }
}