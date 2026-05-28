using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A compound button that combines a primary action area with a dropdown
    /// toggle that opens a <see cref="ContextMenu"/> for additional commands.
    /// </summary>
    /// <remarks>
    /// Template parts:
    /// <list type="bullet">
    ///   <item><see cref="PartActionButton"/> — executes the primary <see cref="Command"/> and raises <see cref="Click"/>.</item>
    ///   <item><see cref="PartDropDownButton"/> — opens the <see cref="DropDown"/> context menu.</item>
    /// </list>
    /// Assign a <see cref="ContextMenu"/> to <see cref="DropDown"/> to supply additional actions.
    /// Bind <see cref="Command"/> for the primary action; listen to <see cref="Click"/> for callback.
    /// </remarks>
    [TemplatePart(Name = PartActionButton,   Type = typeof(Button))]
    [TemplatePart(Name = PartDropDownButton, Type = typeof(ToggleButton))]
    public class SplitButton : Control
    {
        private const string PartActionButton   = "PART_ActionButton";
        private const string PartDropDownButton = "PART_DropDownButton";

        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Content"/> dependency property.</summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(SplitButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Command"/> dependency property.</summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(SplitButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(SplitButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="CommandTarget"/> dependency property.</summary>
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(SplitButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="DropDown"/> dependency property.</summary>
        public static readonly DependencyProperty DropDownProperty =
            DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(SplitButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsDropDownOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(SplitButton),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsDropDownOpenChanged));

        // ── Routed Events ────────────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Click"/> routed event.</summary>
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(SplitButton));

        // ── Private fields ───────────────────────────────────────────────────

        private Button       _actionButton;
        private ToggleButton _dropDownButton;

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="SplitButton"/>.</summary>
        public SplitButton()
        {
            DefaultStyleKey = typeof(SplitButton);
        }

        // ── Template ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_actionButton   != null) _actionButton.Click     -= OnActionButtonClick;
            if (_dropDownButton != null) _dropDownButton.Checked -= OnDropDownButtonChecked;

            _actionButton   = GetTemplateChild(PartActionButton)   as Button;
            _dropDownButton = GetTemplateChild(PartDropDownButton) as ToggleButton;

            if (_actionButton   != null) _actionButton.Click     += OnActionButtonClick;
            if (_dropDownButton != null) _dropDownButton.Checked += OnDropDownButtonChecked;
        }

        // ── Event handlers ───────────────────────────────────────────────────

        private void OnActionButtonClick(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            var cmd   = Command;
            var param = CommandParameter;
            if (cmd != null && cmd.CanExecute(param))
                cmd.Execute(param);
        }

        private void OnDropDownButtonChecked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = true;
        }

        // ── DP callbacks ──────────────────────────────────────────────────────

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var btn    = (SplitButton)d;
            var isOpen = (bool)e.NewValue;

            if (!isOpen) return;

            var menu = btn.DropDown;
            if (menu == null)
            {
                btn.IsDropDownOpen = false;
                return;
            }

            menu.PlacementTarget = btn;
            menu.Placement       = PlacementMode.Bottom;
            menu.Closed         += btn.OnMenuClosed;
            menu.IsOpen          = true;
        }

        private void OnMenuClosed(object sender, RoutedEventArgs e)
        {
            ((ContextMenu)sender).Closed -= OnMenuClosed;
            if (_dropDownButton != null) _dropDownButton.IsChecked = false;
            IsDropDownOpen = false;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the content displayed in the primary action area.</summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>Gets or sets the command executed when the primary button is clicked.</summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>Gets or sets the parameter passed to <see cref="Command"/>.</summary>
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>Gets or sets the element on which the command is raised.</summary>
        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        /// <summary>Gets or sets the <see cref="ContextMenu"/> shown when the dropdown arrow is clicked.</summary>
        public ContextMenu DropDown
        {
            get { return (ContextMenu)GetValue(DropDownProperty); }
            set { SetValue(DropDownProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether the dropdown menu is open. Supports two-way binding.</summary>
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }

        /// <summary>Raised when the primary action button is clicked.</summary>
        public event RoutedEventHandler Click
        {
            add    { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
    }
}
