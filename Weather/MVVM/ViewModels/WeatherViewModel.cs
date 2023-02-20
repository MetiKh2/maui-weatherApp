

using PropertyChanged;
using System.Text.Json;
using System.Windows.Input;
using Weather.MVVM.Models;

namespace Weather.MVVM.ViewModels
{
	[AddINotifyPropertyChangedInterface]
	public class WeatherViewModel
	{
		public WeatherData WeatherData { get; set; }
        public string PlaceName { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
		public bool IsVisible { get; set; }
		public bool IsLoading { get; set; }
		
		HttpClient client;

		public WeatherViewModel()
		{
			this.client = new();
		}

		public ICommand SearchCommand => new Command(async (searchText) =>
		{
			IsLoading = true;
			PlaceName = searchText.ToString();
			var result = await GetCordinates(searchText.ToString());
			await GetWeatherData(result);
			IsVisible = true;
			IsLoading = false;
		});

		public async Task<WeatherResult> GetCordinates(string searchText)
		{
			var url = "https://geocoding-api.open-meteo.com/v1/search?name=" + searchText;
			var response = await client.GetAsync(url);
			WeatherResult weatherResult = null;
			if (response.IsSuccessStatusCode)
			{
				using (var responseStream = await response.Content.ReadAsStreamAsync())
				{
					weatherResult = await JsonSerializer.DeserializeAsync<WeatherResult>(responseStream);
				}
			}
			return weatherResult;
		}

		public async Task GetWeatherData(WeatherResult result)
		{
			var url = $"https://api.open-meteo.com/v1/forecast?latitude={result.results[0].latitude}&longitude={result.results[0].longitude}&daily=weathercode,temperature_2m_max,temperature_2m_min&current_weather=true&timezone=Asia%2FTehran";
			var response = await client.GetAsync(url);
			if (response.IsSuccessStatusCode)
			{
				using (var responseStream = await response.Content.ReadAsStreamAsync())
				{
					var data = await JsonSerializer.DeserializeAsync<WeatherData>(responseStream);
					WeatherData = data;
					for (int i = 0; i < WeatherData.daily.time.Length; i++)
					{
						var daily2 = new Daily2()
						{
							time = WeatherData.daily.time[i],
							temperature_2m_max = WeatherData.daily.temperature_2m_max[i],
							temperature_2m_min = WeatherData.daily.temperature_2m_min[i],
							weathercode = WeatherData.daily.weathercode[i]
						};

						WeatherData.daily2.Add(daily2);
					}
				}
			}
		}
	}
}
