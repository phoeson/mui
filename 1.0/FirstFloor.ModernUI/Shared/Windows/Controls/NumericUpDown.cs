using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A numeric input control with increment and decrement buttons.
    /// Designed for quantity and measurement entry in warehouse and logistics
    /// workflows where accurate numeric input is critical.
    /// </summary>
    /// <remarks>
    /// Template parts:
    /// <list type="bullet">
    ///   <item><see cref="PartTextBox"/> — the editable text area.</item>
    ///   <item><see cref="PartIncreaseButton"/> — increments <see cref="Value"/> by <see cref="Step"/>.</item>
    ///   <item><see cref="PartDecreaseButton"/> — decrements <see cref="Value"/> by <see cref="Step"/>.</item>
    /// </list>
    /// The up/down arrow keys and mouse wheel also increment and decrement the value.
    /// Committing the typed value occurs on <c>Enter</c> or when the control loses focus.
    /// </remarks>
    [TemplatePart(Name = PartTextBox,        Type = typeof(TextBox))]
    [TemplatePart(Name = PartIncreaseButton, Type = typeof(RepeatButton))]
    [TemplatePart(Name = PartDecreaseButton, Type = typeof(RepeatButton))]
    public class NumericUpDown : Control
    {
        private const string PartTextBox        = "PART_TextBox";
        private const string PartIncreaseButton = "PART_IncreaseButton";
        private const string PartDecreaseButton = "PART_DecreaseButton";

        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Value"/> dependency property.</summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(decimal),
                typeof(NumericUpDown),
                new FrameworkPropertyMetadata(
                    0m,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnValueChanged,
                    CoerceValue));

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum",
                typeof(decimal),
                typeof(NumericUpDown),
                new PropertyMetadata(decimal.MinValue, OnMinimumChanged));

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum",
                typeof(decimal),
                typeof(NumericUpDown),
                new PropertyMetadata(decimal.MaxValue, OnMaximumChanged));

        /// <summary>Identifies the <see cref="Step"/> dependency property.</summary>
        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(
                "Step",
                typeof(decimal),
                typeof(NumericUpDown),
                new PropertyMetadata(1m, null, CoerceStep));

        /// <summary>Identifies the <see cref="StringFormat"/> dependency property.</summary>
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register(
                "StringFormat",
                typeof(string),
                typeof(NumericUpDown),
                new PropertyMetadata("G", OnStringFormatChanged));

        // ── Routed Events ────────────────────────────────────────────────────

        /// <summary>Identifies the <see cref="ValueChanged"/> routed event.</summary>
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                "ValueChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<decimal>),
                typeof(NumericUpDown));

        // ── Private fields ───────────────────────────────────────────────────

        private TextBox      textBox;
        private RepeatButton increaseButton;
        private RepeatButton decreaseButton;

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="NumericUpDown"/>.</summary>
        public NumericUpDown()
        {
            DefaultStyleKey = typeof(NumericUpDown);
        }

        // ── Template ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Detach old handlers
            if (textBox != null)
            {
                textBox.LostFocus      -= OnTextBoxLostFocus;
                textBox.PreviewKeyDown -= OnTextBoxPreviewKeyDown;
                DataObject.RemovePastingHandler(textBox, OnPasteFilter);
            }
            if (increaseButton != null) increaseButton.Click -= OnIncreaseClick;
            if (decreaseButton != null) decreaseButton.Click -= OnDecreaseClick;

            // Get new parts
            textBox        = GetTemplateChild(PartTextBox)        as TextBox;
            increaseButton = GetTemplateChild(PartIncreaseButton) as RepeatButton;
            decreaseButton = GetTemplateChild(PartDecreaseButton) as RepeatButton;

            // Attach new handlers
            if (textBox != null)
            {
                textBox.LostFocus      += OnTextBoxLostFocus;
                textBox.PreviewKeyDown += OnTextBoxPreviewKeyDown;
                DataObject.AddPastingHandler(textBox, OnPasteFilter);
                SyncTextFromValue();
            }
            if (increaseButton != null) increaseButton.Click += OnIncreaseClick;
            if (decreaseButton != null) decreaseButton.Click += OnDecreaseClick;

            RefreshButtonStates();
        }

        // ── Mouse wheel ──────────────────────────────────────────────────────

        /// <inheritdoc/>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);
            if (IsFocused || IsKeyboardFocusWithin)
            {
                if (e.Delta > 0) Increment();
                else             Decrement();
                e.Handled = true;
            }
        }

        // ── Event handlers ───────────────────────────────────────────────────

        private void OnIncreaseClick(object sender, RoutedEventArgs e) => Increment();
        private void OnDecreaseClick(object sender, RoutedEventArgs e) => Decrement();

        private void OnTextBoxPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                Increment();
                e.Handled = true;
            }
            else if (e.Key == Key.Down)
            {
                Decrement();
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                CommitTextBoxValue();
                SyncTextFromValue();
                e.Handled = true;
            }
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            CommitTextBoxValue();
            // Reformat to canonical string (e.g. "5." → "5")
            SyncTextFromValue();
        }

        private void OnPasteFilter(object sender, DataObjectPastingEventArgs e)
        {
            var text = e.DataObject.GetDataPresent(typeof(string))
                ? e.DataObject.GetData(typeof(string)) as string
                : null;
            decimal dummy;
            if (text == null || !decimal.TryParse(text, out dummy))
                e.CancelCommand();
        }

        // ── Value helpers ─────────────────────────────────────────────────────

        private void Increment()
        {
            Value = (decimal)CoerceValue(this, Value + Step);
        }

        private void Decrement()
        {
            Value = (decimal)CoerceValue(this, Value - Step);
        }

        private void CommitTextBoxValue()
        {
            if (textBox == null) return;
            decimal parsed;
            if (decimal.TryParse(textBox.Text, out parsed))
            {
                Value = (decimal)CoerceValue(this, parsed);
            }
        }

        private void SyncTextFromValue()
        {
            if (textBox == null) return;
            textBox.Text = Value.ToString(StringFormat ?? "G");
        }

        private void RefreshButtonStates()
        {
            if (increaseButton != null)
                increaseButton.IsEnabled = IsEnabled && Value < Maximum;
            if (decreaseButton != null)
                decreaseButton.IsEnabled = IsEnabled && Value > Minimum;
        }

        // ── DP callbacks ──────────────────────────────────────────────────────

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (NumericUpDown)d;
            ctrl.SyncTextFromValue();
            ctrl.RefreshButtonStates();
            ctrl.RaiseEvent(new RoutedPropertyChangedEventArgs<decimal>(
                (decimal)e.OldValue, (decimal)e.NewValue, ValueChangedEvent));
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var ctrl = (NumericUpDown)d;
            var val  = (decimal)baseValue;
            if (val < ctrl.Minimum) return ctrl.Minimum;
            if (val > ctrl.Maximum) return ctrl.Maximum;
            return val;
        }

        private static object CoerceStep(DependencyObject d, object baseValue)
        {
            var step = (decimal)baseValue;
            return step > 0m ? step : 1m;
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (NumericUpDown)d;
            ctrl.CoerceValue(ValueProperty);
            ctrl.RefreshButtonStates();
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (NumericUpDown)d;
            ctrl.CoerceValue(ValueProperty);
            ctrl.RefreshButtonStates();
        }

        private static void OnStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumericUpDown)d).SyncTextFromValue();
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the current numeric value.</summary>
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>Gets or sets the inclusive lower bound. Defaults to <see cref="decimal.MinValue"/> (no limit).</summary>
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>Gets or sets the inclusive upper bound. Defaults to <see cref="decimal.MaxValue"/> (no limit).</summary>
        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>Gets or sets the amount added or subtracted per click or key press. Defaults to 1.</summary>
        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        /// <summary>
        /// Gets or sets the format string used to display <see cref="Value"/>.
        /// Defaults to <c>"G"</c> (general format).  Use <c>"0"</c> to force an
        /// integer display, <c>"0.00"</c> for two decimal places, etc.
        /// </summary>
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        /// <summary>Raised when <see cref="Value"/> changes.</summary>
        public event RoutedPropertyChangedEventHandler<decimal> ValueChanged
        {
            add    { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
    }
}
