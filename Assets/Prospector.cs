using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Prospector : MonoBehaviour {
	public Transform drawPile;
	public Transform target;
	public Text gameOverText;
	public Text scoreText;

	static int score = 0;
	int run = 0;

	int targetIndex;
	int drawIndex;

	void Start () {
		
	}

	public void DeckReady() {
		PositionCards ();
	}

	void PositionCards() {
		var cards = Deck.deck.cards;
		int c = 0;

		float step = 2.0f;

		// Top Layer
		for (int i = 0; i < 10; i++) {
			cards [c].transform.position = new Vector3 (-9.0f + i * step, transform.position.y, transform.position.z + 0.3f);
			cards [c].GetComponent<SpriteRenderer> ().sortingOrder = 4;
			c++;
		}

		// 2nd Layer
		for (int i = 0; i < 9; i++) {
			cards [c].transform.position = new Vector3 (-8.0f + i * step, transform.position.y - 1, transform.position.z + 0.2f);
			cards [c].GetComponent<SpriteRenderer> ().sortingOrder = 3;
			cards [c].coveredBy.Add (cards[i]);
			cards [c].coveredBy.Add (cards[i + 1]);
			c++; 
		}

		// 3rd Layer
		float offset = -7.0f;
		int coverOffset = 0;
		for (int i = 0; i < 6; i++) {
			cards [c].transform.position = new Vector3 (offset, transform.position.y - 2, transform.position.z + 0.1f);
			cards [c].GetComponent<SpriteRenderer> ().sortingOrder = 2;

			cards [c].coveredBy.Add (cards[10 + coverOffset]);
			cards [c].coveredBy.Add (cards[10 + 1 + coverOffset]);

			if (i % 2 == 1) {
				offset += 2 * step;
				coverOffset += 2;
			} else {
				offset += step;
				coverOffset += 1;
			}
			c++;
		}

		// 4th Layer
		for (int i = 0; i < 3; i++) {
			cards [c].transform.position = new Vector3 (-6.0f + i * 6, transform.position.y - 3, transform.position.z);
			cards [c].GetComponent<SpriteRenderer> ().sortingOrder = 1;
			cards [c].coveredBy.Add (cards[19 + 2 * i]);
			cards [c].coveredBy.Add (cards[19 + 1 + 2 * i]);
			c++;
		}

		// Target card
		targetIndex = c;
		cards [targetIndex].transform.position = target.position;
		cards [targetIndex].Flip ();
		cards [targetIndex].state = CardState.Target;
		c++;

		// Draw pile
		drawIndex = c;
		float drawPileXOffset = 0.0f;
		while(c != cards.Count) {
			drawPileXOffset += 0.01f;
			cards [c].transform.position = new Vector3(drawPile.position.x + drawPileXOffset, drawPile.position.y, drawPile.transform.position.z);
			cards [c].state = CardState.Draw;
			cards [c].GetComponent<SpriteRenderer> ().sortingOrder = cards.Count - c;
			c++;
		}
	}

	bool AdjacentRanks(Card a, Card b) {
		if (!a.faceUp || !b.faceUp) {
			return false;
		}

		if (Mathf.Abs (a.rank - b.rank) == 1) {
			return true;
		}

		if ((a.rank == 0 && b.rank == 12) || (a.rank == 12 && b.rank == 0)) {
			return true;
		}

		return false;
	}

	void GameOver(bool won) {
		if (won) {
			gameOverText.text = "You win :)";
		} else {
			gameOverText.text = "Game Over :(";
			score = 0;
		}

		Invoke ("ReloadLevel", 3);
	}

	void ReloadLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	void CheckGameOver() {
		// If there are no cards on the table, you won
		bool won = true;
		foreach (Card c in Deck.deck.cards) {
			if (c.state == CardState.Table) {
				won = false;
			}
		}

		if (won) {
			GameOver (true);
		}

		// If there is a card still available to draw, the game isn't over
		foreach (Card c in Deck.deck.cards) {
			if (c.state == CardState.Draw)
				return;
		}

		// Check if there is still a viable play
		foreach (Card c in Deck.deck.cards) {
			if (c.state == CardState.Table && c.faceUp && AdjacentRanks(c, Deck.deck.cards[targetIndex]))
				return;
		}

		GameOver (false);
	}
	
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray.origin, ray.direction, out hit, 2000.0f, LayerMask.GetMask ("Card"))) {
				var cards = Deck.deck.cards;
				Card card = hit.collider.gameObject.GetComponent<Card> ();
				if (card.state == CardState.Draw) {
					run = 0;

					var newTarget = cards[drawIndex];

					newTarget.state = CardState.Target;
					newTarget.transform.position = target.position;
					newTarget.Flip ();

					cards [targetIndex].state = CardState.Discarded;
					cards [targetIndex].transform.position = new Vector3 (9999, 9999, 9999);

					targetIndex = drawIndex;
					drawIndex++;
					CheckGameOver ();
				} else if (card.state == CardState.Table) {
					if (AdjacentRanks (card, cards [targetIndex])) {
						run++;
						score += run;
						scoreText.text = "" + score;

						cards [targetIndex].state = CardState.Discarded;
						cards [targetIndex].transform.position = new Vector3 (9999, 9999, 9999);
						
						targetIndex = cards.IndexOf (card);
						card.transform.position = target.position;
						cards [targetIndex].state = CardState.Target;
						CheckGameOver ();
					}
				}
			}
		}
	}
}
