using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI.Table;
using System;
using PokerH;

public class CardController : MonoBehaviour
{
    Transform _parent;
    static public Vector3 _awayPos = new Vector3(10, 0, 0);

    Dictionary<int, CardComponent> ActiveCards { get; set; } = new Dictionary<int, CardComponent>();

    int[] _playerHands_0 = new int[4];
    int[] _playerHands_1 = new int[4];
    int[][] _playerHands = new int[2][];
    SlotController SlotCont { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerHands[0] = _playerHands_0;
        _playerHands[1] = _playerHands_1;

        SlotCont = FindFirstObjectByType<SlotController>();

        _parent = transform.Find("NewCards");

        foreach (Transform child in _parent)
        {
            Destroy(child.gameObject);
        }


        GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/CardPrefab");

        Sprite[] cardSprites = Resources.LoadAll<Sprite>("Sprites/Cards/cards_mini");
        if (cardSprites == null)
        {
            Debug.Log("Invalid cardSprites. check [Sprites/Cards/cards_h] ");
            Debug.Break();
        }

        for (int i = 0; i < Card.MAX_CARD_COUNT; i++) // 59
        {
            int c = i + 1;

            string spritePath = GetCardSpritePath(c);
            Sprite spr = Array.Find<Sprite>(cardSprites, e => e.name == spritePath);

            GameObject newCard = Instantiate(cardPrefab, _parent);
            newCard.name = $"{GetCardSpritePath(c)}_{c:d2}";
            newCard.GetComponent<SpriteRenderer>().sprite = spr;
            newCard.transform.localPosition = _awayPos;

            CardComponent cardComp = newCard.GetComponent<CardComponent>();

            ActiveCards.Add(c, cardComp);
        }
    }
    string GetCardSpritePath(int c)
    {
        string[] names = { "NN", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "ST", "SJ", "SQ", "SK", "SA",
                            "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "DT", "DJ", "DQ", "DK", "DA",
                            "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9", "HT", "HJ", "HQ", "HK", "HA",
                            "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "CT", "CJ", "CQ", "CK", "CA",
                            "JK", "JK", "JK", "JK", "JK", "JK", "JK"};


        return "cards_" + names[c];
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    CardComponent GetCardComp(int c)
    {
        if (ActiveCards.TryGetValue(c, out CardComponent card))
        {
            //card.gameObject.SetActive(true);
            return card;
        }
        return null;
    }

    SlotComponent GetBoarderSlotComp(int y, int x)
    {
        return SlotCont.GetBoarderSlot(y, x);
    }

    SlotComponent GetHandSlotComp(int playerIndex, int index)
    {
        return SlotCont.GetHandSlot(playerIndex, index);
    }


    public void OnDeal9Cards(List<int> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int col = i / 3;
            int row = i % 3;
            int card = cards[i];
            CardComponent cardComp = GetCardComp(card);
            SlotComponent slotComp = GetBoarderSlotComp(col + 1, row + 1);

            cardComp.transform.position = slotComp.transform.position;
        }
    }

    public void OnTakeCards(int playerId, int playerIndex, List<int> cards)
    {
        for (int i = 0; i < cards.Count; i++)
        {
            int card = cards[i];
            int emptyIndex = Array.IndexOf(_playerHands[playerIndex], 0);
            _playerHands[playerIndex][emptyIndex] = card;

            CardComponent cardComp = GetCardComp(card);
            SlotComponent slotComp = GetHandSlotComp(playerIndex, emptyIndex);

            cardComp.transform.position = slotComp.transform.position;
        }
    }

    public void OnPlaceCard(int playerId, int playerIndex, int toY, int toX, int card)
    {
        CardComponent cardComp = GetCardComp(card);
        SlotComponent slotComp = GetBoarderSlotComp(toY, toX);

        int handCardIndex = Array.IndexOf(_playerHands[playerIndex], card);
        _playerHands[playerIndex][handCardIndex] = 0;

        cardComp.transform.position = slotComp.transform.position;
    }
    public void OnPass(int playerId, int playerIndex)
    {

        for (int i = 0; i < _playerHands[playerIndex].Length; i++)
        {
            int card = _playerHands[playerIndex][i];
            if (card == 0)
                continue;

            CardComponent cardComp = GetCardComp(card);
            cardComp.transform.position = _awayPos;
        }

        Array.Clear(_playerHands[playerIndex], 0, _playerHands[playerIndex].Length);
    }


    public void OnCleanUp()
    {
        int ttt = _playerHands.Length;
        Array.Clear(_playerHands[0], 0, _playerHands[0].Length);
        Array.Clear(_playerHands[1], 0, _playerHands[1].Length);

        foreach (KeyValuePair<int, CardComponent> item in ActiveCards)
        {
            item.Value.transform.position = _awayPos;
        }
    }
}
