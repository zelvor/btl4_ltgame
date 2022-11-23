using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerInputs : MonoBehaviour
{
    public int column;
    public MultiplayerGameManager gm;


    private void OnMouseDown()
    {
        gm.selectColumn(column);
    }

    private void OnMouseOver()
    {
        gm.hoverColumn(column);
    }
}
