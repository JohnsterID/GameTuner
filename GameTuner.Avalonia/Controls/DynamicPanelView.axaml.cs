using Avalonia.Controls;
using Avalonia.Media;
using GameTuner.Core;
using System.Collections.Generic;

namespace GameTuner.Avalonia.Controls
{
    public partial class DynamicPanelView : UserControl
    {
        private CustomUI? _customUI;
        private readonly List<Control> _dynamicControls = new();

        public DynamicPanelView()
        {
            InitializeComponent();
        }

        public void LoadPanel(CustomUI customUI)
        {
            _customUI = customUI;
            ClearPanel();
            CreateControls();
        }

        private void ClearPanel()
        {
            PanelCanvas.Children.Clear();
            _dynamicControls.Clear();
        }

        private void CreateControls()
        {
            if (_customUI == null) return;

            // Create action buttons
            foreach (var actionButton in _customUI.GetActionButtons())
            {
                var button = new Button
                {
                    Content = actionButton.Text,
                    Width = actionButton.Width,
                    Height = actionButton.Height,
                    IsEnabled = actionButton.Enabled,
                    Background = new SolidColorBrush(Colors.DarkGray),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                
                if (!string.IsNullOrEmpty(actionButton.ToolTip))
                {
                    ToolTip.SetTip(button, actionButton.ToolTip);
                }

                button.Click += (s, e) => actionButton.Execute();

                Canvas.SetLeft(button, actionButton.X);
                Canvas.SetTop(button, actionButton.Y);

                PanelCanvas.Children.Add(button);
                _dynamicControls.Add(button);
            }

            // Create value controls
            foreach (var valueControl in _customUI.GetValueControls())
            {
                Control control = valueControl.Type switch
                {
                    ValueControlType.Label => CreateLabel(valueControl),
                    ValueControlType.TextBox => CreateTextBox(valueControl),
                    ValueControlType.CheckBox => CreateCheckBox(valueControl),
                    ValueControlType.ComboBox => CreateComboBox(valueControl),
                    ValueControlType.NumericUpDown => CreateNumericUpDown(valueControl),
                    _ => CreateLabel(valueControl)
                };

                Canvas.SetLeft(control, valueControl.X);
                Canvas.SetTop(control, valueControl.Y);

                PanelCanvas.Children.Add(control);
                _dynamicControls.Add(control);
            }
        }

        private Control CreateLabel(ValueControl valueControl)
        {
            var textBlock = new TextBlock
            {
                Text = valueControl.Value,
                Width = valueControl.Width,
                Height = valueControl.Height,
                Foreground = new SolidColorBrush(Colors.White)
            };
            
            if (!string.IsNullOrEmpty(valueControl.ToolTip))
            {
                ToolTip.SetTip(textBlock, valueControl.ToolTip);
            }
            
            return textBlock;
        }

        private Control CreateTextBox(ValueControl valueControl)
        {
            var textBox = new TextBox
            {
                Text = valueControl.Value,
                Width = valueControl.Width,
                Height = valueControl.Height,
                IsReadOnly = valueControl.ReadOnly,
                Background = new SolidColorBrush(Colors.Black),
                Foreground = new SolidColorBrush(Colors.White)
            };

            if (!string.IsNullOrEmpty(valueControl.ToolTip))
            {
                ToolTip.SetTip(textBox, valueControl.ToolTip);
            }

            if (!valueControl.ReadOnly)
            {
                textBox.LostFocus += (s, e) =>
                {
                    if (textBox.Text != valueControl.Value)
                    {
                        valueControl.UpdateValue(textBox.Text ?? string.Empty);
                    }
                };
            }

            return textBox;
        }

        private Control CreateCheckBox(ValueControl valueControl)
        {
            var checkBox = new CheckBox
            {
                Content = valueControl.Label,
                IsChecked = bool.TryParse(valueControl.Value, out bool isChecked) && isChecked,
                Width = valueControl.Width,
                Height = valueControl.Height,
                Foreground = new SolidColorBrush(Colors.White)
            };

            if (!string.IsNullOrEmpty(valueControl.ToolTip))
            {
                ToolTip.SetTip(checkBox, valueControl.ToolTip);
            }

            checkBox.IsCheckedChanged += (s, e) => 
            {
                valueControl.UpdateValue(checkBox.IsChecked == true ? "true" : "false");
            };

            return checkBox;
        }

        private Control CreateComboBox(ValueControl valueControl)
        {
            var comboBox = new ComboBox
            {
                Width = valueControl.Width,
                Height = valueControl.Height,
                Background = new SolidColorBrush(Colors.Black),
                Foreground = new SolidColorBrush(Colors.White)
                // TODO: Populate items based on valueControl configuration
            };
            
            if (!string.IsNullOrEmpty(valueControl.ToolTip))
            {
                ToolTip.SetTip(comboBox, valueControl.ToolTip);
            }
            
            return comboBox;
        }

        private Control CreateNumericUpDown(ValueControl valueControl)
        {
            var numericUpDown = new NumericUpDown
            {
                Value = decimal.TryParse(valueControl.Value, out decimal value) ? value : 0,
                Width = valueControl.Width,
                Height = valueControl.Height,
                Background = new SolidColorBrush(Colors.Black),
                Foreground = new SolidColorBrush(Colors.White)
            };

            if (!string.IsNullOrEmpty(valueControl.ToolTip))
            {
                ToolTip.SetTip(numericUpDown, valueControl.ToolTip);
            }

            numericUpDown.ValueChanged += (s, e) =>
            {
                if (numericUpDown.Value.HasValue)
                {
                    valueControl.UpdateValue(numericUpDown.Value.Value.ToString());
                }
            };

            return numericUpDown;
        }
    }
}