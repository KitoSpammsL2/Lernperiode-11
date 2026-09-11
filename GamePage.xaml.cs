using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace BlackJack;

public partial class GamePage : ContentPage
{
    private readonly Random random = new Random();

    private List<PlayingCard> deck = new List<PlayingCard>();
    private List<PlayingCard> playerHand = new List<PlayingCard>();
    private List<PlayingCard> dealerHand = new List<PlayingCard>();

    private double balance = 1000;
    private double currentBet = 0;

    private bool roundActive = false;


    public GamePage()
    {
        InitializeComponent();

        UpdateBalance();

        GameStatusLabel.Text = "SET YOUR BET";
    }


    // ------------------------------------
    // KARTENKLASSE
    // ------------------------------------

    private class PlayingCard
    {
        public string Text { get; set; } = "";
        public int Points { get; set; }
        public bool IsAce { get; set; }
        public Color Color { get; set; } = Colors.Black;
    }


    // ------------------------------------
    // NEUES KARTENDECK
    // ------------------------------------

    private void CreateDeck()
    {
        deck.Clear();

        string[] values =
        {
            "A", "2", "3", "4", "5", "6",
            "7", "8", "9", "10", "J", "Q", "K"
        };

        string[] suits =
        {
            "♠", "♥", "♣", "♦"
        };


        foreach (string suit in suits)
        {
            foreach (string value in values)
            {
                int points;
                bool isAce = false;


                if (value == "A")
                {
                    points = 11;
                    isAce = true;
                }
                else if (
                    value == "J" ||
                    value == "Q" ||
                    value == "K")
                {
                    points = 10;
                }
                else
                {
                    points = int.Parse(value);
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


                deck.Add(new PlayingCard
                {
                    Text = value + suit,
                    Points = points,
                    IsAce = isAce,
                    Color = color
                });
            }
        }
    }


    // ------------------------------------
    // EINE KARTE ZIEHEN
    // ------------------------------------

    private PlayingCard DrawCard()
    {
        if (deck.Count == 0)
        {
            CreateDeck();
        }


        int index = random.Next(deck.Count);

        PlayingCard card = deck[index];

        deck.RemoveAt(index);

        return card;
    }


    // ------------------------------------
    // SCORE BERECHNEN
    // ASS IST 1 ODER 11
    // ------------------------------------

    private int CalculateScore(List<PlayingCard> hand)
    {
        int score = 0;
        int aces = 0;


        foreach (PlayingCard card in hand)
        {
            score += card.Points;

            if (card.IsAce)
            {
                aces++;
            }
        }


        while (score > 21 && aces > 0)
        {
            score -= 10;
            aces--;
        }


        return score;
    }


    // ------------------------------------
    // PRÜFEN OB BLACKJACK
    // ------------------------------------

    private bool IsBlackjack(List<PlayingCard> hand)
    {
        return hand.Count == 2 &&
               CalculateScore(hand) == 21;
    }


    // ------------------------------------
    // KARTE VISUELL ERSTELLEN
    // ------------------------------------

    private void AddCard(
        HorizontalStackLayout layout,
        PlayingCard playingCard)
    {
        Border card = new Border();

        card.WidthRequest = 75;
        card.HeightRequest = 105;
        card.BackgroundColor = Colors.White;
        card.Stroke = Colors.Gold;
        card.StrokeThickness = 2;

        card.StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(10)
        };


        Label cardLabel = new Label();

        cardLabel.Text = playingCard.Text;
        cardLabel.TextColor = playingCard.Color;
        cardLabel.FontSize = 24;
        cardLabel.FontAttributes = FontAttributes.Bold;

        cardLabel.HorizontalTextAlignment =
            TextAlignment.Center;

        cardLabel.VerticalTextAlignment =
            TextAlignment.Center;


        card.Content = cardLabel;

        layout.Children.Add(card);
    }


    // ------------------------------------
    // VERDECKTE DEALER KARTE
    // ------------------------------------

    private void AddHiddenCard(
        HorizontalStackLayout layout)
    {
        Border card = new Border();

        card.WidthRequest = 75;
        card.HeightRequest = 105;
        card.BackgroundColor = Color.FromArgb("#173F32");
        card.Stroke = Colors.Gold;
        card.StrokeThickness = 2;

        card.StrokeShape = new RoundRectangle
        {
            CornerRadius = new CornerRadius(10)
        };


        Label label = new Label();

        label.Text = "?";
        label.TextColor = Colors.Gold;
        label.FontSize = 30;
        label.FontAttributes = FontAttributes.Bold;

        label.HorizontalTextAlignment =
            TextAlignment.Center;

        label.VerticalTextAlignment =
            TextAlignment.Center;


        card.Content = label;

        layout.Children.Add(card);
    }


    // ------------------------------------
    // SPIELERKARTEN ANZEIGEN
    // ------------------------------------

    private void RenderPlayerHand()
    {
        PlayerCardsLayout.Children.Clear();


        foreach (PlayingCard card in playerHand)
        {
            AddCard(PlayerCardsLayout, card);
        }


        PlayerScoreLabel.Text =
            CalculateScore(playerHand).ToString();
    }


    // ------------------------------------
    // DEALERKARTEN ANZEIGEN
    // ------------------------------------

