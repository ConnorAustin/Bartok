using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    bot,
    player
}

public class Player : MonoBehaviour
{
    const float botDecisionDelay = 0.5f;

    public PlayerType type = PlayerType.bot;
    List<Card> hand = new List<Card>();
    public bool myTurn;

    void Start()
    {

    }

    void Update()
    {
        if (type == PlayerType.bot || Bartok.manager.state != GameState.Playing)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            int cardLayer = LayerMask.GetMask("Card");
            if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000.0f, cardLayer))
            {
                var card = hit.collider.gameObject.GetComponent<Card>();
                var bartok = Bartok.manager;

                if (card.state == CardState.Draw)
                {
                    DrawCard();
                }
                else if (hand.Contains(card) && bartok.ValidMove(card))
                {
                    PlayCard(card);
                }
            }
        }
    }

    void BotMakeDecision()
    {
        var bartok = Bartok.manager;
        if(bartok.state != GameState.Playing)
        {
            return;
        }

        var validCard = hand.Find(c => bartok.ValidMove(c));
        if (validCard)
        {
            PlayCard(validCard);
        }
        else
        {
            DrawCard();
        }
    }

    void DrawCard()
    {
        Bartok.manager.DrawCard();
    }

    void PlayCard(Card card)
    {
        var bartok = Bartok.manager;

        bartok.PlayCard(card);

        hand.Remove(card);
        FanHand();

        if (hand.Count == 0)
        {
            bartok.PlayerWon();
        }
    }

    public void TurnChange(bool myTurn)
    {
        this.myTurn = myTurn;
        if (type == PlayerType.bot && myTurn)
        {
            Invoke("BotMakeDecision", botDecisionDelay);
        }
    }

    public void AddCard(Card c)
    {
        c.state = CardState.Hand;

        hand.Insert(0, c);
        FanHand();
        if (type == PlayerType.player)
        {
            c.FlipUp();
        }
    }

    public void FanHand()
    {
        int count = hand.Count;
        if (count == 0)
            return;

        float cardSquish = 1.0f;
        if (count > 7)
        {
            cardSquish = 1 + (count - 7) / 10.0f;
        }
        float angleRange = count * 0.1f / cardSquish;
        float angleStep = angleRange / (float)count;

        float baseAngle = -angleRange / 2.0f + Mathf.PI / 2.0f;

        float cardAngle = 2.0f * count;
        float cardAngleStep = cardAngle / (float)count;

        for (int i = 0; i < count; i++)
        {
            float angle = baseAngle + angleStep * i;

            Vector3 cardPos = 3.0f * Mathf.Cos(angle) * transform.right + 1.0f * Mathf.Sin(angle) * transform.up;
            var q = Quaternion.Euler(0, 0, cardAngleStep * i + -cardAngle / 2.0f + Vector3.Angle(transform.up, Vector3.up));
            hand[i].MoveTo(transform.position + cardPos * 3 + Vector3.forward * i * 0.1f, q, 1.0f);
            hand[i].SetDrawOrder(-i);
        }
    }
}
