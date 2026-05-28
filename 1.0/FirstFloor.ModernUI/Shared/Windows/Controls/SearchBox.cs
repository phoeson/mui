using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FirstFloor.ModernUI.Windows.Controls
{
    /// <summary>
    /// A search input control with an integrated search icon, watermark, and
    /// a clear (✕) button that appears when text is present.
    /// </summary>
    /// <remarks>
    /// Template parts:
    /// <list type="bullet">
    ///   <item><see cref="PartTextBox"/> — the editable text area.</item>
    ///   <item><see cref="PartClearButton"/> — clears the text when clicked.</item>
    /// </list>
    /// The <c>Enter</c> key executes <see cref="SearchCommand"/>;
    /// the <c>Escape</c> key clears the text.
    /// </remarks>
    [TemplatePart(Name = PartTextBox,     Type = typeof(TextBox))]
    [TemplatePart(Name = PartClearButton, Type = typeof(Button))]
    public class SearchBox : Control
    {
        private const string PartTextBox     = "PART_TextBox";
        private const string PartClearButton = "PART_ClearButton";

        // ── Dependency Properties ────────────────────────────────────────────

        /// <summary>Identifies the <see cref="Text"/> dependency property.</summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(SearchBox),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnTextChanged));

        /// <summary>Identifies the <see cref="Watermark"/> dependency property.</summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(
                "Watermark",
                typeof(string),
                typeof(SearchBox),
                new PropertyMetadata("Search..."));

        /// <summary>Identifies the <see cref="SearchCommand"/> dependency property.</summary>
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register(
                "SearchCommand",
                typeof(ICommand),
                typeof(SearchBox));

        /// <summary>Identifies the <see cref="SearchCommandParameter"/> dependency property.</summary>
        public static readonly DependencyProperty SearchCommandParameterProperty =
            DependencyProperty.Register(
                "SearchCommandParameter",
                typeof(object),
                typeof(SearchBox));

        // ── Routed Events ────────────────────────────────────────────────────

        /// <summary>Identifies the <see cref="TextChanged"/> routed event.</summary>
        public static readonly RoutedEvent TextChangedEvent =
            EventManager.RegisterRoutedEvent(
                "TextChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<string>),
                typeof(SearchBox));

        // ── Private fields ───────────────────────────────────────────────────

        private TextBox textBox;
        private Button  clearButton;
        private bool    isUpdatingText;

        // ── Constructor ──────────────────────────────────────────────────────

        /// <summary>Initializes a new instance of <see cref="SearchBox"/>.</summary>
        public SearchBox()
        {
            DefaultStyleKey = typeof(SearchBox);
        }

        // ── Template ─────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Detach old handlers
            if (textBox != null)
            {
                textBox.TextChanged -= OnTextBoxTextChanged;
                textBox.KeyDown     -= OnTextBoxKeyDown;
            }
            if (clearButton != null)
            {
                clearButton.Click -= OnClearButtonClick;
            }

            // Get new parts
            textBox     = GetTemplateChild(PartTextBox)     as TextBox;
            clearButton = GetTemplateChild(PartClearButton) as Button;

            // Attach new handlers
            if (textBox != null)
            {
                textBox.TextChanged += OnTextBoxTextChanged;
                textBox.KeyDown     += OnTextBoxKeyDown;
                SyncTextBoxFromText();
            }
            if (clearButton != null)
            {
                clearButton.Click += OnClearButtonClick;
            }
        }

        // ── Event handlers ───────────────────────────────────────────────────

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (isUpdatingText) return;

            var oldText = Text;
            var newText = textBox.Text;

            // Push the new text to the DP without triggering SyncTextBoxFromText
            isUpdatingText = true;
            SetCurrentValue(TextProperty, newText);
            isUpdatingText = false;

            RaiseEvent(new RoutedPropertyChangedEventArgs<string>(oldText, newText, TextChangedEvent));
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteSearch();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                SetCurrentValue(TextProperty, string.Empty);
                e.Handled = true;
            }
        }

        private void OnClearButtonClick(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(TextProperty, string.Empty);
            textBox?.Focus();
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private void ExecuteSearch()
        {
            var cmd   = SearchCommand;
            var param = SearchCommandParameter ?? Text;
            if (cmd != null && cmd.CanExecute(param))
            {
                cmd.Execute(param);
            }
        }

        private void SyncTextBoxFromText()
        {
            if (textBox == null) return;
            var target = Text ?? string.Empty;
            if (textBox.Text != target)
            {
                isUpdatingText = true;
                textBox.Text   = target;
                isUpdatingText = false;
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SearchBox)d).SyncTextBoxFromText();
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Gets or sets the current search text.  Supports two-way binding.</summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>Gets or sets the placeholder text shown when the box is empty and unfocused.</summary>
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        /// <summary>Gets or sets the command executed when the user presses <c>Enter</c>.</summary>
        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the parameter passed to <see cref="SearchCommand"/>.
        /// When <c>null</c>, the current <see cref="Text"/> value is used.
        /// </summary>
        public object SearchCommandParameter
        {
            get { return GetValue(SearchCommandParameterProperty); }
            set { SetValue(SearchCommandParameterProperty, value); }
        }

        /// <summary>Raised when <see cref="Text"/> changes.</summary>
        public event RoutedPropertyChangedEventHandler<string> TextChanged
        {
            add    { AddHandler(TextChangedEvent, value); }
            remove { RemoveHandler(TextChangedEvent, value); }
        }
    }
}
