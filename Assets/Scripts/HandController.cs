using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum HandType
{
	Player,
	Opponent
}

public class HandController : MonoBehaviour {

	DeckController deckController;
	DeckView deckView;
	public List<GameCard> gameCards;
	HandType handOwner;
    int HandTotalValue;
	
	// Use this for initialization

	public void Initialize(DeckController controller, HandType handType)
	{
		deckController = controller;
		handOwner = handType;
		deckView = deckController.gameObject.GetComponent<DeckView>();
		gameCards = new List<GameCard> ();
	}

	public IEnumerator Deal()
	{
		if (gameCards.Count < 2) 
		{
			while(gameCards.Count < 2)
			{
                if (this.handOwner == HandType.Opponent && gameCards.Count != 0)
                    yield return StartCoroutine(DealCardToHand(
                                        (GameCard gc) =>
                                        {
                                            gameCards.Add(gc);
                                            gc.transform.parent = transform;
                                        },
                                        false
                                    ));
                else yield return StartCoroutine(DealCardToHand( 
					(GameCard gc) =>
				    {
						gameCards.Add(gc);
						gc.transform.parent = transform;
					}
				));
			
			}
		}
	}

	public IEnumerator DealCardToHand(Action<GameCard> cardAnimComplete, bool faceUp = true)
	{
		GameCard dealtCard = deckView.DrawCard();
		switch(handOwner)
		{
			case HandType.Player:
				dealtCard.transform.positionTo(.25f,transform.position + new Vector3(10*gameCards.Count,0,0));
				break;
			case HandType.Opponent:
				Vector3 vec = new Vector3(0,180,0);
                if (faceUp == false)
                    dealtCard.transform.localRotation = Quaternion.Euler(vec);
                dealtCard.transform.positionTo(.25f, transform.position + new Vector3(10 * gameCards.Count, 0, 0));
				break;
		}
		yield return new WaitForSeconds(.25f);
		cardAnimComplete(dealtCard);
        HandTotalValue += dealtCard.Model.GetBlackJackCardValue();
        Debug.Log(this.handOwner.ToString() + " " + HandTotalValue);
    }

	public void AttackHand(HandController defender, GameCard attackCard)
	{
		gameCards.Remove(attackCard);
		defender.RecieveAttack(attackCard);
	}

	public void RecieveAttack(GameCard attackCard)
	{
		StartCoroutine(coRecieveAttack(attackCard));
	}

	IEnumerator coRecieveAttack(GameCard attackCard)
	{
		//attackCard
		yield return new WaitForSeconds(.25f);
	}

	public void ActivateCardInput(Action<GameCard> onCardSelected)
	{
		foreach(GameCard card in gameCards)
		{
			Debug.Log("Card Activated: " + card.name);
			card.GetComponent<SimpleButton>().OnClick += (SimpleButton btn) =>
			{
				onCardSelected(btn.gameObject.GetComponent<GameCard>());
			};
		}
	}

	public void DisableCardInput()
	{
		foreach(GameCard card in gameCards)
		{
			card.GetComponent<SimpleButton>().OnClick = null;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
