using Xamarin.Forms;

namespace Poc.Luis.Xamarin.Views
{
    public partial class BaseNavigationPage : NavigationPage
    {
        public BaseNavigationPage(Page page) : base(page)
        {
            InitializeComponent();
        }
    }
}
