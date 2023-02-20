using Weather.MVVM.ViewModels;

namespace Weather.MVVM.Views;

public partial class WeatherView : ContentPage
{
	public WeatherView()
	{
		BindingContext = new WeatherViewModel();
		InitializeComponent();

	}
}