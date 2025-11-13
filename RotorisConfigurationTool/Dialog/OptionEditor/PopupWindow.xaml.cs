
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows;
using RotorisLib;

namespace RotorisConfigurationTool.Dialog.OptionEditor
{
    /// <summary>
    /// PopupWindow.xaml 的互動邏輯
    /// </summary>
    public partial class PopupWindow : Window
    {
        public event OptionUpdateEventHandler? OptionUpdated;
        public PopupWindow(SettingsManager settings, MenuOptionData option)
        {
            InitializeComponent();
            OptionEditorState state = new()
            {
                OptionId = option.Id,
                IconPath = option.IconPath,
                ActionId = option.ActionId,
                RadialMenuNames = settings.RadialMenuNames,
                ExternalActionNames = settings.ExternalActionNames,
                Loaded = true,
            };

            Resources["State"] = state;

            state.OptionUpdated += (updatedOption) =>
            {
                OptionUpdated?.Invoke(updatedOption);
                DialogResult = true;
                Close();
            };
        }

        private void OptionId_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is OptionEditorState state)
            {
                state.CurrentSelectionView = OptionSelectionViewType.OptionId;
            }
        }

        private void IconPath_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is OptionEditorState state)
            {
                state.CurrentSelectionView = OptionSelectionViewType.IconPath;
            }
        }

        private void ActionId_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is OptionEditorState state)
            {
                state.CurrentSelectionView = OptionSelectionViewType.ActionId;
            }
        }

        private void OptionId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.DataContext is OptionEditorState state)
            {
                if (state.SetOptionIdCommand.CanExecute(listBox.SelectedItem))
                {
                    state.SetOptionIdCommand.Execute(listBox.SelectedItem);
                }
                listBox.SelectedItem = null;
            }
        }

        private void IconPath_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.DataContext is OptionEditorState state)
            {
                if (state.SetIconPathCommand.CanExecute(listBox.SelectedItem))
                {
                    state.SetIconPathCommand.Execute(listBox.SelectedItem);
                }
                listBox.SelectedItem = null;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView && treeView.DataContext is OptionEditorState state)
            {
                if (state.SetActionIdCommand.CanExecute(treeView.SelectedItem))
                {
                    state.SetActionIdCommand.Execute(treeView.SelectedItem);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
