using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.IO;
using RotorisLib.UI;
using RotorisLib;

namespace RotorisConfigurationTool.ConfigurationControls.ActionManagement
{

    public partial class Component
    {
        private void EditTextIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is TextBox textBox && e.NewValue is bool visible && visible)
            {
                textBox.Focus();
            }
        }
        private void EditTextKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && sender is TextBox textBox && textBox.DataContext is ActionItemState ctx)
            {
                ctx.UpdateActionName();
            }
        }

        private void EditTextLostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is ActionItemState ctx)
            {
                ctx.UpdateActionName();
            }
        }
    }
    internal class ActionItemState : DependencyObject
    {
        public ICommand EditActionNameCommand { get; }
        public ICommand ShowContextMenuCommand { get; }
        public ICommand RemoveActionCommand { get; }
        public ICommand UseNotepadCommand { get; }
        public ICommand UseVSCodeCommand { get; }
        public ICommand UseNotepadPlusPlusCommand { get; }

        private readonly SettingsManager settings;
        public ActionItemState(SettingsManager s, EditorAvailability availableEditors, string name, ICommand removeActionCommand)
        {
            EditActionNameCommand = new RelayCommand(ExecuteEditActionName);
            ShowContextMenuCommand = new RelayCommand(ExecuteShowContextMenu);
            UseNotepadCommand = new RelayCommand(ExecuteUseNotepad);
            UseVSCodeCommand = new RelayCommand(ExecuteUseVSCode);
            UseNotepadPlusPlusCommand = new RelayCommand(ExecuteUseNotepadPlusPlus);
            RemoveActionCommand = removeActionCommand;

            settings = s;
            AvailableEditors = availableEditors;
            ActionName = name;
        }

        private void ExecuteEditActionName()
        {
            EditingActionName = ActionName;
            IsEditing = true;
        }

        private void ExecuteShowContextMenu(object? parameter)
        {
            if (parameter is Grid element)
            {
                ContextMenu contextMenu = element.ContextMenu;
                if (contextMenu.IsEnabled)
                {
                    contextMenu.PlacementTarget = element;
                    contextMenu.IsOpen = true;
                }
            }
        }

        private void ExecuteUseNotepad()
        {
            string fullTargetFilePath = Path.Combine(AppConstants.AppModuleDirectory, ActionName.Replace('/', Path.DirectorySeparatorChar));

            string editorArguments = $"\"{fullTargetFilePath}\"";
            string processName = "notepad.exe";

            try
            {
                ProcessStartInfo processStartInfo = new(processName, editorArguments)
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal,
                    CreateNoWindow = false
                };
                Process.Start(processStartInfo);
                Debug.WriteLine($"[INFO] Successfully launched default Notepad for file: {fullTargetFilePath}");
            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                string errorMessage = $"[ERROR] Failed to start {processName} process. Please verify the executable is accessible in the system's PATH. Arguments: {editorArguments}. Error: {win32Exception.Message}";
                Debug.WriteLine(errorMessage);
            }
            catch (Exception unexpectedException)
            {
                string errorMessage = $"[ERROR] An unexpected error occurred while trying to launch {processName}. Arguments: {editorArguments}. Error: {unexpectedException.Message}";
                Debug.WriteLine(errorMessage);
            }
        }
        private void ExecuteUseVSCode()
        {
            string appWorkspacePath = AppConstants.AppDataDirectory;

            string fullTargetFilePath = Path.Combine(AppConstants.AppModuleDirectory, ActionName.Replace('/', Path.DirectorySeparatorChar));

            string editorArguments = $"\"{appWorkspacePath}\" \"{fullTargetFilePath}\"";
            string processName = "code";
            string editorName = "VS Code";

            try
            {
                ProcessStartInfo processStartInfo = new(processName, editorArguments)
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
                Debug.WriteLine($"[INFO] Successfully launched {editorName}. Arguments: {editorArguments}");
            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                string errorMessage = $"[ERROR] Failed to start {editorName} process using command '{processName}'. Please ensure '{processName}' command is accessible in the system's PATH. Arguments: {editorArguments}. Error: {win32Exception.Message}";
                Debug.WriteLine(errorMessage);
            }
            catch (Exception unexpectedException)
            {
                string errorMessage = $"[ERROR] An unexpected error occurred while trying to launch {editorName}. Arguments: {editorArguments}. Error: {unexpectedException.Message}";
                Debug.WriteLine(errorMessage);
            }
        }

        private void ExecuteUseNotepadPlusPlus()
        {
            string fullTargetFilePath = Path.Combine(AppConstants.AppModuleDirectory, ActionName.Replace('/', Path.DirectorySeparatorChar));

            string editorArguments = $"\"{fullTargetFilePath}\"";
            string editorPath = AvailableEditors.NotepadPlusPlusPath;
            string editorName = "Notepad++";

            try
            {
                ProcessStartInfo processStartInfo = new(editorPath, editorArguments)
                {
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
                Debug.WriteLine($"[INFO] Successfully launched {editorName} for file: {fullTargetFilePath}");
            }
            catch (System.ComponentModel.Win32Exception win32Exception)
            {
                string errorMessage = $"[ERROR] Failed to start {editorName} process from path: '{editorPath}'. Please verify the path is correct and accessible. Arguments: {editorArguments}. Error: {win32Exception.Message}";
                Debug.WriteLine(errorMessage);
            }
            catch (Exception unexpectedException)
            {
                string errorMessage = $"[ERROR] An unexpected error occurred while trying to launch {editorName}. Arguments: {editorArguments}. Error: {unexpectedException.Message}";
                Debug.WriteLine(errorMessage);
            }
        }

        public static readonly DependencyProperty AvailableEditorsProperty =
            DependencyProperty.Register(
                nameof(AvailableEditors),
                typeof(EditorAvailability),
                typeof(ActionManagementState),
                new PropertyMetadata(new EditorAvailability { }));
        public EditorAvailability AvailableEditors
        {
            get { return (EditorAvailability)GetValue(AvailableEditorsProperty); }
            set { SetValue(AvailableEditorsProperty, value); }
        }

        public static readonly DependencyProperty ActionNameProperty =
            DependencyProperty.Register(
                nameof(ActionName),
                typeof(string),
                typeof(ActionItemState),
                new PropertyMetadata(""));
        public string ActionName
        {
            get { return (string)GetValue(ActionNameProperty); }
            set { SetValue(ActionNameProperty, value); }
        }

        public static readonly DependencyProperty EditingActionNameProperty =
            DependencyProperty.Register(
                nameof(EditingActionName),
                typeof(string),
                typeof(ActionItemState),
                new PropertyMetadata(""));
        public string EditingActionName
        {
            get { return (string)GetValue(EditingActionNameProperty); }
            set { SetValue(EditingActionNameProperty, value); }
        }

        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register(
                nameof(IsEditing),
                typeof(bool),
                typeof(ActionItemState),
                new PropertyMetadata(false));
        public bool IsEditing
        {
            get { return (bool)GetValue(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        public void UpdateActionName()
        {
            IsEditing = false;
            string oldValue = ActionName;
            string newValue = EditingActionName;

            if (!newValue.EndsWith(".lua"))
            {
                newValue += ".lua";
            }
            if (newValue != oldValue && SettingsManager.IsValidModuleName(newValue))
            {
                ActionName = newValue;
                settings.RenameExternalAction(oldValue, newValue);
            }
        }
    }


    public class ActionItemStateConverter : System.Windows.Data.IMultiValueConverter
    {
        public object? Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3 && values[0] is Component component && component.DataContext is MainContext ctx && values[1] is string menuName && values[2] is ICommand removeActionCommand)
            {
                return new ActionItemState(ctx.Settings, ctx.AvailableEditors, menuName, removeActionCommand);
            }

            return DependencyProperty.UnsetValue;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
