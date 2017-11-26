using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Initializing,
    Playing,
    GameOver
}

public class Bartok : MonoBehaviour
{
    const float turnDelay = 0.5f;
    public static Bartok manager;

    public Player[] players;
    public Transform drawPilePos;
    public Transform targetPos;
    public Text turnIndicator;
    public GameState state = GameState.Initializing;

    List<Card> drawPile = new List<Card>();
    List<Card> discardPile = new List<Card>();

    Card targetCard;
    int playerTurn = -1;

    void Start()
    {
        manager = this;
    }

    void ReshuffleDeck()
    {
        drawPile = discardPile;
        discardPile = new List<Card>();

        Deck.deck.Shuffle(ref drawPile);
        drawPile.ForEach(c => c.state = CardState.Draw);
        drawPile.ForEach(c => c.FlipDown());

        ArrangeDrawPile();
    }

    public void PlayerWon()
    {
        state = GameState.GameOver;
        var player = players[playerTurn];

        if (player.type == PlayerType.player)
        {
            turnIndicator.text = "You won!";
        }
        else
        {
            turnIndicator.text = "Computer " + (playerTurn + 1) + " won.";
        }
        Invoke("ReloadGame", 3.0f);
    }

    void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool ValidMove(Card card)
    {
        return targetCard.rank == card.rank || targetCard.suit == card.suit;
    }

    public void PlayCard(Card card)
    {
        var cards = Deck.deck.cards;

        card.SetDrawOrder(targetCard.GetDrawOrder() + 1);

        discardPile.Add(targetCard);

        targetCard = card;
        card.state = CardState.Discard;
        card.FlipUp();
        card.MoveTo(targetPos.position, Quaternion.identity, 1.0f);
        Invoke("NextTurn", turnDelay);
    }

    public void DrawCard()
    {
        var card = drawPile[0];
        drawPile.RemoveAt(0);

        var player = players[playerTurn];
        player.AddCard(card);
        Invoke("NextTurn", turnDelay);

        if (drawPile.Count == 0)
        {
            ReshuffleDeck();
        }
    }

    void Update()
    {
    }

    public void NextTurn()
    {
        if (state == GameState.GameOver)
        {
            return;
        }

        if (playerTurn < 0)
        {
            playerTurn = 0;
        }
        else
        {
            players[playerTurn].myTurn = false;
            playerTurn = (playerTurn + 1) % players.Length;
        }

        turnIndicator.gameObject.SetActive(true);

        var player = players[playerTurn];
        if (player.type == PlayerType.player)
        {
            turnIndicator.text = "Your Turn";
        }
        else
        {
            turnIndicator.text = "Computer " + (playerTurn + 1) + "'s Turn";
        }

        // Notify players of turn change
        for (int i = 0; i < players.Length; i++)
        {
            players[i].TurnChange(i == playerTurn);
        }
    }

    IEnumerator RevealTarget()
    {
        var cards = Deck.deck.cards;

        targetCard.MoveTo(targetPos.position, Quaternion.identity, 1.0f);
        targetCard.FlipUp();

        yield return new WaitForSeconds(1.0f);

        state = GameState.Playing;
        NextTurn();
    }

    IEnumerator DealOutCards()
    {
        var cards = Deck.deck.cards;

        int cardIndex = 0;
        for (int c = 0; c < 7; c++)
        {
            for (int p = 0; p < 4; p++)
            {
                players[p].AddCard(cards[cardIndex]);
                drawPile.Remove(cards[cardIndex]);
                yield return new WaitForSeconds(0.2f);
                cardIndex++;
            }
        }

        targetCard = cards[cardIndex];
        drawPile.Remove(cards[cardIndex]);

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RevealTarget());
    }

    IEnumerator ArrangeDeck()
    {
        ArrangeDrawPile();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(DealOutCards());
    }

    void ArrangeDrawPile()
    {
        for (int i = 0; i < drawPile.Count; i++)
        {
            drawPile[i].transform.position = drawPilePos.position;
            drawPile[i].MoveTo(drawPilePos.position + new Vector3(-i * 0.005f, 0, i * 0.1f), Quaternion.identity, 0.5f, false);
            drawPile[i].SetDrawOrder(-i);
        }
    }

    public void DeckReady()
    {
        foreach (var c in Deck.deck.cards)
        {
            drawPile.Add(c);
        }

        StartCoroutine(ArrangeDeck());
    }
}
