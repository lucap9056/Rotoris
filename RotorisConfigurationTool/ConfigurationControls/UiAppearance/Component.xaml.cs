using System.Windows.Controls;
using System.Windows;

namespace RotorisConfigurationTool.ConfigurationControls.UiAppearance
{
    /// <summary>
    /// UiAppearance.xaml 的互動邏輯
    /// </summary>
    public partial class Component : UserControl
    {
        public static readonly DependencyProperty IsTabSelectedProperty =
            DependencyProperty.Register(
                nameof(IsTabSelected),
                typeof(bool),
                typeof(Component),
                new PropertyMetadata(true));

        public bool IsTabSelected
        {
            get { return (bool)GetValue(IsTabSelectedProperty); }
            set { SetValue(IsTabSelectedProperty, value); }
        }
        public Component()
        {
            InitializeComponent();

            Loaded += Component_Loaded;
        }
        private void Component_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= Component_Loaded;

            if (Parent is TabItem parentTabItem && parentTabItem.Parent is TabControl tabControl)
            {
                tabControl.SelectionChanged += (senderTab, eTab) =>
                {
                    if (eTab.AddedItems.Contains(parentTabItem))
                    {
                        IsTabSelected = true;
                    }
                    else if (eTab.RemovedItems.Contains(parentTabItem))
                    {
                        IsTabSelected = false;
                    }
                };
            }
        }
    }
}
