using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// Describes the visual position of a <see cref="ButtonGroupItem"/> or <see cref="SegmentedItem"/>
    /// within its parent container, used to set the appropriate corner radii.
    /// </summary>
    public enum ButtonGroupPosition
    {
        /// <summary>Only item in the group — all corners rounded.</summary>
        Only,
        /// <summary>First item — left corners rounded.</summary>
        First,
        /// <summary>Middle item — no rounded corners.</summary>
        Middle,
        /// <summary>Last item — right corners rounded.</summary>
        Last,
    }

    /// <summary>
    /// A horizontal strip of action buttons that share a continuous border.
    /// Each child becomes a <see cref="ButtonGroupItem"/> with automatic corner-radius
    /// assignment based on its position in the group.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;mui:ButtonGroup&gt;
    ///     &lt;mui:ButtonGroupItem Content="Receive" Command="{Binding ReceiveCmd}"/&gt;
    ///     &lt;mui:ButtonGroupItem Content="Ship"    Command="{Binding ShipCmd}"/&gt;
    ///     &lt;mui:ButtonGroupItem Content="Return"  Command="{Binding ReturnCmd}"/&gt;
    /// &lt;/mui:ButtonGroup&gt;
    /// </code>
    /// </example>
    public class ButtonGroup : ItemsControl
    {
        /// <summary>Initializes a new instance of <see cref="ButtonGroup"/>.</summary>
        public ButtonGroup()
        {
            DefaultStyleKey = typeof(ButtonGroup);
            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ButtonGroupItem();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ButtonGroupItem;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            // Schedule after all containers for this pass are prepared.
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
                var container = ItemContainerGenerator.ContainerFromIndex(i) as ButtonGroupItem;
                if (container == null) continue;

                ButtonGroupPosition pos;
                if (count == 1)       pos = ButtonGroupPosition.Only;
                else if (i == 0)      pos = ButtonGroupPosition.First;
                else if (i == count - 1) pos = ButtonGroupPosition.Last;
                else                  pos = ButtonGroupPosition.Middle;

                container.Position = pos;
            }
        }
    }
}
