namespace BlackJack;

public partial class GamePage : ContentPage
{
    int score; 
    Random random = new Random();
    public GamePage()
    {
        InitializeComponent();



        int firstCard = 7;
        int secondCard = 10;
        score = firstCard + secondCard;

        PlayerScoreLabel.Text = score.ToString();
    }


    private void OnHitClicked(object sender, EventArgs e)
    {
        int newCard = random.Next(2, 11);
        score = score + newCard;
        PlayerScoreLabel.Text = score.ToString();
    }



    private void OnStandClicked(object sender, EventArgs e)
    {

    }

}