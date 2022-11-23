using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;

public class SavingData : MonoBehaviour
{
    private string DATA_PATH = "/MyGame.dat";
    private BoardState boardState;
    private bool test;


    void Start()
    {
    }

    void SaveData(){
        FileStream file = null;


        try {
            BinaryFormatter br = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + DATA_PATH);
            BoardState bs = new BoardState();
        }
        catch (Exception e){

        }
    }
}
