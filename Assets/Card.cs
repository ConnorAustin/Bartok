using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardState {
	NONE
}

public class Card : MonoBehaviour {
    public float baseSpeed;
    float speed;

	Sprite cardSprite;
	Sprite cardbackSprite;
	string suit;

	Vector3 startLerpPos;
	Quaternion startLerpRot;

	Vector3 endLerpPos;
	Quaternion endLerpRot;

	float lerp;

	public CardState state = CardState.NONE;

	[HideInInspector]
	public bool faceUp = false;

	void Start () {
		cardbackSprite = GetComponent<SpriteRenderer> ().sprite;
	}

	public void MoveTo(Vector3 pos, Quaternion rot, float speed) {
		endLerpPos = pos;
		endLerpRot = rot;
        startLerpPos = transform.position;
        startLerpRot = transform.rotation;
		lerp = 0;
        this.speed = baseSpeed * speed;
	}

    public void SetDepth(int depth) {
        GetComponent<SpriteRenderer>().sortingOrder = depth;
    }

	public void FlipUp() {
		if (!faceUp) {
			faceUp = true;
			var s = GetComponent<SpriteRenderer> ();
			s.sprite = cardSprite;
		}
	}

	public void SetCard(int rank, string suit) {
		cardSprite = Deck.deck.GetCardSprite (rank, suit);
	}
	
	void Update () {
		if (lerp >= 0.0f) {
			transform.position = Vector3.Lerp (startLerpPos, endLerpPos, Mathf.SmoothStep(0, 1, lerp));
    
			transform.localRotation = Quaternion.Slerp (startLerpRot, endLerpRot, lerp);
			lerp += speed * Time.deltaTime;
			if (lerp >= 1.0f) {
				lerp = -1.0f;
			}
		}
	}
}
