namespace Rotoris.MainViewer
{
    public partial class MainWindow
    {
        public void InitializeStateSubscriptions()
        {
            State.SizeChanged += OnSizeChanged;
        }
    }
}
