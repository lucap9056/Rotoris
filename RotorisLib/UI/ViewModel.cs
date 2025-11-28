namespace RotorisLib.UI
{
    public class ViewModel : System.Windows.DependencyObject
    {
        public static readonly System.Windows.DependencyProperty AccentBrushProperty =
            System.Windows.DependencyProperty.Register(
                nameof(AccentBrush),
                typeof(System.Windows.Media.SolidColorBrush),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(new UserInterface.AppThemeBrushes().AccentBrush)
                );

        public System.Windows.Media.SolidColorBrush AccentBrush
        {
            get => (System.Windows.Media.SolidColorBrush)GetValue(AccentBrushProperty);
            set => SetValue(AccentBrushProperty, value);
        }

        public static readonly System.Windows.DependencyProperty ThemeBrushesProperty =
            System.Windows.DependencyProperty.Register(
                nameof(ThemeBrushes),
                typeof(UserInterface.AppThemeBrushes),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(new UserInterface.AppThemeBrushes())
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

        public static readonly System.Windows.DependencyProperty PaddingProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Padding),
                typeof(int),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(10));
        public int Padding
        {
            get => (int)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public static readonly System.Windows.DependencyProperty CenterPointProperty =
            System.Windows.DependencyProperty.Register(
                nameof(CenterPoint),
                typeof(System.Windows.Point),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(new System.Windows.Point(200, 200)));
        public string CenterPoint
        {
            get => (string)GetValue(CenterPointProperty);
            set => SetValue(CenterPointProperty, value);
        }

        public static readonly System.Windows.DependencyProperty OutsideRadiusProperty =
            System.Windows.DependencyProperty.Register(
                nameof(OutsideRadius),
                typeof(double),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata((double)200));
        public double OutsideRadius
        {
            get => (double)GetValue(OutsideRadiusProperty);
            set => SetValue(OutsideRadiusProperty, value);
        }

        public static readonly System.Windows.DependencyProperty InsideRadiusProperty =
            System.Windows.DependencyProperty.Register(
                nameof(InsideRadius),
                typeof(double),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata((double)100));
        public double InsideRadius
        {
            get => (double)GetValue(InsideRadiusProperty);
            set => SetValue(InsideRadiusProperty, value);
        }

        public static readonly System.Windows.DependencyProperty CenterRadiusProperty =
            System.Windows.DependencyProperty.Register(
                nameof(CenterRadius),
                typeof(double),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata((double)95));
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

        private static void OnSizeChanged(System.Windows.DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (d is ViewModel viewerState)
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;
                if (oldValue == newValue)
                {
                    return;
                }

                double halfSize = newValue / 2;
                int padding = (int)d.GetValue(PaddingProperty);

                d.SetValue(CenterPointProperty, new System.Windows.Point(halfSize + padding / 2, halfSize + padding / 2));
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

        public static readonly System.Windows.DependencyProperty SizeProperty =
            System.Windows.DependencyProperty.Register(
                nameof(Size),
                typeof(double),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(0.0, OnSizeChanged));
        public double Size
        {
            get => (double)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        public struct OptionSectorData
        {
            public System.Windows.Size OuterRadiusSize { get; set; }
            public System.Windows.Size InnerRadiusSize { get; set; }
            public System.Windows.Point OuterStartPoint { get; set; }
            public System.Windows.Point OuterEndPoint { get; set; }
            public System.Windows.Point InnerStartPoint { get; set; }
            public System.Windows.Point InnerEndPoint { get; set; }
            public bool IsLargeArc { get; set; }
            public OptionSectorData(double width, double height, int optionCount, int padding)
            {
                System.Windows.Point centerPoint = new(width / 2, height / 2);
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

                    OuterRadiusSize = new System.Windows.Size(outerRadiusX, outerRadiusY);
                    InnerRadiusSize = new System.Windows.Size(innerRadiusX, innerRadiusY);
                    IsLargeArc = anglePerOption > 180;
                }
            }
        }

        public static readonly System.Windows.DependencyProperty OptionSectorProperty =
            System.Windows.DependencyProperty.Register(
                nameof(OptionSector),
                typeof(OptionSectorData),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(new OptionSectorData { }));
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

        public static readonly System.Windows.DependencyProperty FocusedMenuOptionIndexProperty =
            System.Windows.DependencyProperty.Register(
                nameof(FocusedMenuOptionIndex),
                typeof(int),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(0)
                );

        public int FocusedMenuOptionIndex
        {
            get => (int)GetValue(FocusedMenuOptionIndexProperty);
            set => SetValue(FocusedMenuOptionIndexProperty, value);
        }

        public static readonly System.Windows.DependencyProperty MenuOptionsProperty =
            System.Windows.DependencyProperty.Register(
                nameof(MenuOptions),
                typeof(MenuOptionData[]),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(Array.Empty<MenuOptionData>())
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

        public static readonly System.Windows.DependencyProperty MessageContentProperty =
            System.Windows.DependencyProperty.Register(
                nameof(MessageContent),
                typeof(string),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(""));
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

        public static readonly System.Windows.DependencyProperty MessageCanvasBitmapProperty =
            System.Windows.DependencyProperty.Register(
                nameof(MessageCanvasBitmap),
                typeof(System.Windows.Media.Imaging.WriteableBitmap),
                typeof(ViewModel),
                new System.Windows.PropertyMetadata(null));
        public System.Windows.Media.Imaging.WriteableBitmap MessageCanvasBitmap
        {
            get => (System.Windows.Media.Imaging.WriteableBitmap)GetValue(MessageCanvasBitmapProperty);
            set => SetValue(MessageCanvasBitmapProperty, value);
        }

        /*
         * 
         * 
         * 
         */

    }
}
