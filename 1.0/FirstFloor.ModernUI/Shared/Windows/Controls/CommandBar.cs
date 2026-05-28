using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A horizontal toolbar that hosts <see cref="CommandBarButton"/> and
    /// <see cref="CommandBarSeparator"/> items for screen-level actions.
    /// </summary>
    /// <remarks>
    /// Place <see cref="CommandBarButton"/> children directly, or bind <see cref="ItemsControl.ItemsSource"/>
    /// to a collection.  Set <see cref="IsCompact"/> to show icons only and hide labels.
    /// </remarks>
    public class CommandBar : ItemsControl
    {
        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="IsCompact"/> dependency property.</summary>
        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register("IsCompact", typeof(bool), typeof(CommandBar),
                new PropertyMetadata(false, OnIsCompactChanged));

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="CommandBar"/>.</summary>
        public CommandBar()
        {
            DefaultStyleKey = typeof(CommandBar);
        }

        // ── Overrides ────────────────────────────────────────────────────────

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new CommandBarButton();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is CommandBarButton || item is CommandBarSeparator;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var btn = element as CommandBarButton;
            if (btn != null) btn.IsCompact = IsCompact;
        }

        // ── DP callbacks ──────────────────────────────────────────────────────

        private static void OnIsCompactChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bar       = (CommandBar)d;
            var isCompact = (bool)e.NewValue;
            for (int i = 0; i < bar.Items.Count; i++)
            {
                var btn = bar.ItemContainerGenerator.ContainerFromIndex(i) as CommandBarButton;
                if (btn != null) btn.IsCompact = isCompact;
            }
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets a value indicating whether button labels are hidden, showing only icons.</summary>
        public bool IsCompact
        {
            get { return (bool)GetValue(IsCompactProperty); }
            set { SetValue(IsCompactProperty, value); }
        }
    }

    /// <summary>
    /// A compact icon-and-label button for use inside a <see cref="CommandBar"/>.
    /// </summary>
    /// <remarks>
    /// Set <see cref="Icon"/> to a <see cref="System.Windows.Shapes.Path"/> or other
    /// visual element.  Inherits <see cref="ButtonBase.Command"/>,
    /// <see cref="ButtonBase.CommandParameter"/>, and <see cref="ButtonBase.CommandTarget"/>
    /// from <see cref="ButtonBase"/>.
    /// </remarks>
    public class CommandBarButton : ButtonBase
    {
        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(CommandBarButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Label"/> dependency property.</summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(CommandBarButton),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="IsCompact"/> dependency property.</summary>
        public static readonly DependencyProperty IsCompactProperty =
            DependencyProperty.Register("IsCompact", typeof(bool), typeof(CommandBarButton),
                new PropertyMetadata(false));

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="CommandBarButton"/>.</summary>
        public CommandBarButton()
        {
            DefaultStyleKey = typeof(CommandBarButton);
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the icon displayed above the label. Typically a <see cref="System.Windows.Shapes.Path"/> or <see cref="System.Windows.Controls.Image"/>.</summary>
        public object Icon
        {
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>Gets or sets the text label displayed below the icon.</summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether the label is hidden, showing only the icon. Usually propagated by the parent <see cref="CommandBar"/>.</summary>
        public bool IsCompact
        {
            get { return (bool)GetValue(IsCompactProperty); }
            set { SetValue(IsCompactProperty, value); }
        }
    }

    /// <summary>
    /// A vertical divider line used to visually separate groups of buttons in a <see cref="CommandBar"/>.
    /// </summary>
    public class CommandBarSeparator : Separator
    {
        /// <summary>Initializes a new instance of <see cref="CommandBarSeparator"/>.</summary>
        public CommandBarSeparator()
        {
            DefaultStyleKey = typeof(CommandBarSeparator);
        }
    }
}
