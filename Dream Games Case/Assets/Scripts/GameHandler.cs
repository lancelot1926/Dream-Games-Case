using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System;
using System.Linq;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    public int scoreCount;

    public string questiondataLink;
    public string leaderboardLink;
    public string questionjson;

    public string lbLinkOne;
    public string lbLinkTwo;
    
    public questionsData qstData;
    public leaderBoardData lbData;
    public Root rootOne;
    public Root rootTwo;
    public List<Datum> datumList=new List<Datum>();

    public List<Question> askedQuestionList;
    public Question currentQuestion=null;
    public string ChoosenAnswer;

    private bool control = false;

    public enum States
    {
        StartState,
        PlayingState,
        EndState,
    }
    public States gameState;

    private void Awake()
    {
        lbLinkOne = "https://magegamessite.web.app/case1/leaderboard_page_0.json";
        lbLinkTwo = "https://magegamessite.web.app/case1/leaderboard_page_1.json";
        questiondataLink = "https://magegamessite.web.app/case1/questions.json";
        leaderboardLink = "http://localhost:8080/leaderboard";
        StartCoroutine(GetData(questiondataLink)); //sorular ve leaderboard data indiriliyor.
        StartCoroutine(MakeRequest(lbLinkOne));
        StartCoroutine(MakeRequest(lbLinkTwo));
        /*
          Normalde leaderboard datay� a�a��daki fonksiyonla almam�z gerekiyordu ama sebebini bulamad���m bir bugdan dolay� �al��mad�.
          Ayn� localhost a ayn� y�ntemle birebir ayn� layout ile sorular� ekleyip onlar� �ekmeyi deneyince �al���yor ama leadboard bir t�rl� �al��m�yor.
          GetData2 yi kontrol ederseniz ordaki web request do�ru bir �ekilde �al���yor ve veriyi indiriyor ancak JsonConver.DeserilizeObject d�zg�n bir �ekilde atam�yor.
          Datay� list olarakta �ekmeyi �al��t�m ama o da olmad� incelemeniz i�in fonksiyonlar� oldu�u gibi b�rak�yorum. Ayr�ca sorunun ne oldu�unu s�ylerseniz �ok sevinirim.
         */
        //StartCoroutine(GetData2(leaderboardLink));
        gameState = States.StartState;
        Debug.Log(gameState);
        
    }
    void Start()
    {

    }
    private void Update()
    {     
                
        if (gameState == States.PlayingState)
        {
            PreapareQuestion();
        }

        if (uiManager.timerOn == false&& gameState == States.PlayingState)
        {

            /*
             * Verilen zaman i�inde cevap verilmezse s�radaki soruya ge�ip puan d���yor
             * 
             * */
            control= true;
            if (control==true)
            {
                scoreCount -= 3;
                askedQuestionList.Add(currentQuestion);
                currentQuestion= null;
                PreapareQuestion();
                control = false;
            }
        }
        
        if (askedQuestionList.Count == 10 && currentQuestion == null)
        {
            gameState = States.EndState;
        }

        /*
         * Soru limitine var�ld���nda oyunu durdurup end men�y� a��yor
         * */
        if (gameState== States.EndState) 
        {
            uiManager.OpenEndMenu();
            uiManager.gameCanvas.SetActive(false);
            askedQuestionList.Clear();
            

        }
              
        
    }


    IEnumerator GetData(string url)
    {
        UnityWebRequest _www = UnityWebRequest.Get(url);
        yield return _www.SendWebRequest();   //Data d�nd�kten sonra i�lemeye ba�l�yor
        if(_www.error == null ) 
        {
            qstData = JsonConvert.DeserializeObject<questionsData>(_www.downloadHandler.text);
            foreach (Question qq in qstData.questions)
            {
                for(int x=0;x<qq.choices.Count;x++)
                {
                    qq.choices[x] = qq.choices[x].Replace("\n", "");
                    
                }
                
            }

        }
        else
        {
            Debug.Log("Failed URl");
        }
    }
    IEnumerator GetData2(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();
        if (request.error == null)
        {
            lbData = JsonConvert.DeserializeObject<leaderBoardData>(request.downloadHandler.text);

            List<Player> pList = JsonUtility.FromJson<List<Player>>(request.downloadHandler.text);

        }
        else
        {
            Debug.Log("Failed URl");
        }
    }

    IEnumerator MakeRequest(string link)
    {
        UnityWebRequest request = UnityWebRequest.Get(link);
        yield return request.SendWebRequest();
        if (request.error==null)
        {
            Debug.Log("Failed URl");
        }
        else
        {
            /*
             *GetData ile ayn� �ekilde �al���yor sadece leaderboard i�in �zel atamalar� yap�yor 
             * */
            if (link == lbLinkOne)
            {
                rootOne = JsonConvert.DeserializeObject<Root>(request.downloadHandler.text);
                Debug.Log(rootOne.data[0].nickname);
                for(int x = 0; x < rootOne.data.Count; x++)
                {
                    datumList.Add(rootOne.data[x]);
                }
            }
            if(link == lbLinkTwo)
            {
                rootTwo = JsonConvert.DeserializeObject<Root>(request.downloadHandler.text);
                Debug.Log(rootTwo.data[0].nickname);
                for (int x = 0; x < rootTwo.data.Count; x++)
                {
                    datumList.Add(rootTwo.data[x]);
                }

            }
            
            
        }
    }

    public void PreapareQuestion()
    {
        /*
         * �lk ve sonraki sorular burada haz�rlan�yor.
         * Random bir �ekilde se�ilip soruluyor.
         * Cevap al�nd�ktan sonra yeni atama buradan yap�l�yor
         * 
         * */
        if (currentQuestion == null || currentQuestion.category == "")
        {
            int randomNum = UnityEngine.Random.Range(0, qstData.questions.Count);
            if(askedQuestionList.Count != 0 ) 
            {
                if (askedQuestionList.Contains(qstData.questions[randomNum]) == false)
                {
                    currentQuestion = qstData.questions[randomNum];
                }
                if(currentQuestion==null&&askedQuestionList.Count!=10)
                {
                    PreapareQuestion();
                }
            }
            else
            {
                currentQuestion = qstData.questions[randomNum];
            }
            if (currentQuestion != null)
            {
                //Zamanlay�c� ve ui aktif ediliyor
                uiManager.SetTimer();
                uiManager.SetQuestion();
            }
            
        }
        
    }
    public void StartGame()
    {
        if (qstData != null)
        {
            gameState = States.PlayingState;

        }
    }

    public void ResetScore()
    {
        scoreCount= 0;
    }


    private IEnumerator Delayer(Action action)
    {
        yield return new WaitForSecondsRealtime(1);
        action();
    }
}
