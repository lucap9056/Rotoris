using RotorisConfigurationTool.Properties.Dialog.OptionEditor;
using BuiltInOptionsI18n = RotorisLib.Properties.I18n;
using RotorisLib.UI;
using RotorisLib;

namespace RotorisConfigurationTool.Dialog.OptionEditor
{
    public enum OptionSelectionViewType
    {
        /// <summary>No selection view is currently displayed.</summary>
        None,
        /// <summary>View for selecting built-in Option IDs.</summary>
        OptionId,
        /// <summary>View for selecting Icon Paths (built-in).</summary>
        IconPath,
        /// <summary>View for selecting Action IDs (built-in, menu open, or external actions).</summary>
        ActionId,
    }

    public delegate void OptionUpdateEventHandler(MenuOptionData option);
    internal class OptionEditorViewModel : System.Windows.DependencyObject
    {
        public event OptionUpdateEventHandler? OptionUpdated;
        public System.Windows.Input.ICommand IconPathBrowseCommand { get; }
        public System.Windows.Input.ICommand SetOptionIdCommand { get; }
        public System.Windows.Input.ICommand SetIconPathCommand { get; }
        public System.Windows.Input.ICommand SetActionIdCommand { get; }
        public System.Windows.Input.ICommand RemoveCommand { get; }
        public System.Windows.Input.ICommand SaveCommand { get; }
        public OptionEditorViewModel()
        {
            IconPathBrowseCommand = new RelayCommand(ExecuteIconPathBrowse);
            SetOptionIdCommand = new RelayCommand(ExecuteSetOptionId);
            SetIconPathCommand = new RelayCommand(ExecuteSetIconPath);
            SetActionIdCommand = new RelayCommand(ExecuteSetActionId);
            RemoveCommand = new RelayCommand(ExecuteRemove);
            SaveCommand = new RelayCommand(ExecuteSave);
        }

        private void ExecuteIconPathBrowse()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Filter = "Image Files and Executables (*.png;*.jpg;*.jpeg;*.ico;*.exe)|*.png;*.jpg;*.jpeg;*.ico;*.exe|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() ?? false)
            {
                IconPath = openFileDialog.FileName;
            }
        }
        private void ExecuteSetOptionId(object? parameter)
        {
            if (parameter is string optionId && !string.IsNullOrEmpty(optionId))
            {
                OptionId = optionId;
            }
        }

        private void ExecuteSetIconPath(object? parameter)
        {
            if (parameter is KeyValuePair<string, string> iconPath && !string.IsNullOrEmpty(iconPath.Key))
            {
                IconPath = iconPath.Key;
            }
        }

        private void ExecuteSetActionId(object? parameter)
        {
            if (parameter is TreeItem item && !string.IsNullOrEmpty(item.Value))
            {
                ActionId = item.Value;
            }
        }
        private void ExecuteRemove()
        {
            OptionUpdated?.Invoke(new MenuOptionData { Id = "" });
        }
        private void ExecuteSave()
        {
            if (string.IsNullOrEmpty(OptionId))
            {
                return;
            }

            MenuOptionData option = new()
            {
                Id = OptionId,
                IconPath = IconPath,
                ActionId = ActionId,
            };

            option.ResolveIconPath();
            OptionUpdated?.Invoke(option);
        }

