using System.Windows;
using System.Windows.Controls;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>The severity level that controls the colour of a <see cref="StatusBadge"/>.</summary>
    public enum StatusBadgeSeverity
    {
        /// <summary>Neutral state, rendered in grey.</summary>
        Default,
        /// <summary>Informational state, rendered in blue.</summary>
        Information,
        /// <summary>Positive or completed state, rendered in green.</summary>
        Success,
        /// <summary>Advisory or pending state, rendered in amber.</summary>
        Warning,
        /// <summary>Failed or cancelled state, rendered in red.</summary>
        Error
    }

    /// <summary>
    /// A compact pill-shaped tag for displaying order and shipment states at a glance.
    /// </summary>
    /// <remarks>
    /// Set <see cref="Severity"/> to control the colour theme.  The <see cref="ContentControl.Content"/>
    /// property accepts any string or inline element as the badge label.
    /// </remarks>
    public class StatusBadge : ContentControl
    {
        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Severity"/> dependency property.</summary>
        public static readonly DependencyProperty SeverityProperty =
            DependencyProperty.Register("Severity", typeof(StatusBadgeSeverity), typeof(StatusBadge),
                new PropertyMetadata(StatusBadgeSeverity.Default));

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="StatusBadge"/>.</summary>
        public StatusBadge()
        {
            DefaultStyleKey = typeof(StatusBadge);
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the severity level that determines the badge colour.</summary>
        public StatusBadgeSeverity Severity
        {
            get { return (StatusBadgeSeverity)GetValue(SeverityProperty); }
            set { SetValue(SeverityProperty, value); }
        }
    }
}
