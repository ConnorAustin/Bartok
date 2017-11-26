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
		Shuffle (ref cards);
		SendMessage ("DeckReady");
	}

	Sprite LoadCardSprite(string suit, string value) {
		return Resources.Load<Sprite> ("Sprites/card" + suit + value);
	}

	List<Sprite> LoadCardSuit(string suit) {
		List<Sprite> sprites = new List<Sprite> ();

		for(int i = 2; i < 11; i++) {
			sprites.Add(LoadCardSprite(suit, i.ToString()));
		}
		sprites.Add(LoadCardSprite(suit, "J"));
		sprites.Add(LoadCardSprite(suit, "Q"));
		sprites.Add(LoadCardSprite(suit, "K"));
		sprites.Add(LoadCardSprite(suit, "A"));
		return sprites;
	}

	void LoadCardSprites() {
		cardSprites.Add("Clubs", LoadCardSuit ("Clubs"));
		cardSprites.Add("Spades", LoadCardSuit ("Spades"));
		cardSprites.Add("Hearts", LoadCardSuit ("Hearts"));
		cardSprites.Add("Diamonds", LoadCardSuit ("Diamonds"));
	}

	public Sprite GetCardSprite (int rank, string suit) {
		return cardSprites [suit] [rank];
	}

	Card CreateCard(int rank, string suit) {
		Card c = GameObject.Instantiate (cardPrefab).GetComponent<Card>();
		c.SetCard (rank, suit);
		return c;
	}

	List<Card> CreateSuit(string suit) {
		var result = new List<Card> ();
		for(int i = 0; i < 13; i++) {
			result.Add(CreateCard(i, suit));
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

	public void Shuffle(ref List<Card> cardsToShuffle) {
		var newCards = new List<Card> ();
		while (cardsToShuffle.Count != 0) {
			int randomIndex = Random.Range (0, cardsToShuffle.Count);
			newCards.Add(cardsToShuffle[randomIndex]);
            cardsToShuffle.RemoveAt (randomIndex);
		}
        cardsToShuffle = newCards;
    }
	
	void Update () {
		
	}
}
