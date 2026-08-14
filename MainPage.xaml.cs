namespace BlackJack;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnPlayClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GamePage());
    }
}