        public static readonly System.Windows.DependencyProperty LoadedProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Loaded),
                typeof(bool),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata(false)
                );

        public bool Loaded
        {
            get => (bool)GetValue(LoadedProperty);
            set => SetValue(LoadedProperty, value);
        }

        public static readonly System.Windows.DependencyProperty OptionIdProperty =
            System.Windows.DependencyProperty.Register(
                nameof(OptionId),
                typeof(string),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata("")
                );

        public string OptionId
        {
            get => (string)GetValue(OptionIdProperty);
            set => SetValue(OptionIdProperty, value);
        }

        public static readonly System.Windows.DependencyProperty IconPathProperty =
            System.Windows.DependencyProperty.Register(
                nameof(IconPath),
                typeof(string),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata("")
                );

        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        public static readonly System.Windows.DependencyProperty ActionIdProperty =
            System.Windows.DependencyProperty.Register(
                nameof(ActionId),
                typeof(string),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata("")
                );

        public string ActionId
        {
            get => (string)GetValue(ActionIdProperty);
            set => SetValue(ActionIdProperty, value);
        }


        public static readonly System.Windows.DependencyProperty CurrentSelectionViewProperty =
            System.Windows.DependencyProperty.Register(
                nameof(CurrentSelectionView),
                typeof(OptionSelectionViewType),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata(OptionSelectionViewType.OptionId, OnCurrentSelectionViewChanged)
                );

        public OptionSelectionViewType CurrentSelectionView
        {
            get => (OptionSelectionViewType)GetValue(CurrentSelectionViewProperty);
            set => SetValue(CurrentSelectionViewProperty, value);
        }

        private static void OnCurrentSelectionViewChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (d is OptionEditorViewModel state && e.NewValue is OptionSelectionViewType t)
            {
                switch (t)
                {
                    case OptionSelectionViewType.OptionId:
                        state.SelectionViewLabelKey = "ListLabel-Options";
                        state.SelectionViewDescriptionKey = "ListDescription-Options";
                        break;
                    case OptionSelectionViewType.IconPath:
                        state.SelectionViewLabelKey = "ListLabel-Icons";
                        state.SelectionViewDescriptionKey = "ListDescription-Icons";
                        break;
                    case OptionSelectionViewType.ActionId:
                        state.SelectionViewLabelKey = "ListLabel-Actions";
                        state.SelectionViewDescriptionKey = "ListDescription-Actions";
                        break;
                }
            }
        }

        public static readonly System.Windows.DependencyProperty SelectionViewLabelKeyProperty =
            System.Windows.DependencyProperty.Register(
                nameof(SelectionViewLabelKey),
                typeof(string),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata("ListLabel-Options")
                );

        public string SelectionViewLabelKey
        {
            get => (string)GetValue(SelectionViewLabelKeyProperty);
            set => SetValue(SelectionViewLabelKeyProperty, value);
        }

        public static readonly System.Windows.DependencyProperty SelectionViewDescriptionKeyProperty =
            System.Windows.DependencyProperty.Register(
                nameof(SelectionViewDescriptionKey),
                typeof(string),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata("ListDescription-Options")
                );

        public string SelectionViewDescriptionKey
        {
            get => (string)GetValue(SelectionViewDescriptionKeyProperty);
            set => SetValue(SelectionViewDescriptionKeyProperty, value);
        }

        public static readonly System.Windows.DependencyProperty BuiltInOptionsProperty =
            System.Windows.DependencyProperty.Register(
                nameof(BuiltInOptions),
                typeof(string[]),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata((string[])[.. AppConstants.BuiltInOptionsMap.Keys])
                );

        public string[] BuiltInOptions
        {
            get => (string[])GetValue(BuiltInOptionsProperty);
            set => SetValue(BuiltInOptionsProperty, value);
        }

        public static readonly System.Windows.DependencyProperty BuiltInIconPathsProperty =
            System.Windows.DependencyProperty.Register(
                nameof(BuiltInIconPaths),
                typeof(Dictionary<string, string>),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata(AppConstants.BuiltInIconPaths)
                );

        public Dictionary<string, string> BuiltInIconPaths
        {
            get => (Dictionary<string, string>)GetValue(BuiltInIconPathsProperty);
            set => SetValue(BuiltInIconPathsProperty, value);
        }

        public static readonly System.Windows.DependencyProperty RadialMenuNamesProperty =
            System.Windows.DependencyProperty.Register(
                nameof(RadialMenuNames),
                typeof(string[]),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata(Array.Empty<string>())
                );

        public string[] RadialMenuNames
        {
            get => (string[])GetValue(RadialMenuNamesProperty);
            set => SetValue(RadialMenuNamesProperty, value);
        }

        public static readonly System.Windows.DependencyProperty ExternalActionNamesProperty =
            System.Windows.DependencyProperty.Register(
                nameof(ExternalActionNames),
                typeof(string[]),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata(Array.Empty<string>(), OnExternalActionNamesChanged)
                );

        public string[] ExternalActionNames
        {
            get => (string[])GetValue(ExternalActionNamesProperty);
            set => SetValue(ExternalActionNamesProperty, value);
        }


        public static readonly System.Windows.DependencyProperty ExternalActionTreeProperty =
            System.Windows.DependencyProperty.Register(
                nameof(ExternalActionTree),
                typeof(System.Collections.ObjectModel.ObservableCollection<TreeItem>),
                typeof(OptionEditorViewModel),
                new System.Windows.PropertyMetadata((System.Collections.ObjectModel.ObservableCollection<TreeItem>)[])
                );

        public System.Collections.ObjectModel.ObservableCollection<TreeItem> ExternalActionTree
        {
            get => (System.Collections.ObjectModel.ObservableCollection<TreeItem>)GetValue(ExternalActionTreeProperty);
            set => SetValue(ExternalActionTreeProperty, value);
        }

        private static void OnExternalActionNamesChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {

            if (d is OptionEditorViewModel state && e.NewValue is string[] externalActionNames)
            {

                TreeItem builtInActionItems = new(
                    I18n.ResourceManager.GetString("BuiltIn", SettingsManager.CurrentCulture) ?? "BuiltIn"
                    );

                foreach (string builtInOption in state.BuiltInOptions)
                {
                    string header = BuiltInOptionsI18n.ResourceManager.GetString(builtInOption, SettingsManager.CurrentCulture) ?? builtInOption;
                    TreeItem builtInActionItem = new(header, builtInOption);
                    builtInActionItems.Children.Add(builtInActionItem);
                }

                /*
                 * 
                 * 
                 * 
                 */

                string openMenuText = I18n.ResourceManager.GetString("OpenMenu", SettingsManager.CurrentCulture) ?? "OpenMenu({0})";

                TreeItem openMenuActionItems = new(
                    I18n.ResourceManager.GetString("Menus", SettingsManager.CurrentCulture) ?? "Menus"
                    );

                foreach (string menuName in state.RadialMenuNames)
                {
                    string header = string.Format(openMenuText, menuName[..^5]);
                    TreeItem openMenuActionItem = new(header, $"OPEN_MENU-{menuName}");
                    openMenuActionItems.Children.Add(openMenuActionItem);
                }

                /*
                 * 
                 * 
                 * 
                 */

                TreeItem externalActionItems = new(
                    I18n.ResourceManager.GetString("ExternalActions", SettingsManager.CurrentCulture) ?? "External Actions"
                    );

                foreach (var fullActionPath in externalActionNames)
                {
                    string[] pathNodes = fullActionPath.Split('/');

                    TreeItem currentNode = externalActionItems;

                    for (int i = 0; i < pathNodes.Length; i++)
                    {
                        string nodeName = pathNodes[i];
                        bool isFinalAction = (i == pathNodes.Length - 1);

                        TreeItem? existingNode = currentNode.Children.FirstOrDefault(item => item.Header.Equals(nodeName, StringComparison.OrdinalIgnoreCase));

                        if (existingNode != null)
                        {
                            currentNode = existingNode;
                        }
                        else
                        {
                            TreeItem newNode;
                            if (isFinalAction)
                            {
                                newNode = new(nodeName, fullActionPath);
                                if (nodeName == "index.lua")
                                {
                                    currentNode.Value = fullActionPath[..^10];
                                }
                            }
                            else
                            {
                                newNode = new(nodeName);
                            }

                            currentNode.Children.Add(newNode);
                            currentNode = newNode;
                        }
                    }
                }

                /*
                 * 
                 * 
                 * 
                 */

                state.ExternalActionTree = [builtInActionItems, openMenuActionItems, externalActionItems];
            }
        }

    }

    public class TreeItem : System.ComponentModel.INotifyPropertyChanged
    {
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        public System.Collections.ObjectModel.ObservableCollection<TreeItem> Children { get; } = [];
        private string _header;
        private string _value = "";

        public TreeItem(string header)
        {
            _header = header;
        }

        public TreeItem(string header, string value)
        {
            _header = header;
            _value = value;
        }

        public string Header
        {
            get => _header;
            set
            {
                if (_header != value)
                {
                    _header = value;
                    OnPropertyChanged(nameof(Header));
                }
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }

    public class I18nResourceConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string key)
            {
                return I18n.ResourceManager.GetString(key, culture) ?? "";
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }

    public class BuiltInOptionNameConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string optionId)
            {
                return BuiltInOptionsI18n.ResourceManager.GetString(optionId, culture) ?? optionId;
            }

            return System.Windows.DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }

    public class OptionSelectionViewVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is OptionSelectionViewType currentType && parameter is OptionSelectionViewType listType && currentType == listType)
            {
                return System.Windows.Visibility.Visible;
            }

            return System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Windows.DependencyProperty.UnsetValue;
        }
    }
}
