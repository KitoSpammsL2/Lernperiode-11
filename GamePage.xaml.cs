using Microsoft.Maui.Controls.Shapes;

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
        string[] cardValues = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        string cardValue = cardValues[random.Next(cardValues.Length)];

        int newCard;

        if (cardValue == "J" || cardValue == "Q" || cardValue == "K")
        {
            newCard = 10;
        }
        else
        {
            newCard = int.Parse(cardValue);
        }

        string[] suits = { "♠", "♥", "♣", "♦" };
        string suit = suits[random.Next(suits.Length)];

        Border card = new Border();
        card.WidthRequest = 85;
        card.HeightRequest = 120;
        card.BackgroundColor = Colors.White;
        card.Stroke = Colors.Gold;
        card.StrokeThickness = 2;
        card.StrokeShape = new RoundRectangle
        {
            CornerRadius = 10
        };

        Label cardLabel = new Label();
        cardLabel.Text = cardValue + suit;

        if (suit == "♥" || suit == "♦")
        {
            cardLabel.TextColor = Colors.Red;
        }
        else
        {
            cardLabel.TextColor = Colors.Black;
        }

        cardLabel.HorizontalTextAlignment = TextAlignment.Center;
        cardLabel.VerticalTextAlignment = TextAlignment.Center;
        cardLabel.FontSize = 28;
        cardLabel.FontAttributes = FontAttributes.Bold;
        card.Content = cardLabel;

        PlayerCardsLayout.Children.Add(card);

        score = score + newCard;
        PlayerScoreLabel.Text = score.ToString();
      
    }



    private void OnStandClicked(object sender, EventArgs e)
    {
        HitButton.IsEnabled = false;
        StandButton.IsEnabled = false;

        GameStatusLabel.Text = "DEALER'S TURN";
    }

}