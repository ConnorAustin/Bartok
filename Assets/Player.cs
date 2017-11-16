using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType {
	bot,
	player
}

public class Player : MonoBehaviour {
	public PlayerType type = PlayerType.bot;
	List<Card> hand = new List<Card>();

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void AddCard(Card c) {
		hand.Add (c);
		FanHand ();
		if (type == PlayerType.player) {
			c.FlipUp ();
		}
	}

	public void FanHand() {
		int count = hand.Count;

		float angleStep = 0.4f;

		float baseAngle = Mathf.PI / 2.0f + Vector3.Angle (transform.up, Vector3.up);

		for(int i = 0; i < count; i++) {
			float angle = baseAngle + -1 * (i % 2) * i * angleStep;
			Vector3 cardLook = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle), 0);

			var q = Quaternion.LookRotation(Vector3.forward, new Vector3(cardLook.x, cardLook.z, 0));
			hand [i].MoveTo (transform.position + cardLook * 3, q);
			if(count == 6)
				Debug.DrawLine (transform.position, transform.position + cardLook, Color.red, 200.0f);
		}
	}
}
