using System.Windows;
using System.Windows.Controls.Primitives;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A clickable item inside a <see cref="ButtonGroup"/>.
    /// Supports <see cref="System.Windows.Input.ICommand"/> via the inherited
    /// <c>Command</c> / <c>CommandParameter</c> / <c>CommandTarget</c> properties.
    /// The <see cref="Position"/> property is set automatically by the parent
    /// <see cref="ButtonGroup"/> and drives the corner-radius style triggers.
    /// </summary>
    public class ButtonGroupItem : ButtonBase
    {
        /// <summary>Identifies the <see cref="Position"/> dependency property.</summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "Position",
                typeof(ButtonGroupPosition),
                typeof(ButtonGroupItem),
                new PropertyMetadata(ButtonGroupPosition.Only));

        /// <summary>Initializes a new instance of <see cref="ButtonGroupItem"/>.</summary>
        public ButtonGroupItem()
        {
            DefaultStyleKey = typeof(ButtonGroupItem);
        }

        /// <summary>
        /// Gets or sets the visual position of this item within the parent group.
        /// Set automatically by <see cref="ButtonGroup"/>; rarely set manually.
        /// </summary>
        public ButtonGroupPosition Position
        {
            get { return (ButtonGroupPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
    }
}
