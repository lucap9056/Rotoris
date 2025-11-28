
using System.Windows.Media;
using Newtonsoft.Json;
using RotorisLib;

namespace RotorisConfigurationTool.Preview
{
    public partial class PreviewWindow
    {
        public void IsRadialMenuEditor(SettingsManager settings, string name = "")
        {
            State.Settings = settings;
            State.MenuName = name;
            State.Size = 500;


            {
                List<MenuOptionData> options = [.. settings.GetMenuOptions(name)];

                if (options.Count == 0)
                {
                    options.Add(AppConstants.BuiltInOptions.Empty);
                }

                DrawOptions([AppConstants.BuiltInOptions.Close, .. options], 1);
            }

            State.OriginalMenuOptionsString = JsonConvert.SerializeObject(State.MenuOptions, Formatting.Indented);

        }
        public void IsColorEditor(UserInterface.ThemeBrushIdentifier name, Configuration configuration)
        {
            LoadThemeBrushes(configuration);
            Show();

            State.MessageContent = "Hello World!";
            if (GetColorFromName(configuration, name) is Color color)
            {
                Dialog.ColorPicker.PopupWindow colorPicker = new(color) { Owner = this };
                colorPicker.ColorSelected += (selected) =>
                {
                    color = selected;
                    var themeBrushes = State.ThemeBrushes;
                    switch (name)
                    {
                        case UserInterface.ThemeBrushIdentifier.Accent:
                            themeBrushes.AccentBrush = UserInterface.AppThemeBrushes.BrushFromColor(selected);
                            break;
                        case UserInterface.ThemeBrushIdentifier.Foreground:
                            themeBrushes.ForegroundBrush = UserInterface.AppThemeBrushes.BrushFromColor(selected);
                            break;
                        case UserInterface.ThemeBrushIdentifier.Background:
                            themeBrushes.BackgroundBrush = UserInterface.AppThemeBrushes.BrushFromColor(selected);
                            break;
                    }
                    State.ThemeBrushes = themeBrushes;
                };

                DrawOptions([AppConstants.BuiltInOptions.Close, AppConstants.BuiltInOptions.Empty]);

                colorPicker.Hide();
                if (colorPicker.ShowDialog() ?? false)
                {
                    ColorChanged?.Invoke(name, color);
                }
            }

            Close();
        }
        private static Color? GetColorFromName(Configuration configuration, UserInterface.ThemeBrushIdentifier name)
        {
            return name switch
            {
                UserInterface.ThemeBrushIdentifier.Accent => (Color?)(configuration.UiAccent ?? UserInterface.AppThemeBrushes.SystemAccentColor),
                UserInterface.ThemeBrushIdentifier.Foreground => (Color?)(configuration.UiForeground ?? UserInterface.AppThemeBrushes.SystemForegroundColor),
                UserInterface.ThemeBrushIdentifier.Background => (Color?)(configuration.UiBackground ?? UserInterface.AppThemeBrushes.SystemBackgroundColor),
                _ => null,
            };
        }

        public void IsSizeEditor(double size, Configuration configuration)
        {
            LoadThemeBrushes(configuration);
            Show();
            State.Size = size;

            Dialog.SizeSlider.PopupWindow sizeSlider = new(size) { Owner = this };
            sizeSlider.SizeChanged += (updatedSize) =>
            {
                size = updatedSize;
                State.Size = updatedSize;
            };

            sizeSlider.Hide();
            if (sizeSlider.ShowDialog() ?? false)
            {
                SizeChanged?.Invoke(size);
            }

            Close();
        }

        private void DrawOptions(MenuOptionData[] options, int index = 0)
        {
            State.MenuOptions = options;
            State.OptionSector = new RotorisLib.UI.ViewModel.OptionSectorData(Width, Height, options.Length, State.Padding);
            State.FocusedMenuOptionIndex = index;
        }
    }
}
