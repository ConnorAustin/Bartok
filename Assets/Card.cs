using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardState {
	DrawPile,
	ToHand,
	Hand,
	ToTarget,
	Target,
	Discarded,
	To,
	Idle
}

public class Card : MonoBehaviour {
	Sprite cardSprite;
	Sprite cardbackSprite;
	string suit;

	[HideInInspector]
	public int rank; // From 0 (which is a 2) upwards to Ace

	[HideInInspector]
	public List<Card> coveredBy = new List<Card> ();

	public CardState state = CardState.Table;

	[HideInInspector]
	public bool faceUp = false;

	void Start () {
		cardbackSprite = GetComponent<SpriteRenderer> ().sprite;
	}

	public void Flip() {
		faceUp = !faceUp;
		var s = GetComponent<SpriteRenderer> ();
		if (faceUp) {
			s.sprite = cardSprite;
		} else {
			s.sprite = cardbackSprite;
		}
	}

	public void SetCard(int rank, string suit) {
		this.rank = rank;
		this.suit = suit;
		cardSprite = Deck.deck.getCardSprite (rank, suit);
	}
	
	// Update is called once per frame
	void Update () {
		int covered = coveredBy.Count;
		foreach (var c in coveredBy) {
			if (c.state != CardState.Table) {
				covered--;
			}
		}
		if (!faceUp && covered == 0 && state == CardState.Table) {
			Flip ();
		}
	}
}
