using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState {
	
}

public class Bartok : MonoBehaviour {
	public Player[] players;
    public Transform drawPilePos;
    public Transform targetPos;

    int targetIndex = 0;
    int drawIndex = 0;

	void Start () {
		
	}

    int xx = 0; int y = 0;

    void Update () {
        var cards = Deck.deck.cards;
        if(Input.GetKeyDown(KeyCode.Space))
        {
            players[y].AddCard(cards[xx]);
            y++;
            y %= 4;
            xx++;
        }
    }

    IEnumerator RevealTarget()
    {
        var cards = Deck.deck.cards;

        cards[targetIndex].MoveTo(targetPos.position, Quaternion.identity, 1.0f);
        cards[targetIndex].FlipUp();
        yield return new WaitForSeconds(1.0f);
        // StartTurn();
    }

    IEnumerator DealOutCards() {
        var cards = Deck.deck.cards;

        int cardIndex = 0;
        for (int c = 0; c < 7; c++)
        {
            for(int p = 0; p < 4; p++) {
                players[p].AddCard(cards[cardIndex]);
                yield return new WaitForSeconds(0.2f);
                cardIndex++;
            }
        }
        drawIndex = cardIndex + 1;
        targetIndex = cardIndex;

        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RevealTarget());
    }

    IEnumerator ArrangeDrawPile()
    {
        var cards = Deck.deck.cards;

        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.position = drawPilePos.position;
            cards[i].MoveTo(drawPilePos.position + new Vector3(-i * 0.005f, 0, i * 0.1f), Quaternion.identity, 0.5f);
            cards[i].SetDepth(-i);
        }
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(DealOutCards());
    }

    public void DeckReady() {
        StartCoroutine(ArrangeDrawPile());
	}
}
