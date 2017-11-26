using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardState
{
    Hand,
    Draw,
    Discard
}

public class Card : MonoBehaviour
{
    public float baseSpeed;
    public AudioClip moveSound;

    float speed;

    Sprite cardSprite;
    Sprite cardbackSprite;

    public string suit;
    public int rank;

    Vector3 startLerpPos;
    Quaternion startLerpRot;

    Vector3 endLerpPos;
    Quaternion endLerpRot;

    float lerp = -1.0f;

    [HideInInspector]
    public CardState state = CardState.Draw;

    [HideInInspector]
    public bool faceUp = false;

    void Start()
    {
        cardbackSprite = GetComponent<SpriteRenderer>().sprite;
    }

    public void MoveTo(Vector3 pos, Quaternion rot, float speed, bool playSound)
    {
        if(playSound)
        {
            Camera.main.GetComponent<AudioSource>().PlayOneShot(moveSound, 0.1f);
        }
        
        endLerpPos = pos;
        endLerpRot = rot;
        startLerpPos = transform.position;
        startLerpRot = transform.rotation;
        lerp = 0;
        this.speed = baseSpeed * speed;
    }

    public void MoveTo(Vector3 pos, Quaternion rot, float speed)
    {
        MoveTo(pos, rot, speed, true);
    }

    public void SetDrawOrder(int depth)
    {
        GetComponent<SpriteRenderer>().sortingOrder = depth;
    }

    public int GetDrawOrder()
    {
        return GetComponent<SpriteRenderer>().sortingOrder;
    }

    public void FlipUp()
    {
        faceUp = true;
        GetComponent<SpriteRenderer>().sprite = cardSprite;
    }

    public void FlipDown()
    {
        faceUp = true;
        GetComponent<SpriteRenderer>().sprite = cardbackSprite;
    }

    public void SetCard(int rank, string suit)
    {
        this.suit = suit;
        this.rank = rank;
        cardSprite = Deck.deck.GetCardSprite(rank, suit);
    }

    void Update()
    {
        if (lerp >= 0.0f)
        {
            lerp += speed * Time.deltaTime;

            transform.position = Vector3.Lerp(startLerpPos, endLerpPos, Mathf.SmoothStep(0, 1, lerp));

            transform.localRotation = Quaternion.Slerp(startLerpRot, endLerpRot, lerp);
            if (lerp >= 1.0f)
            {
                lerp = -1.0f;
            }
        }
    }
}
