using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A sliding on/off toggle switch with an optional header label and
    /// customisable state labels.
    /// </summary>
    /// <remarks>
    /// Template parts:
    /// <list type="bullet">
    ///   <item><see cref="PartTrack"/> — the inner <see cref="ToggleButton"/> whose checked state
    ///   drives the sliding thumb animation.</item>
    /// </list>
    /// Bind <see cref="IsOn"/> two-way to a boolean property.  Listen to <see cref="Toggled"/>
    /// for user-interaction callbacks.
    /// </remarks>
    [TemplatePart(Name = PartTrack, Type = typeof(ToggleButton))]
    public class ToggleSwitch : Control
    {
        private const string PartTrack = "PART_Track";

        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="IsOn"/> dependency property.</summary>
        public static readonly DependencyProperty IsOnProperty =
            DependencyProperty.Register("IsOn", typeof(bool), typeof(ToggleSwitch),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsOnChanged));

        /// <summary>Identifies the <see cref="Header"/> dependency property.</summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(ToggleSwitch),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="OnContent"/> dependency property.</summary>
        public static readonly DependencyProperty OnContentProperty =
            DependencyProperty.Register("OnContent", typeof(object), typeof(ToggleSwitch),
                new PropertyMetadata("On"));

        /// <summary>Identifies the <see cref="OffContent"/> dependency property.</summary>
        public static readonly DependencyProperty OffContentProperty =
            DependencyProperty.Register("OffContent", typeof(object), typeof(ToggleSwitch),
                new PropertyMetadata("Off"));

        // ── Routed Events ────────────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Toggled"/> routed event.</summary>
        public static readonly RoutedEvent ToggledEvent =
            EventManager.RegisterRoutedEvent("Toggled", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ToggleSwitch));

        // ── Private fields ───────────────────────────────────────────────────

        private ToggleButton _track;
        private bool _isUpdating;

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="ToggleSwitch"/>.</summary>
        public ToggleSwitch()
        {
            DefaultStyleKey = typeof(ToggleSwitch);
        }

        // ── Template ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_track != null)
            {
                _track.Checked -= OnTrackCheckedChanged;
                _track.Unchecked -= OnTrackCheckedChanged;
            }

            _track = GetTemplateChild(PartTrack) as ToggleButton;

            if (_track != null)
            {
                // Sync initial state before attaching handlers to avoid spurious Toggled events.
                _track.IsChecked = IsOn;
                _track.Checked += OnTrackCheckedChanged;
                _track.Unchecked += OnTrackCheckedChanged;
            }
        }

        // ── Event handlers ───────────────────────────────────────────────────

        private void OnTrackCheckedChanged(object sender, RoutedEventArgs e)
        {
            if (_isUpdating) return;
            _isUpdating = true;
            IsOn = _track.IsChecked == true;
            _isUpdating = false;
            RaiseEvent(new RoutedEventArgs(ToggledEvent, this));
        }

        // ── DP callbacks ──────────────────────────────────────────────────────

        private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var toggle = (ToggleSwitch)d;
            if (toggle._isUpdating) return;
            toggle._isUpdating = true;
            if (toggle._track != null)
            {
                toggle._track.IsChecked = (bool)e.NewValue;
            }
            toggle._isUpdating = false;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets a value indicating whether the switch is in the On position. Supports two-way binding.</summary>
        public bool IsOn
        {
            get { return (bool)GetValue(IsOnProperty); }
            set { SetValue(IsOnProperty, value); }
        }

        /// <summary>Gets or sets the label displayed above the switch track. Hidden when <c>null</c>.</summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>Gets or sets the label shown next to the track when the switch is On. Defaults to <c>"On"</c>.</summary>
        public object OnContent
        {
            get { return GetValue(OnContentProperty); }
            set { SetValue(OnContentProperty, value); }
        }

        /// <summary>Gets or sets the label shown next to the track when the switch is Off. Defaults to <c>"Off"</c>.</summary>
        public object OffContent
        {
            get { return GetValue(OffContentProperty); }
            set { SetValue(OffContentProperty, value); }
        }

        /// <summary>Raised when the user toggles the switch via the track button.</summary>
        public event RoutedEventHandler Toggled
        {
            add { AddHandler(ToggledEvent, value); }
            remove { RemoveHandler(ToggledEvent, value); }
        }
    }
}
