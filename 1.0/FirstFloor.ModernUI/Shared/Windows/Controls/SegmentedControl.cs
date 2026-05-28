using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A mutually-exclusive selection strip where exactly one item is active at a time.
    /// Items render as a continuous pill divided by thin lines; the selected item is
    /// highlighted with the accent colour.  Ideal for view-mode toggles such as
    /// List / Grid / Map, or filter states like All / Pending / Completed.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;mui:SegmentedControl SelectedIndex="0"&gt;
    ///     &lt;mui:SegmentedItem Content="All"/&gt;
    ///     &lt;mui:SegmentedItem Content="Pending"/&gt;
    ///     &lt;mui:SegmentedItem Content="Completed"/&gt;
    /// &lt;/mui:SegmentedControl&gt;
    /// </code>
    /// </example>
    public class SegmentedControl : ListBox
    {
        /// <summary>Initializes a new instance of <see cref="SegmentedControl"/>.</summary>
        public SegmentedControl()
        {
            DefaultStyleKey = typeof(SegmentedControl);
            SelectionMode = SelectionMode.Single;
            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SegmentedItem();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SegmentedItem;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            Dispatcher.BeginInvoke(
                new Action(UpdatePositions),
                System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                UpdatePositions();
            }
        }

        private void UpdatePositions()
        {
            int count = Items.Count;
            for (int i = 0; i < count; i++)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(i) as SegmentedItem;
                if (container == null) continue;

                ButtonGroupPosition pos;
                if (count == 1)          pos = ButtonGroupPosition.Only;
                else if (i == 0)         pos = ButtonGroupPosition.First;
                else if (i == count - 1) pos = ButtonGroupPosition.Last;
                else                     pos = ButtonGroupPosition.Middle;

                container.Position = pos;
            }
        }
    }

    /// <summary>
    /// A selectable item inside a <see cref="SegmentedControl"/>.
    /// The <see cref="Position"/> property is managed by the parent control.
    /// </summary>
    public class SegmentedItem : ListBoxItem
    {
        /// <summary>Identifies the <see cref="Position"/> dependency property.</summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(
                "Position",
                typeof(ButtonGroupPosition),
                typeof(SegmentedItem),
                new PropertyMetadata(ButtonGroupPosition.Only));

        /// <summary>Initializes a new instance of <see cref="SegmentedItem"/>.</summary>
        public SegmentedItem()
        {
            DefaultStyleKey = typeof(SegmentedItem);
        }

        /// <summary>
        /// Gets or sets the visual position of this item within the parent control.
        /// Set automatically by <see cref="SegmentedControl"/>; rarely set manually.
        /// </summary>
        public ButtonGroupPosition Position
        {
            get { return (ButtonGroupPosition)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
    }
}