    private void RenderDealerHand(bool hideSecondCard)
    {
        DealerCardsLayout.Children.Clear();


        if (dealerHand.Count == 0)
        {
            DealerScoreLabel.Text = "0";
            return;
        }


        AddCard(
            DealerCardsLayout,
            dealerHand[0]);


        if (hideSecondCard && dealerHand.Count > 1)
        {
            AddHiddenCard(DealerCardsLayout);

            DealerScoreLabel.Text =
                dealerHand[0].Points.ToString();

            return;
        }


        for (int i = 1; i < dealerHand.Count; i++)
        {
            AddCard(
                DealerCardsLayout,
                dealerHand[i]);
        }


        DealerScoreLabel.Text =
            CalculateScore(dealerHand).ToString();
    }


    // ------------------------------------
    // DEAL / NEUE RUNDE
    // ------------------------------------

    private void OnDealClicked(
        object sender,
        EventArgs e)
    {
        if (roundActive)
        {
            return;
        }


        if (!double.TryParse(
            BetEntry.Text,
            out double bet))
        {
            GameStatusLabel.Text =
                "ENTER A VALID BET";

            return;
        }


        if (bet <= 0)
        {
            GameStatusLabel.Text =
                "BET MUST BE ABOVE 0";

            return;
        }


        if (bet > balance)
        {
            GameStatusLabel.Text =
                "NOT ENOUGH CHIPS";

            return;
        }


        currentBet = bet;

        balance -= currentBet;

        UpdateBalance();


        playerHand.Clear();
        dealerHand.Clear();

        CreateDeck();


        // Spieler bekommt zwei Karten

        playerHand.Add(DrawCard());
        playerHand.Add(DrawCard());


        // Dealer bekommt zwei Karten

        dealerHand.Add(DrawCard());
        dealerHand.Add(DrawCard());


        RenderPlayerHand();
        RenderDealerHand(true);


        roundActive = true;

        HitButton.IsEnabled = true;
        StandButton.IsEnabled = true;

        DealButton.IsEnabled = false;

        GameStatusLabel.Text =
            "YOUR TURN";


        CheckStartingBlackjack();
    }


    // ------------------------------------
    // BLACKJACK DIREKT AM START
    // ------------------------------------

    private void CheckStartingBlackjack()
    {
        bool playerBlackjack =
            IsBlackjack(playerHand);

        bool dealerBlackjack =
            IsBlackjack(dealerHand);


        if (!playerBlackjack &&
            !dealerBlackjack)
        {
            return;
        }


        RenderDealerHand(false);


        if (playerBlackjack &&
            dealerBlackjack)
        {
            balance += currentBet;

            EndRound(
                "BOTH BLACKJACK - PUSH");
        }
        else if (playerBlackjack)
        {
            // Einsatz zurück + 3:2 Gewinn

            balance +=
                currentBet * 2.5;

            EndRound(
                "BLACKJACK! YOU WIN 3:2");
        }
        else
        {
            EndRound(
                "DEALER BLACKJACK - YOU LOSE");
        }
    }


    // ------------------------------------
    // HIT
    // ------------------------------------

    private void OnHitClicked(
        object sender,
        EventArgs e)
    {
        if (!roundActive)
        {
            return;
        }


        PlayingCard card =
            DrawCard();

        playerHand.Add(card);

        RenderPlayerHand();


        int playerScore =
            CalculateScore(playerHand);


        if (playerScore > 21)
        {
            RenderDealerHand(false);

            EndRound(
                "BUST - YOU LOSE");
        }
    }


    // ------------------------------------
    // STAND
    // ------------------------------------

    private void OnStandClicked(
        object sender,
        EventArgs e)
    {
        if (!roundActive)
        {
            return;
        }


        HitButton.IsEnabled = false;
        StandButton.IsEnabled = false;

        GameStatusLabel.Text =
            "DEALER'S TURN";


        DealerTurn();
    }


    // ------------------------------------
    // DEALER ZIEHT BIS MINDESTENS 17
    // ------------------------------------

    private void DealerTurn()
    {
        RenderDealerHand(false);


        int dealerScore =
            CalculateScore(dealerHand);


        while (dealerScore < 17)
        {
            dealerHand.Add(
                DrawCard());

            dealerScore =
                CalculateScore(dealerHand);
        }


        RenderDealerHand(false);

        CompareScores();
    }


    // ------------------------------------
    // GEWINNER BESTIMMEN
    // ------------------------------------

    private void CompareScores()
    {
        int playerScore =
            CalculateScore(playerHand);

        int dealerScore =
            CalculateScore(dealerHand);


        if (dealerScore > 21)
        {
            // Einsatz zurück + gleicher Betrag Gewinn

            balance +=
                currentBet * 2;

            EndRound(
                "DEALER BUST - YOU WIN");
        }
        else if (playerScore > dealerScore)
        {
            balance +=
                currentBet * 2;

            EndRound(
                "YOU WIN");
        }
        else if (playerScore < dealerScore)
        {
            // Geld wurde bereits beim Start abgezogen

            EndRound(
                "DEALER WINS");
        }
        else
        {
            // Unentschieden -> Einsatz zurück

            balance += currentBet;

            EndRound(
                "PUSH");
        }
    }


    // ------------------------------------
    // RUNDE BEENDEN
    // ------------------------------------

    private void EndRound(string message)
    {
        roundActive = false;

        HitButton.IsEnabled = false;
        StandButton.IsEnabled = false;

        DealButton.IsEnabled = true;
        DealButton.Text = "NEW ROUND";

        GameStatusLabel.Text = message;

        UpdateBalance();
    }


    // ------------------------------------
    // BALANCE ANZEIGEN
    // ------------------------------------

    private void UpdateBalance()
    {
        BalanceLabel.Text =
            $"Balance: {balance:0.##} Chips";
    }
}