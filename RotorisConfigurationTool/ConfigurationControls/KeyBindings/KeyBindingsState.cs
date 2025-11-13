
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;
using RotorisLib.UI;
using RotorisLib;
using System.Diagnostics;

namespace RotorisConfigurationTool.ConfigurationControls.KeyBindings
{
    public enum TriggerKeys
    {
        None,
        PrimaryKey,
        ClockwiseKey,
        CounterclockwiseKey
    }
    public class KeyBindingsState : DependencyObject
    {
        public ICommand SaveCommand { get; }
        public ICommand SetDefaultCommand { get; }
        public ICommand EnableKeyboardCaptureCommand { get; }
        public ICommand DisableKeyboardCaptureCommand { get; }

        private readonly SettingsManager settings;
        private readonly GlobalInputHook inputHook;
        private EventHandler<GlobalInputHook.InputHookEventArgs>? inputHookHandler;
        public KeyBindingsState(SettingsManager s, GlobalInputHook hook)
        {
            SaveCommand = new RelayCommand(ExecuteSave);
            SetDefaultCommand = new RelayCommand(ExecuteSetDefault);
            EnableKeyboardCaptureCommand = new RelayCommand(ExecuteEnableKeyboardCapture);
            DisableKeyboardCaptureCommand = new RelayCommand(ExecuteDisableKeyboardCapture);

            settings = s;
            inputHook = hook;

            var configuration = settings.CurrentConfig;
            PrimaryKey = configuration.PrimaryKey;
            ClockwiseKey = configuration.ClockwiseKey;
            CounterclockwiseKey = configuration.CounterclockwiseKey;
        }
        private void ExecuteSave()
        {
            var configuration = new Configuration()
            {
                PrimaryKey = PrimaryKey,
                ClockwiseKey = ClockwiseKey,
                CounterclockwiseKey = CounterclockwiseKey
            };
            settings.UpdateKeyBindings(configuration);
            settings.SaveSettings();
        }
        private void ExecuteSetDefault()
        {
            PrimaryKey = Configuration.Default.PrimaryKey;
            ClockwiseKey = Configuration.Default.ClockwiseKey;
            CounterclockwiseKey = Configuration.Default.CounterclockwiseKey;
        }
        private void ExecuteEnableKeyboardCapture(object? parameter)
        {
            ClearKeyCapture();

            if (parameter is TextBox textBox && textBox.Tag is TriggerKeys triggerKey)
            {
                inputHookHandler = (sender, e) =>
                {
                    switch (e.VirtualKeyCode)
                    {
                        case (int)VirtualKeys.Keyboard.LeftControl:
                        case (int)VirtualKeys.Keyboard.RightControl:
                        case (int)VirtualKeys.Keyboard.LeftShift:
                        case (int)VirtualKeys.Keyboard.RightShift:
                        case (int)VirtualKeys.Keyboard.LeftWindows:
                        case (int)VirtualKeys.Keyboard.RightWindows:
                            return;
                        case (int)VirtualKeys.Mouse.Left:
                        case (int)VirtualKeys.Mouse.Right:
                        case (int)VirtualKeys.Mouse.Middle:
                        case (int)VirtualKeys.Wheel.WheelUp:
                        case (int)VirtualKeys.Wheel.WheelDown:
                        case (int)VirtualKeys.Keyboard.OemAttn:
                        case (int)VirtualKeys.Keyboard.OemFinish:
                            if (e.Key.GetModifiers() == Hotkey.HotkeyModifiers.None)
                            {
                                return;
                            }
                            break;
                    }

                    e.ShouldBlock();

                    textBox.Dispatcher.Invoke(() =>
                    {
                        if (!TrySetHotkey(triggerKey, e.Key))
                        {
                            ClearKeyCapture();
                        }
                    });


                };

                inputHook.KeyDown += inputHookHandler;
                EditingKey = triggerKey;
            }
        }

        private void ExecuteDisableKeyboardCapture()
        {
            ClearKeyCapture();
        }


