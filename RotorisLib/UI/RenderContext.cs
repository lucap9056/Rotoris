namespace RotorisLib.UI
{
    public class RenderContextAttacher
    {
        public readonly record struct RenderContextData(double AngleIncrement, double Radius, System.Windows.Point CanvasCenter, double IconSize);

        public static readonly System.Windows.DependencyProperty ContextDataProperty =
            System.Windows.DependencyProperty.RegisterAttached(
                "ContextData",
                typeof(RenderContextData),
                typeof(RenderContextAttacher),
                new System.Windows.PropertyMetadata(
                    default(RenderContextData)
                    )
                );


        public static RenderContextData GetContextData(System.Windows.DependencyObject obj)
        {
            return (RenderContextData)obj.GetValue(ContextDataProperty);
        }

        public static void SetContextData(System.Windows.DependencyObject obj, RenderContextData value)
        {
            obj.SetValue(ContextDataProperty, value);
        }


        public readonly record struct RenderIconRect(double Top, double Left, double Width, double Height);

        public static readonly System.Windows.DependencyProperty IconRectProperty =
            System.Windows.DependencyProperty.RegisterAttached(
                "IconRect",
                typeof(RenderIconRect),
                typeof(RenderContextAttacher),
                new System.Windows.PropertyMetadata(
                    default(RenderIconRect)
                    )
                );

        public static RenderIconRect GetIconRect(System.Windows.DependencyObject obj)
        {
            return (RenderIconRect)obj.GetValue(IconRectProperty);
        }

        public static void SetIconRect(System.Windows.DependencyObject obj, RenderIconRect value)
        {
            obj.SetValue(IconRectProperty, value);
        }
    }

}
