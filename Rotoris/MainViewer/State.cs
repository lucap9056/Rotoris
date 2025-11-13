using RotorisLib;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rotoris.MainViewer
{
    public class MainViewerState : DependencyObject
    {

        public static readonly DependencyProperty AccentBrushProperty =
            DependencyProperty.Register(
                nameof(AccentBrush),
                typeof(SolidColorBrush),
                typeof(MainViewerState),
                new PropertyMetadata(new UserInterface.AppThemeBrushes().AccentBrush)
                );

        public SolidColorBrush AccentBrush
        {
            get => (SolidColorBrush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }

        public static readonly DependencyProperty ThemeBrushesProperty =
            DependencyProperty.Register(
                nameof(ThemeBrushes),
                typeof(UserInterface.AppThemeBrushes),
                typeof(MainViewerState),
                new PropertyMetadata(new UserInterface.AppThemeBrushes())
                );

        public UserInterface.AppThemeBrushes ThemeBrushes
        {
            get => (UserInterface.AppThemeBrushes)GetValue(ThemeBrushesProperty);
            set
            {
                AccentBrush = value.AccentBrush;
                SetValue(ThemeBrushesProperty, value);
            }
        }

        /*
         * 
         * 
         * 
         */

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(
                nameof(Padding),
                typeof(int),
                typeof(MainViewerState),
                new PropertyMetadata(10));
        public int Padding
        {
            get => (int)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public static readonly DependencyProperty CenterPointProperty =
            DependencyProperty.Register(
                nameof(CenterPoint),
                typeof(Point),
                typeof(MainViewerState),
                new PropertyMetadata(new Point(200, 200)));
        public string CenterPoint
        {
            get => (string)GetValue(CenterPointProperty);
            set => SetValue(CenterPointProperty, value);
        }

        public static readonly DependencyProperty OutsideRadiusProperty =
            DependencyProperty.Register(
                nameof(OutsideRadius),
                typeof(double),
                typeof(MainViewerState),
                new PropertyMetadata((double)200));
        public double OutsideRadius
        {
            get => (double)GetValue(OutsideRadiusProperty);
            set => SetValue(OutsideRadiusProperty, value);
        }

        public static readonly DependencyProperty InsideRadiusProperty =
            DependencyProperty.Register(
                nameof(InsideRadius),
                typeof(double),
                typeof(MainViewerState),
                new PropertyMetadata((double)100));
        public double InsideRadius
        {
            get => (double)GetValue(InsideRadiusProperty);
            set => SetValue(InsideRadiusProperty, value);
        }

        public static readonly DependencyProperty CenterRadiusProperty =
            DependencyProperty.Register(
                nameof(CenterRadius),
                typeof(double),
                typeof(MainViewerState),
                new PropertyMetadata((double)95));
        public double CenterRadius
        {
            get => (double)GetValue(CenterRadiusProperty);
            set => SetValue(CenterRadiusProperty, value);
        }

        /*
         * 
         * 
         * 
         */

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainViewerState viewerState)
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;
                if (oldValue == newValue)
                {
                    return;
                }

                double halfSize = newValue / 2;
                int padding = (int)d.GetValue(PaddingProperty);

                d.SetValue(CenterPointProperty, new Point(halfSize + padding / 2, halfSize + padding / 2));
                d.SetValue(OutsideRadiusProperty, halfSize);
                d.SetValue(InsideRadiusProperty, newValue / 4.0);
                d.SetValue(CenterRadiusProperty, newValue * 0.475 / 2.0);

                viewerState.OnSizeValueChanged(
                    (double)e.OldValue,
                    (double)e.NewValue);
            }
        }
        public delegate void SizeChangedEventHandler(
            object sender,
            double oldValue,
            double newValue);

        public event SizeChangedEventHandler? SizeChanged;
        protected virtual void OnSizeValueChanged(
            double oldValue,
            double newValue)
        {
            SizeChanged?.Invoke(this, oldValue, newValue);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(
                nameof(Size),
                typeof(double),
                typeof(MainViewerState),
                new PropertyMetadata((double)400, OnSizeChanged));
        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public struct OptionSectorData
        {
            public Size OuterRadiusSize { get; set; }
            public Size InnerRadiusSize { get; set; }
            public Point OuterStartPoint { get; set; }
            public Point OuterEndPoint { get; set; }
            public Point InnerStartPoint { get; set; }
            public Point InnerEndPoint { get; set; }
            public bool IsLargeArc { get; set; }
            public OptionSectorData(double width, double height, int optionCount, int padding)
            {
                Point centerPoint = new(width / 2, height / 2);
                double anglePerOption = 360.0 / optionCount;
                double halfAngle = anglePerOption / 2.0;

                double startAngleRad = (-halfAngle) * Math.PI / 180.0;
                double endAngleRad = halfAngle * Math.PI / 180.0;

                double outerRadiusX = (width - padding) / 2.0;
                double outerRadiusY = (height - padding) / 2.0;

                double innerRadiusX = outerRadiusX / 2.0;
                double innerRadiusY = outerRadiusY / 2.0;

                if (innerRadiusX > 0 && innerRadiusY > 0)
                {
                    OuterStartPoint = new(
                    centerPoint.X + outerRadiusX * Math.Cos(startAngleRad),
                    centerPoint.Y + outerRadiusY * Math.Sin(startAngleRad));
                    OuterEndPoint = new(
                    centerPoint.X + outerRadiusX * Math.Cos(endAngleRad),
                    centerPoint.Y + outerRadiusY * Math.Sin(endAngleRad));

                    InnerStartPoint = new(
                    centerPoint.X + innerRadiusX * Math.Cos(endAngleRad),
                    centerPoint.Y + innerRadiusY * Math.Sin(endAngleRad));
                    InnerEndPoint = new(
                        centerPoint.X + innerRadiusX * Math.Cos(startAngleRad),
                        centerPoint.Y + innerRadiusY * Math.Sin(startAngleRad));

                    OuterRadiusSize = new Size(outerRadiusX, outerRadiusY);
                    InnerRadiusSize = new Size(innerRadiusX, innerRadiusY);
                    IsLargeArc = anglePerOption > 180;
                }
            }
        }

        public static readonly DependencyProperty OptionSectorProperty =
            DependencyProperty.Register(
                nameof(OptionSector),
                typeof(OptionSectorData),
                typeof(MainViewerState),
                new PropertyMetadata(new OptionSectorData { }));
        public OptionSectorData OptionSector
        {
            get => (OptionSectorData)GetValue(OptionSectorProperty);
            set => SetValue(OptionSectorProperty, value);
        }

        /*
         * 
         * 
         * 
         */

        public static readonly DependencyProperty FocusedMenuOptionIndexProperty =
            DependencyProperty.Register(
                nameof(FocusedMenuOptionIndex),
                typeof(int),
                typeof(MainViewerState),
                new PropertyMetadata(0)
                );

        public int FocusedMenuOptionIndex
        {
            get => (int)GetValue(FocusedMenuOptionIndexProperty);
            set => SetValue(FocusedMenuOptionIndexProperty, value);
        }

        public static readonly DependencyProperty MenuOptionsProperty =
            DependencyProperty.Register(
                nameof(MenuOptions),
                typeof(MenuOptionData[]),
                typeof(MainViewerState),
                new PropertyMetadata(Array.Empty<MenuOptionData>())
                );

        public MenuOptionData[] MenuOptions
        {
            get => (MenuOptionData[])GetValue(MenuOptionsProperty);
            set => SetValue(MenuOptionsProperty, value);
        }

        /*
         * 
         * 
         * 
         */

        public static readonly DependencyProperty MessageContentProperty =
            DependencyProperty.Register(
                nameof(MessageContent),
                typeof(string),
                typeof(MainViewerState),
                new PropertyMetadata(""));
        public string MessageContent
        {
            get => (string)GetValue(MessageContentProperty);
            set => SetValue(MessageContentProperty, value);
        }

        /*
         * 
         * 
         * 
         */

        public static readonly DependencyProperty MessageCanvasBitmapProperty =
            DependencyProperty.Register(
                nameof(MessageCanvasBitmap),
                typeof(WriteableBitmap),
                typeof(MainViewerState),
                new PropertyMetadata(null));
        public WriteableBitmap MessageCanvasBitmap
        {
            get => (WriteableBitmap)GetValue(MessageCanvasBitmapProperty);
            set => SetValue(MessageCanvasBitmapProperty, value);
        }
    }

    public class NotNullToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
