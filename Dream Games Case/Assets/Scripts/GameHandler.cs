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
          Normalde leaderboard datayý aþaðýdaki fonksiyonla almamýz gerekiyordu ama sebebini bulamadýðým bir bugdan dolayý çalýþmadý.
          Ayný localhost a ayný yöntemle birebir ayný layout ile sorularý ekleyip onlarý çekmeyi deneyince çalýþýyor ama leadboard bir türlü çalýþmýyor.
          GetData2 yi kontrol ederseniz ordaki web request doðru bir þekilde çalýþýyor ve veriyi indiriyor ancak JsonConver.DeserilizeObject düzgün bir þekilde atamýyor.
          Datayý list olarakta çekmeyi çalýþtým ama o da olmadý incelemeniz için fonksiyonlarý olduðu gibi býrakýyorum. Ayrýca sorunun ne olduðunu söylerseniz çok sevinirim.
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
             * Verilen zaman içinde cevap verilmezse sýradaki soruya geçip puan düþüyor
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
         * Soru limitine varýldýðýnda oyunu durdurup end menüyü açýyor
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
        yield return _www.SendWebRequest();   //Data döndükten sonra iþlemeye baþlýyor
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
             *GetData ile ayný þekilde çalýþýyor sadece leaderboard için özel atamalarý yapýyor 
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
         * Ýlk ve sonraki sorular burada hazýrlanýyor.
         * Random bir þekilde seçilip soruluyor.
         * Cevap alýndýktan sonra yeni atama buradan yapýlýyor
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
                //Zamanlayýcý ve ui aktif ediliyor
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
