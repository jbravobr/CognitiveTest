using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;

namespace Poc.Luis.Xamarin.ViewModels
{
    [ImplementPropertyChanged]
    public class BlankPageViewModel : BindableBase, INavigatedAware
    {
        public string Words { get; set; }

        public BlankPageViewModel()
        {
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("Words"))
                Words = parameters["Words"].ToString();
        }
    }
}