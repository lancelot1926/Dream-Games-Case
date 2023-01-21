using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Sorular� tutacak olan class


[System.Serializable]
public class questionsData
{
    public List<Question> questions;
}

[System.Serializable]
public class Question
{
    public string category;
    public string question;
    public List<string> choices;
    public string answer;
}



