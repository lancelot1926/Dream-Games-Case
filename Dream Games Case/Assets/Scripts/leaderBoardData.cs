using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Localhost a yazd�rd���m json file � tutacak olan class

[System.Serializable]
public class leaderBoardData
{
    public List<Player> userList;
}


[System.Serializable]
public class Player
{
    public int rank;
    public string nickname;
    public int score;
}




