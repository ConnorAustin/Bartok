using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState {
	
}

public class Bartok : MonoBehaviour {
	public Player[] players;

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

	public void DeckReady() {
		
	}
}
