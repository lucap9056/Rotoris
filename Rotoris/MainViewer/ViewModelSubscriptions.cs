namespace Rotoris.MainViewer
{
    public partial class MainWindow
    {
        public void InitializeViewModelSubscriptions()
        {
            viewModel.SizeChanged += OnSizeChanged;
        }
    }
}
