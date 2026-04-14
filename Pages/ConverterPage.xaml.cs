using Lab1.Entities.Http;
using Lab1.Services.Http;
using System.Collections.ObjectModel;

namespace Lab1.Pages;

public partial class ConverterPage : ContentPage
{
    private readonly ObservableCollection<Rate> rates = new();
    private readonly ObservableCollection<Currency> currencies = new();
    private readonly IRateService rateService;

    public ConverterPage(IRateService rateService)
    {
        InitializeComponent();

        this.rateService = rateService;

        RatesListView.ItemsSource = rates;
        CurrencyPicker.ItemsSource = currencies;

        DatePicker.Date = DateTime.Today;
        DatePicker.MaximumDate = DateTime.Today;

        LoadCurrencies();          
        UpdateConvertButtonState(); 
    }


    private async void LoadCurrencies()
    {
        var currencyList = new List<Currency>
        {
            new Currency { Cur_Abbreviation = "RUB", Cur_Name = "Russian Ruble" },
            new Currency { Cur_Abbreviation = "EUR", Cur_Name = "Euro" },
            new Currency { Cur_Abbreviation = "USD", Cur_Name = "US Dollar" },
            new Currency { Cur_Abbreviation = "CHF", Cur_Name = "Swiss Franc" },
            new Currency { Cur_Abbreviation = "CNY", Cur_Name = "Chinese Yuan" },
            new Currency { Cur_Abbreviation = "GBP", Cur_Name = "British Pound Sterling" }
        };

        currencies.Clear();
        foreach (var c in currencyList)
            currencies.Add(c);

        await LoadRatesAsync(DatePicker.Date);
    }

    private async void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        await LoadRatesAsync(e.NewDate);
    }

    private async Task LoadRatesAsync(DateTime date)
    {
        ShowLoading(true);

        var (success, ratesForDate, error) = await rateService.GetRatesWithStatus(date);

        rates.Clear();

        if (success)
        {
            foreach (var rate in ratesForDate)
                rates.Add(rate);
        }
        else
        {
            await DisplayAlert("Ошибка", error, "OK");
        }

        ShowLoading(false);
        UpdateConvertButtonState();
    }


    private async void OnConvertClicked(object sender, EventArgs e)
    {
        if (CurrencyPicker.SelectedItem == null || string.IsNullOrWhiteSpace(ValueEntry.Text))
            return;

        if (!decimal.TryParse(ValueEntry.Text, out decimal amount))
        {
            await DisplayAlert("Ошибка", "Введите корректное число", "OK");
            return;
        }

        ShowLoading(true);
        ConvertButton.IsEnabled = false;

        var selectedCurrency = (Currency)CurrencyPicker.SelectedItem;
        var selectedDate = DatePicker.Date;

        var (success, ratesForDate, error) = await rateService.GetRatesWithStatus(selectedDate);

        if (!success)
        {
            await DisplayAlert("Ошибка", error, "OK");
            ShowLoading(false);
            UpdateConvertButtonState();
            return;
        }

        var selectedRate = ratesForDate.FirstOrDefault(r => 
            r.Cur_Abbreviation == selectedCurrency.Cur_Abbreviation);

        ConvertCurrency(amount, selectedRate);

        ShowLoading(false);
        UpdateConvertButtonState();
    }

    private void ConvertCurrency(decimal amount, Rate? selectedRate)
    {
        if (selectedRate?.Cur_OfficialRate.HasValue == true)
        {
            decimal converted = amount * (decimal)selectedRate.Cur_OfficialRate.Value / selectedRate.Cur_Scale;
            ResultLabel.Text = $"{amount} {selectedRate.Cur_Abbreviation} = {converted:F2} BYN";
        }
        else
        {
            ResultLabel.Text = "Курс для выбранной валюты не найден.";
        }
    }

    private void ShowLoading(bool isLoading)
    {
        LoadingIndicator.IsVisible = isLoading;
        LoadingIndicator.IsRunning = isLoading;
    }

    private void UpdateConvertButtonState()
    {
        bool hasInternet = Connectivity.Current.NetworkAccess == NetworkAccess.Internet;
        bool hasCurrency = CurrencyPicker.SelectedItem != null;
        bool hasValue = !string.IsNullOrWhiteSpace(ValueEntry.Text);

        ConvertButton.IsEnabled = hasInternet && hasCurrency && hasValue;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CurrencyPicker.SelectedIndexChanged += (s, e) => UpdateConvertButtonState();
        ValueEntry.TextChanged += (s, e) => UpdateConvertButtonState();
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
    }

    protected override void OnDisappearing()
    {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;

        base.OnDisappearing();
    }

    private async void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        UpdateConvertButtonState();

        if (e.NetworkAccess == NetworkAccess.Internet)
        {
            await Task.Delay(500);
            await LoadRatesAsync(DatePicker.Date);
        }
        else
        {
            if (rates.Count == 0) 
            {
                await DisplayAlert("Нет интернета", "Проверьте подключение к сети", "OK");
            }
        }
    }
}