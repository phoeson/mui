using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>The severity level that determines the colour and icon of an <see cref="InfoBar"/>.</summary>
    public enum InfoBarSeverity
    {
        /// <summary>Neutral informational message (blue).</summary>
        Information,
        /// <summary>Positive confirmation message (green).</summary>
        Success,
        /// <summary>Advisory or caution message (amber).</summary>
        Warning,
        /// <summary>Failure or critical alert message (red).</summary>
        Error
    }

    /// <summary>
    /// An inline notification strip that displays a severity-coloured message
    /// with an optional title and dismiss button.
    /// </summary>
    /// <remarks>
    /// Template parts:
    /// <list type="bullet">
    ///   <item><see cref="PartCloseButton"/> — dismisses the bar by setting <see cref="IsOpen"/> to <c>false</c>.</item>
    /// </list>
    /// Set <see cref="IsOpen"/> to <c>false</c> to hide the bar programmatically; bind it two-way
    /// to restore it from a view-model.
    /// </remarks>
    [TemplatePart(Name = PartCloseButton, Type = typeof(Button))]
    public class InfoBar : Control
    {
        private const string PartCloseButton = "PART_CloseButton";

        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(InfoBar),
                new PropertyMetadata(string.Empty));

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(InfoBar),
                new PropertyMetadata(string.Empty));

        /// <summary>Identifies the <see cref="Severity"/> dependency property.</summary>
        public static readonly DependencyProperty SeverityProperty =
            DependencyProperty.Register("Severity", typeof(InfoBarSeverity), typeof(InfoBar),
                new PropertyMetadata(InfoBarSeverity.Information));

        /// <summary>Identifies the <see cref="IsOpen"/> dependency property.</summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(InfoBar),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>Identifies the <see cref="IsClosable"/> dependency property.</summary>
        public static readonly DependencyProperty IsClosableProperty =
            DependencyProperty.Register("IsClosable", typeof(bool), typeof(InfoBar),
                new PropertyMetadata(true));

        // ── Routed Events ────────────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Closed"/> routed event.</summary>
        public static readonly RoutedEvent ClosedEvent =
            EventManager.RegisterRoutedEvent("Closed", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(InfoBar));

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="InfoBar"/>.</summary>
        public InfoBar()
        {
            DefaultStyleKey = typeof(InfoBar);
        }

        // ── Template ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var closeButton = GetTemplateChild(PartCloseButton) as Button;
            if (closeButton != null)
            {
                closeButton.Click += OnCloseButtonClick;
            }
        }

        // ── Event handlers ───────────────────────────────────────────────────

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
            RaiseEvent(new RoutedEventArgs(ClosedEvent, this));
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the bold heading text displayed above the message.</summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>Gets or sets the body text of the notification.</summary>
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        /// <summary>Gets or sets the severity level that determines colour and icon.</summary>
        public InfoBarSeverity Severity
        {
            get { return (InfoBarSeverity)GetValue(SeverityProperty); }
            set { SetValue(SeverityProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the bar is visible.
        /// Supports two-way binding; the dismiss button sets this to <c>false</c>.
        /// </summary>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether the dismiss (×) button is shown.</summary>
        public bool IsClosable
        {
            get { return (bool)GetValue(IsClosableProperty); }
            set { SetValue(IsClosableProperty, value); }
        }

        /// <summary>Raised after the user dismisses the bar via the close button.</summary>
        public event RoutedEventHandler Closed
        {
            add { AddHandler(ClosedEvent, value); }
            remove { RemoveHandler(ClosedEvent, value); }
        }
    }
}
