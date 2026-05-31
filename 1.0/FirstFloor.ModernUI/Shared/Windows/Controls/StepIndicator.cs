using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>The completion state of a step within a <see cref="StepIndicator"/>.</summary>
    public enum StepStatus
    {
        /// <summary>Step has not been reached yet.</summary>
        Pending,
        /// <summary>Step is the currently active stage.</summary>
        Active,
        /// <summary>Step has been completed.</summary>
        Completed
    }

    /// <summary>
    /// An individual step item rendered by a <see cref="StepIndicator"/>.
    /// </summary>
    /// <remarks>
    /// Properties such as <see cref="StepNumber"/>, <see cref="IsFirst"/>, <see cref="IsLast"/>,
    /// and <see cref="Status"/> are set automatically by the parent <see cref="StepIndicator"/>.
    /// You may also set them manually when using <see cref="StepIndicatorItem"/> outside the control.
    /// </remarks>
    public class StepIndicatorItem : ContentControl
    {
        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Title"/> dependency property.</summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(StepIndicatorItem),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Description"/> dependency property.</summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(StepIndicatorItem),
                new PropertyMetadata(null));

        /// <summary>Identifies the <see cref="Status"/> dependency property.</summary>
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register("Status", typeof(StepStatus), typeof(StepIndicatorItem),
                new PropertyMetadata(StepStatus.Pending));

        /// <summary>Identifies the <see cref="StepNumber"/> dependency property.</summary>
        public static readonly DependencyProperty StepNumberProperty =
            DependencyProperty.Register("StepNumber", typeof(int), typeof(StepIndicatorItem),
                new PropertyMetadata(1));

        /// <summary>Identifies the <see cref="IsFirst"/> dependency property.</summary>
        public static readonly DependencyProperty IsFirstProperty =
            DependencyProperty.Register("IsFirst", typeof(bool), typeof(StepIndicatorItem),
                new PropertyMetadata(false));

        /// <summary>Identifies the <see cref="IsLast"/> dependency property.</summary>
        public static readonly DependencyProperty IsLastProperty =
            DependencyProperty.Register("IsLast", typeof(bool), typeof(StepIndicatorItem),
                new PropertyMetadata(false));

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="StepIndicatorItem"/>.</summary>
        public StepIndicatorItem()
        {
            DefaultStyleKey = typeof(StepIndicatorItem);
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the title text displayed below the step circle.</summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>Gets or sets an optional description shown beneath the title.</summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>Gets or sets the completion state of this step.</summary>
        public StepStatus Status
        {
            get { return (StepStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        /// <summary>Gets or sets the 1-based ordinal assigned by the parent <see cref="StepIndicator"/>.</summary>
        public int StepNumber
        {
            get { return (int)GetValue(StepNumberProperty); }
            set { SetValue(StepNumberProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this is the first step, suppressing the left connector line.</summary>
        public bool IsFirst
        {
            get { return (bool)GetValue(IsFirstProperty); }
            set { SetValue(IsFirstProperty, value); }
        }

        /// <summary>Gets or sets a value indicating whether this is the last step, suppressing the right connector line.</summary>
        public bool IsLast
        {
            get { return (bool)GetValue(IsLastProperty); }
            set { SetValue(IsLastProperty, value); }
        }
    }

    /// <summary>
    /// A horizontal progress tracker that visualises a multi-stage workflow.
    /// </summary>
    /// <remarks>
    /// Populate with <see cref="StepIndicatorItem"/> children, or bind
    /// <see cref="ItemsControl.ItemsSource"/> to a collection of strings used as step titles.
    /// Set <see cref="CurrentStep"/> (zero-based) to advance or retreat the active step.
    /// Items before <see cref="CurrentStep"/> are marked <see cref="StepStatus.Completed"/>;
    /// the item at <see cref="CurrentStep"/> is marked <see cref="StepStatus.Active"/>;
    /// items after are <see cref="StepStatus.Pending"/>.
    /// </remarks>
    public class StepIndicator : ItemsControl
    {
        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="CurrentStep"/> dependency property.</summary>
        public static readonly DependencyProperty CurrentStepProperty =
            DependencyProperty.Register("CurrentStep", typeof(int), typeof(StepIndicator),
                new PropertyMetadata(0, OnCurrentStepChanged, CoerceCurrentStep));

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="StepIndicator"/>.</summary>
        public StepIndicator()
        {
            DefaultStyleKey = typeof(StepIndicator);
            ItemContainerGenerator.StatusChanged += OnGeneratorStatusChanged;
        }

        // ── Overrides ────────────────────────────────────────────────────────

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new StepIndicatorItem();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is StepIndicatorItem;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            var step = element as StepIndicatorItem;
            if (step != null && item is string && string.IsNullOrEmpty(step.Title))
                step.Title = (string)item;

            Dispatcher.BeginInvoke(
                new Action(UpdateAllSteps),
                System.Windows.Threading.DispatcherPriority.Loaded);
        }

        // ── DP callbacks / event handlers ────────────────────────────────────

        private void OnGeneratorStatusChanged(object sender, EventArgs e)
        {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                UpdateAllSteps();
        }

        private static void OnCurrentStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StepIndicator)d).UpdateAllSteps();
        }

        private static object CoerceCurrentStep(DependencyObject d, object baseValue)
        {
            var ctrl = (StepIndicator)d;
            var step = (int)baseValue;
            if (step < 0) return 0;
            int max = ctrl.Items.Count - 1;
            // Don't clamp before items are loaded (max == -1)
            return max >= 0 && step > max ? max : step;
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void UpdateAllSteps()
        {
            int count   = Items.Count;
            int current = CurrentStep;

            for (int i = 0; i < count; i++)
            {
                var container = ItemContainerGenerator.ContainerFromIndex(i) as StepIndicatorItem;
                if (container == null) continue;

                container.StepNumber = i + 1;
                container.IsFirst    = (i == 0);
                container.IsLast     = (i == count - 1);

                if      (i < current)  container.Status = StepStatus.Completed;
                else if (i == current) container.Status = StepStatus.Active;
                else                   container.Status = StepStatus.Pending;
            }
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the zero-based index of the currently active step.</summary>
        public int CurrentStep
        {
            get { return (int)GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }
    }
}
