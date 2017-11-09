using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {
	public GameObject cardPrefab;

	public static Deck deck;

	Dictionary<string, List<Sprite>> cardSprites = new Dictionary<string, List<Sprite>>();

	[HideInInspector]
	public List<Card> cards;

	void Awake () {
		deck = this;
		LoadCardSprites ();
	}

	void Start() {
		cards = CreateDeck ();
		Shuffle ();
		GetComponent<Prospector> ().DeckReady ();
	}

	Sprite loadCardSprite(string suit, string value) {
		return Resources.Load<Sprite> ("Sprites/card" + suit + value);
	}

	List<Sprite> LoadCardSuit(string suit) {
		List<Sprite> sprites = new List<Sprite> ();

		for(int i = 2; i < 11; i++) {
			sprites.Add(loadCardSprite(suit, i.ToString()));
		}
		sprites.Add(loadCardSprite(suit, "J"));
		sprites.Add(loadCardSprite(suit, "Q"));
		sprites.Add(loadCardSprite(suit, "K"));
		sprites.Add(loadCardSprite(suit, "A"));
		return sprites;
	}

	void LoadCardSprites() {
		cardSprites.Add("Clubs", LoadCardSuit ("Clubs"));
		cardSprites.Add("Spades", LoadCardSuit ("Spades"));
		cardSprites.Add("Hearts", LoadCardSuit ("Hearts"));
		cardSprites.Add("Diamonds", LoadCardSuit ("Diamonds"));
	}

	public Sprite getCardSprite (int rank, string suit) {
		return cardSprites [suit] [rank];
	}

	Card createCard(int rank, string suit) {
		Card c = GameObject.Instantiate (cardPrefab).GetComponent<Card>();
		c.SetCard (rank, suit);
		return c;
	}

	List<Card> CreateSuit(string suit) {
		var result = new List<Card> ();
		for(int i = 0; i < 13; i++) {
			result.Add(createCard(i, suit));
		}
		return result;
	}

	List<Card> CreateDeck() {
		var result = new List<Card> ();
		result.AddRange (CreateSuit("Clubs"));
		result.AddRange (CreateSuit("Spades"));
		result.AddRange (CreateSuit("Hearts"));
		result.AddRange (CreateSuit("Diamonds"));
		return result;
	}

	void Shuffle() {
		var newCards = new List<Card> ();
		while (cards.Count != 0) {
			int randomIndex = Random.Range (0, cards.Count);
			newCards.Add(cards[randomIndex]);
			cards.RemoveAt (randomIndex);
		}
		cards = newCards;
	}
	
	void Update () {
		
	}
}
