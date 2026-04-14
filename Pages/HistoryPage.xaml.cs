namespace Lab1.Pages {

    public partial class HistoryPage : ContentPage
    {
        public HistoryPage()
        {
            InitializeComponent();
            LoadHistory();
        }

        private void LoadHistory()
        {
            string savedHistory = Preferences.Default.Get("CalcHistory", "");
            HistoryDisplay.Text = string.IsNullOrWhiteSpace(savedHistory)
                ? "История пуста"
                : savedHistory;
        }

        private void OnClearHistoryClicked(object sender, EventArgs e)
        {
            MainPage.ClearHistory();
            HistoryDisplay.Text = "История пуста";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadHistory();
        }
    }
}