        public static readonly DependencyProperty EditingKeyProperty =
            DependencyProperty.Register(
                nameof(EditingKey),
                typeof(TriggerKeys),
                typeof(KeyBindingsState),
                new PropertyMetadata(TriggerKeys.None)
                );

        public TriggerKeys EditingKey
        {
            get => (TriggerKeys)GetValue(EditingKeyProperty);
            set => SetValue(EditingKeyProperty, value);
        }

        public static readonly DependencyProperty PrimaryKeyProperty =
            DependencyProperty.Register(
                nameof(PrimaryKey),
                typeof(Hotkey),
                typeof(KeyBindingsState),
                new PropertyMetadata(new Hotkey())
                );

        public Hotkey PrimaryKey
        {
            get => (Hotkey)GetValue(PrimaryKeyProperty);
            set => SetValue(PrimaryKeyProperty, value);
        }

        public static readonly DependencyProperty ClockwiseKeyProperty =
            DependencyProperty.Register(
                nameof(ClockwiseKey),
                typeof(Hotkey),
                typeof(KeyBindingsState),
                new PropertyMetadata(new Hotkey())
                );

        public Hotkey ClockwiseKey
        {
            get => (Hotkey)GetValue(ClockwiseKeyProperty);
            set => SetValue(ClockwiseKeyProperty, value);
        }

        public static readonly DependencyProperty CounterclockwiseKeyProperty =
            DependencyProperty.Register(
                nameof(CounterclockwiseKey),
                typeof(Hotkey),
                typeof(KeyBindingsState),
                new PropertyMetadata(new Hotkey())
                );

        public Hotkey CounterclockwiseKey
        {
            get => (Hotkey)GetValue(CounterclockwiseKeyProperty);
            set => SetValue(CounterclockwiseKeyProperty, value);
        }

        private void ClearKeyCapture()
        {
            EditingKey = TriggerKeys.None;
            if (inputHookHandler != null)
            {
                inputHook.KeyDown -= inputHookHandler;
                inputHookHandler = null;
            }
        }

        private bool TrySetHotkey(TriggerKeys triggerKey, Hotkey hotkey)
        {
            string hotkeyString = hotkey.ToString();
            string primaryKeyString = PrimaryKey.ToString();
            string clockwiseKeyString = ClockwiseKey.ToString();
            string counterclockwiseKeyString = CounterclockwiseKey.ToString();
            switch (triggerKey)
            {
                case TriggerKeys.PrimaryKey:
                    if (hotkeyString == clockwiseKeyString || hotkeyString == counterclockwiseKeyString)
                    {
                        return true;
                    }
                    PrimaryKey = new Hotkey(hotkey);
                    break;
                case TriggerKeys.ClockwiseKey:
                    if (hotkeyString == primaryKeyString || hotkeyString == counterclockwiseKeyString)
                    {
                        return true;
                    }
                    ClockwiseKey = new Hotkey(hotkey);
                    break;
                case TriggerKeys.CounterclockwiseKey:
                    if (hotkeyString == primaryKeyString || hotkeyString == clockwiseKeyString)
                    {
                        return true;
                    }
                    CounterclockwiseKey = new Hotkey(hotkey);
                    break;
            }

            return false;
        }
    }
    public class KeyBindingsStateConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is MainContext ctx && values[1] is bool isTabSelected && isTabSelected)
            {
                return new KeyBindingsState(ctx.Settings, ctx.InputHook);
            }

            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class HotkeyTextConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Hotkey hotkey)
            {
                return hotkey.ToString("n");
            }

            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TrunoffKeyboardCapture : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is bool IsActive && !IsActive && values[1] is KeyBindingsState ctx && ctx.DisableKeyboardCaptureCommand.CanExecute(IsActive))
            {
                ctx.DisableKeyboardCaptureCommand.Execute(IsActive);
            }

            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EditingKeyBorderBrushConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is TextBox textBox && textBox.Tag is TriggerKeys triggerKey && values[1] is TriggerKeys editingKey && triggerKey == editingKey)
            {
                return new SolidColorBrush(Colors.LightGreen);
            }

            return new SolidColorBrush(Colors.Transparent);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
