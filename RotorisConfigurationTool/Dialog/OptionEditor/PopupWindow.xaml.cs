
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
            OptionEditorViewModel viewModel = new()
            {
                OptionId = option.Id,
                IconPath = option.IconPath,
                ActionId = option.ActionId,
                RadialMenuNames = settings.RadialMenuNames,
                ExternalActionNames = settings.ExternalActionNames,
                Loaded = true,
            };

            Resources["ViewModel"] = viewModel;

            viewModel.OptionUpdated += (updatedOption) =>
            {
                OptionUpdated?.Invoke(updatedOption);
                DialogResult = true;
                Close();
            };
        }

        private void OptionId_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is OptionEditorViewModel viewModel)
            {
                viewModel.CurrentSelectionView = OptionSelectionViewType.OptionId;
            }
        }

        private void IconPath_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is OptionEditorViewModel viewModel)
            {
                viewModel.CurrentSelectionView = OptionSelectionViewType.IconPath;
            }
        }

        private void ActionId_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.DataContext is OptionEditorViewModel viewModel)
            {
                viewModel.CurrentSelectionView = OptionSelectionViewType.ActionId;
            }
        }

        private void OptionId_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.DataContext is OptionEditorViewModel viewModel)
            {
                if (viewModel.SetOptionIdCommand.CanExecute(listBox.SelectedItem))
                {
                    viewModel.SetOptionIdCommand.Execute(listBox.SelectedItem);
                }
                listBox.SelectedItem = null;
            }
        }

        private void IconPath_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.DataContext is OptionEditorViewModel viewModel)
            {
                if (viewModel.SetIconPathCommand.CanExecute(listBox.SelectedItem))
                {
                    viewModel.SetIconPathCommand.Execute(listBox.SelectedItem);
                }
                listBox.SelectedItem = null;
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (sender is TreeView treeView && treeView.DataContext is OptionEditorViewModel viewModel)
            {
                if (viewModel.SetActionIdCommand.CanExecute(treeView.SelectedItem))
                {
                    viewModel.SetActionIdCommand.Execute(treeView.SelectedItem);
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
