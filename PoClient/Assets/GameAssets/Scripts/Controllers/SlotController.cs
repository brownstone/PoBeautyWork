using UnityEngine;

public class SlotController : MonoBehaviour
{
    [SerializeField] SlotComponent[] _bslots_0;
    [SerializeField] SlotComponent[] _bslots_1;
    [SerializeField] SlotComponent[] _bslots_2;
    [SerializeField] SlotComponent[] _bslots_3;
    [SerializeField] SlotComponent[] _bslots_4;

    [SerializeField] SlotComponent[] _hslots_0;
    [SerializeField] SlotComponent[] _hslots_1;

    SlotComponent[][] _bslots = new SlotComponent[5][];
    SlotComponent[][] _hslots = new SlotComponent[2][];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bslots[0] = _bslots_0;
        _bslots[1] = _bslots_1;
        _bslots[2] = _bslots_2;
        _bslots[3] = _bslots_3;
        _bslots[4] = _bslots_4;

        _hslots[0] = _hslots_0;
        _hslots[1] = _hslots_1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public SlotComponent GetBoarderSlot(int y, int x)
    {
        return _bslots[y][x];
    }

    public SlotComponent GetHandSlot(int playerIndex, int index)
    {
        return _hslots[playerIndex][index];
    }

}
