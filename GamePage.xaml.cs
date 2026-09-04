using Microsoft.Maui.Controls.Shapes;

namespace BlackJack;

public partial class GamePage : ContentPage
{
    int score;
    int dealerScore;
    Random random = new Random();
    public GamePage()
    {
        InitializeComponent();



        int firstCard = random.Next(2, 11);
        int secondCard = random.Next(2, 11);

        score = firstCard + secondCard;

        FirstCardLabel.Text = firstCard.ToString();
        SecondCardLabel.Text = secondCard.ToString();

        PlayerScoreLabel.Text = score.ToString();
    }


    private void OnHitClicked(object sender, EventArgs e)
    {
        var card = DrawCard();

        score += card.Points;

        AddCard(PlayerCardsLayout, card.Text, card.Color);

        PlayerScoreLabel.Text = score.ToString();

        if (score > 21)
        {
            GameStatusLabel.Text = "BUST - YOU LOSE";

            HitButton.IsEnabled = false;
            StandButton.IsEnabled = false;
        }
    }



    private void OnStandClicked(object sender, EventArgs e)
    {
        HitButton.IsEnabled = false;
        StandButton.IsEnabled = false;

        GameStatusLabel.Text = "DEALER'S TURN";

        while (dealerScore < 17)
        {
            var card = DrawCard();

            dealerScore += card.Points;

            AddCard(DealerCardsLayout, card.Text, card.Color);
        }

        DealerScoreLabel.Text = dealerScore.ToString();

        if (dealerScore > 21)
        {
            GameStatusLabel.Text = "DEALER BUST - YOU WIN";
        }
        else if (dealerScore > score)
        {
            GameStatusLabel.Text = "DEALER WINS";
        }
        else if (dealerScore < score)
        {
            GameStatusLabel.Text = "YOU WIN";
        }
        else
        {
            GameStatusLabel.Text = "PUSH";
        }
    }

    private (string Text, int Points, Color Color) DrawCard()
    {
        string[] cardValues = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        string[] suits = { "♠", "♥", "♣", "♦" };

        string cardValue = cardValues[random.Next(cardValues.Length)];
        string suit = suits[random.Next(suits.Length)];

        int points;

        if (cardValue == "J" || cardValue == "Q" || cardValue == "K")
        {
            points = 10;
        }
        else
        {
            points = int.Parse(cardValue);
        }

        Color color;

        if (suit == "♥" || suit == "♦")
        {
            color = Colors.Red;
        }
        else
        {
            color = Colors.Black;
        }

        return (cardValue + suit, points, color);
    }
    private void AddCard(HorizontalStackLayout layout, string text, Color color)
    {
        Border card = new Border();

        card.WidthRequest = 85;
        card.HeightRequest = 120;
        card.BackgroundColor = Colors.White;
        card.Stroke = Colors.Gold;
        card.StrokeThickness = 2;

        card.StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(10)
        };

        Label cardLabel = new Label();

        cardLabel.Text = text;
        cardLabel.TextColor = color;
        cardLabel.FontSize = 28;
        cardLabel.FontAttributes = FontAttributes.Bold;
        cardLabel.HorizontalTextAlignment = TextAlignment.Center;
        cardLabel.VerticalTextAlignment = TextAlignment.Center;

        card.Content = cardLabel;

        layout.Children.Add(card);
    }

}