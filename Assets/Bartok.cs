using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GameState {
	
}

public class Bartok : MonoBehaviour {
	public Player[] players;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void DeckReady() {
		var cards = Deck.deck.cards;
		for (int i = 0; i < 6; i++) {
			players [0].AddCard (cards [i]);
		}
	}
}
