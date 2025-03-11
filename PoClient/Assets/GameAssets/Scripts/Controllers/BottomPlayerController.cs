using TMPro;
using UnityEngine;

public class BottomPlayerController : MonoBehaviour
{
    [SerializeField] TMP_Text _txtDamage0;
    [SerializeField] TMP_Text _txtDamage1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string GetDamageString(int damageRank)
    {

        //if (royalFlush)
        //    return 9;

        //bool straightFlush = IsStraightFlush(jokerCount);
        //if (straightFlush)
        //    return 8;
        //bool fourCard = IsFourCard(jokerCount);
        //if (fourCard)
        //    return 7;

        //bool fullHouse = IsFullHouse(jokerCount);
        //if (fullHouse)
        //    return 6;
        //bool flush = IsFlush(jokerCount);
        //if (flush)
        //    return 5;
        //bool straight = IsStraight(jokerCount);
        //if (straight)
        //    return 4;
        //bool triful = IsTriful(jokerCount);
        //if (triful)
        //    return 3;
        //bool twoPairs = IsTwoPairs(jokerCount);
        //if (twoPairs)
        //    return 2;
        //bool onePair = IsOnePair(jokerCount);
        //if (onePair)
        //    return 1;

        string str = "None";
        if (damageRank == 9)
            str = "Royal Flush";
        if (damageRank == 8)
            str = "Straight Flush";
        if (damageRank == 7)
            str = "Four Card";
        if (damageRank == 6)
            str = "Full House";
        if (damageRank == 5)
            str = "Flush";
        if (damageRank == 4)
            str = "Straight";
        if (damageRank == 3)
            str = "Triful";
        if (damageRank == 2)
            str = "Two Pairs";
        if (damageRank == 1)
            str = "One Pair";

        return str;

    }
    public void OnAttackDamage(int playerId, int playerIndex, int damageRank)
    {
        if (playerIndex == 0)
        {
            _txtDamage0.text = GetDamageString(damageRank);

        }
        else if (playerIndex == 1)
        {
            _txtDamage1.text = GetDamageString(damageRank);
        }
    }
    public void OnTurnStart()
    {
        _txtDamage0.text = "";
        _txtDamage1.text = "";
    }

    public void OnCleanUp()
    {
        _txtDamage0.text = "";
        _txtDamage1.text = "";
    }

    public void OnTouch()
    {
        _txtDamage0.text = "Touch";

    }
    public void OnClick()
    {
        _txtDamage1.text = "Click";

    }
}
