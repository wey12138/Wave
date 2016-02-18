﻿using System.Collections.Generic;
using System.Timers;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    /// <summary>
    ///     A <see cref="TextBox" /> control that supports type-ahead (or auto complete).
    /// </summary>
    public class AutoCompleteTextBox : Control, IDisposable
    {
        #region Fields

        /// <summary>
        ///     The automatic complete source property
        /// </summary>
        public static readonly DependencyProperty AutoCompleteSourceProperty =
            DependencyProperty.Register("AutoCompleteSource", typeof (IEnumerable<string>), typeof (AutoCompleteTextBox), new FrameworkPropertyMetadata(OnAutoCompleteSourceChanged));

        /// <summary>
        ///     The delay time property
        /// </summary>
        public static readonly DependencyProperty DelayTimeProperty =
            DependencyProperty.Register("DelayTime", typeof (int), typeof (AutoCompleteTextBox), new UIPropertyMetadata(500, OnDelayTimeChanged));

        /// <summary>
        ///     The text property
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof (string), typeof (AutoCompleteTextBox), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        private readonly ComboBox _ComboBox;
        private readonly VisualCollection _Controls;
        private readonly Timer _KeypressTimer;
        private readonly TextBox _TextBox;

        private bool _SuppressTextChanged;
        private int _Threshold;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AutoCompleteTextBox" /> class.
        /// </summary>
        public AutoCompleteTextBox()
        {
            _Controls = new VisualCollection(this);
            _Threshold = 1;

            // Set up the key press timer
            _KeypressTimer = new Timer();
            _KeypressTimer.Elapsed += OnTimeElapsed;

            // Set up the text box and the combo box
            _ComboBox = new ComboBox();
            _ComboBox.IsSynchronizedWithCurrentItem = true;
            _ComboBox.IsTabStop = false;
            _ComboBox.MaxDropDownHeight = 150;
            _ComboBox.SelectionChanged += ComboBox_SelectionChanged;

            _TextBox = new TextBox();
            _TextBox.VerticalContentAlignment = VerticalAlignment.Center;
            _TextBox.TextChanged += TextBox_TextChanged;
            _TextBox.KeyDown += TextBox_OnKeyDown;

            _Controls.Add(_ComboBox);
            _Controls.Add(_TextBox);
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the automatic complete source.
        /// </summary>
        /// <value>
        ///     The automatic complete source.
        /// </value>
        public IEnumerable<string> AutoCompleteSource
        {
            get { return (IEnumerable<string>) this.GetValue(AutoCompleteSourceProperty); }
            set { this.SetValue(AutoCompleteSourceProperty, value); }
        }

        /// <summary>
        ///     Gets and Sets the amount of time (in miliseconds) to wait after the text has changed before updating the binding.
        /// </summary>
        public int DelayTime
        {
            get { return (int) this.GetValue(DelayTimeProperty); }
            set { this.SetValue(DelayTimeProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the maximum height of the drop down.
        /// </summary>
        /// <value>
        ///     The maximum height of the drop down.
        /// </value>
        public double MaxDropDownHeight
        {
            get { return _ComboBox.MaxDropDownHeight; }
            set { _ComboBox.MaxDropDownHeight = value; }
        }

        /// <summary>
        ///     Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text
        {
            get { return (string) this.GetValue(TextProperty); }
            set
            {
                this.SetValue(TextProperty, value);

                _SuppressTextChanged = true;
                _TextBox.Text = value;
                _SuppressTextChanged = false;
            }
        }

        /// <summary>
        ///     Gets or sets the minium number of characters before populating the drop down.
        /// </summary>
        /// <value>
        ///     The threshold.
        /// </value>
        public int Threshold
        {
            get { return _Threshold; }
            set { _Threshold = value; }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        ///     Gets the number of child <see cref="T:System.Windows.Media.Visual" /> objects in this instance of
        ///     <see cref="T:System.Windows.Controls.Panel" />.
        /// </summary>
        /// <returns>
        ///     The number of child <see cref="T:System.Windows.Media.Visual" /> objects.
        /// </returns>
        protected override int VisualChildrenCount
        {
            get { return _Controls.Count; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Arranges the content of a <see cref="T:System.Windows.Controls.Canvas" /> element.
        /// </summary>
        /// <param name="arrangeSize">
        ///     The size that this <see cref="T:System.Windows.Controls.Canvas" /> element should use to
        ///     arrange its child elements.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Windows.Size" /> that represents the arranged size of this
        ///     <see cref="T:System.Windows.Controls.Canvas" /> element and its descendants.
        /// </returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            _TextBox.Arrange(new Rect(arrangeSize));
            _ComboBox.Arrange(new Rect(arrangeSize));

            return base.ArrangeOverride(arrangeSize);
        }

        /// <summary>
        ///     Gets a <see cref="T:System.Windows.Media.Visual" /> child of this <see cref="T:System.Windows.Controls.Panel" /> at
        ///     the specified index position.
        /// </summary>
        /// <param name="index">The index position of the <see cref="T:System.Windows.Media.Visual" /> child.</param>
        /// <returns>
        ///     A <see cref="T:System.Windows.Media.Visual" /> child of the parent <see cref="T:System.Windows.Controls.Panel" />
        ///     element.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return _Controls[index];
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Handles the SelectionChanged event of the ComboBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_ComboBox.SelectedItem != null)
            {
                var item = (ComboBoxItem) _ComboBox.SelectedItem;
                this.Text = string.Format("{0}", item.Tag);
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="dissposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        private void Dispose(bool dissposing)
        {
            if (dissposing)
            {
                _KeypressTimer.Dispose();
            }
        }

        /// <summary>
        ///     Called when <see cref="AutoCompleteTextBox.AutoCompleteSource" /> dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnAutoCompleteSourceChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = dependencyObject as AutoCompleteTextBox;
            if (autoCompleteTextBox != null)
                autoCompleteTextBox.AutoCompleteSource = (IEnumerable<string>) e.NewValue;
        }

        /// <summary>
        ///     Called when <see cref="AutoCompleteTextBox.DelayTime" /> property changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">
        ///     The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event
        ///     data.
        /// </param>
        private static void OnDelayTimeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = dependencyObject as AutoCompleteTextBox;
            if (autoCompleteTextBox != null)
                autoCompleteTextBox.DelayTime = (int) e.NewValue;
        }

        /// <summary>
        ///     Called when <see cref="AutoCompleteTextBox.Text" /> dependency property changes.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void OnTextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            AutoCompleteTextBox autoCompleteTextBox = dependencyObject as AutoCompleteTextBox;
            if (autoCompleteTextBox != null)
                autoCompleteTextBox.Text = (string) e.NewValue;
        }

        /// <summary>
        ///     Called when the time has elapsed.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs" /> instance containing the event data.</param>
        private void OnTimeElapsed(object source, ElapsedEventArgs e)
        {
            _KeypressTimer.Stop();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action) this.ShowDropDown);
        }

        /// <summary>
        ///     Populates the combo box and shows the drop-down.
        /// </summary>
        private void ShowDropDown()
        {
            _ComboBox.Items.Clear();

            if (_TextBox.Text.Length >= _Threshold && this.AutoCompleteSource != null)
            {
                foreach (string source in this.AutoCompleteSource)
                {
                    if (source.StartsWith(_TextBox.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var textBlock = new TextBlock();
                        textBlock.Inlines.Add(new Run
                        {
                            Text = source.Substring(0, _TextBox.Text.Length),
                            FontWeight = FontWeights.Bold
                        });
                        textBlock.Inlines.Add(new Run
                        {
                            Text = source.Substring(_TextBox.Text.Length)
                        });

                        _ComboBox.Items.Add(new ComboBoxItem
                        {
                            Content = textBlock,
                            Tag = source
                        });
                    }
                }

                _ComboBox.IsDropDownOpen = _ComboBox.HasItems;
            }
            else
            {
                _ComboBox.IsDropDownOpen = false;
            }
        }

        /// <summary>
        ///     Handles the OnKeyDown event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs" /> instance containing the event data.</param>
        private void TextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return || e.Key == Key.Enter || e.Key == Key.Tab)
            {
                if (_ComboBox.IsDropDownOpen && _ComboBox.Items.Count == 1)
                    _ComboBox.SelectedIndex = 0;

                _ComboBox.IsDropDownOpen = false;
            }
        }

        /// <summary>
        ///     Handles the TextChanged event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_SuppressTextChanged)
            {
                if (this.DelayTime > 0)
                {
                    _KeypressTimer.Interval = this.DelayTime;
                    _KeypressTimer.Start();
                }
                else
                {
                    this.ShowDropDown();
                }
            }

            this.Text = _TextBox.Text;
        }

        #endregion
    }
}