using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Leaderboard datayý tutan class


public class Datum
{
    public int rank;
    public string nickname;
    public int score;
}

public class Root
{
    public int page;
    public bool is_last;
    public List<Datum> data;
}

