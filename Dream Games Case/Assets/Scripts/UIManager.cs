using Mopsicus.InfiniteScroll;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Slider phillIndicator;

    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Image iconHolder;
    [SerializeField] private Image imageHolder;
    [SerializeField] private List<Button> buttonList;

    [SerializeField] private List<Sprite> iconList;
    [SerializeField] private List<Sprite> imageList;
    
    private GameHandler gameHandler;
    private bool control = false;
    private bool controlTwo=false;
    private bool controlThree=false;
    private bool isButtonPressed = false;


    [SerializeField] private Image _time;
    [SerializeField] private TextMeshProUGUI _timeText;
    private float _currentTime;
    public bool timerOn;
    [SerializeField] private float _duration;

    [SerializeField]
    private InfiniteScroll Scroll;

    [SerializeField] private GameObject leadboardCanvas;
    [SerializeField] private GameObject endMenuCanvas;
    [SerializeField] private GameObject startMenuCanvas;
    [SerializeField] public GameObject gameCanvas;
    [SerializeField] private TextMeshProUGUI scoreScreenText;



    void Start()
    {
        
        gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();

        phillIndicator.minValue = 0;
        phillIndicator.maxValue = 100; //Skor tutan kýsýmdaki fill ayarlarý


        
        
    }

    void Update()
    {
        scoreText.text = "" + gameHandler.scoreCount;
        phillIndicator.value = gameHandler.scoreCount; //Skor tutan kýsýmdaki fill ayarlarý
        scoreScreenText.text ="Score: "+ gameHandler.scoreCount.ToString();
        if (gameHandler.currentQuestion != null && gameHandler.gameState == GameHandler.States.PlayingState)
        {
            
            SetButtons();
        }

    }



    public void SetQuestion()
    {
        /*
         * Sorunun yapýsýna göre ui ve buttonlarý düzenliyor
         * 
         * */
        questionText.text = gameHandler.currentQuestion.question;
        switch (gameHandler.currentQuestion.category)
        {
            case "general-culture":
                iconHolder.sprite = iconList[0];
                imageHolder.sprite= imageList[0];
                break;
            case "history":
                iconHolder.sprite = iconList[1];
                imageHolder.sprite = imageList[1];
                break;
            case "music":
                iconHolder.sprite = iconList[2];
                imageHolder.sprite = imageList[2];
                break;
            case "cinema":
                iconHolder.sprite = iconList[3];
                imageHolder.sprite = imageList[3];
                break;
        }
        for(int x = 0; x < buttonList.Count; x++)
        {
            if (gameHandler.currentQuestion.choices.Count != 0)
            {
                buttonList[x].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = gameHandler.currentQuestion.choices[x];
            }
            
        }
    }
    
    private void SetButtons()
    {
        /*
         * Button atamalarý yapýlýyor
         * */
        buttonList[0].onClick.AddListener(() => {
            if(isButtonPressed==false)
            {
                gameHandler.ChoosenAnswer = gameHandler.currentQuestion.choices[0];
                ColorChange(buttonList[0]);
                isButtonPressed= true;
            }
            
        });
        buttonList[1].onClick.AddListener(() => {
            if (isButtonPressed == false)
            {
                gameHandler.ChoosenAnswer = gameHandler.currentQuestion.choices[1];
                ColorChange(buttonList[1]);
                isButtonPressed = true;
            }
            
        });
        buttonList[2].onClick.AddListener(() => {
            if (isButtonPressed == false)
            {
                gameHandler.ChoosenAnswer = gameHandler.currentQuestion.choices[2];
                ColorChange(buttonList[2]);
                isButtonPressed = true;
            }
            
        });
        buttonList[3].onClick.AddListener(() => {
            if (isButtonPressed == false)
            {
                gameHandler.ChoosenAnswer = gameHandler.currentQuestion.choices[3];
                ColorChange(buttonList[3]);
                isButtonPressed = true;
            }
            
        });
    }

    private void ColorChange(Button button)
    {
        /*
         * Cevaba göre skor ve ui atamasý yapýlýyor
         * 
         * */
        string res = gameHandler.ChoosenAnswer.Substring(0, 1);
        if (res == gameHandler.currentQuestion.answer)
        {
            button.image.color = Color.green;
            control = true;
            Invoke("CorrectAnswer", 0.2f);
            
        }
        else
        {
            button.image.color = Color.red;
            controlThree= true;
            Invoke("InCorrectAnswer", 0.2f);
        }
        
        StartCoroutine(Delayer(() => {
            button.image.color = Color.white;
            controlTwo= true;
            
            Invoke("RenewQuestion", 0.2f);

            

        },1.5f));

    }

    private IEnumerator Delayer(Action action,float time)
    {
        yield return new WaitForSecondsRealtime(time);
        action();
    }
    private IEnumerator CountdownTime()
    {
        /*
         * Timer fonksiyonu
         * */
        while (_currentTime >= 0)
        {
            timerOn = true;
            _time.fillAmount = Mathf.InverseLerp(0, _duration, _currentTime);
            _timeText.text = _currentTime.ToString();
            yield return new WaitForSeconds(1f);
            _currentTime--;
        }
        timerOn = false;
        yield return null;
    }

    private void CorrectAnswer()
    {
        if (control == true)
        {
            gameHandler.scoreCount+=10;
            control= false;
        }
        
    }
    /*
     * Update te bir kere çalýþmasý için yazýlmýþ fonksiyonlar
     * */
    private void InCorrectAnswer()
    {
        if (controlThree == true)
        {
            gameHandler.scoreCount -= 5;
            controlThree = false;
        }

    }
    private void RenewQuestion()
    {
        if (controlTwo)
        {
            gameHandler.askedQuestionList.Add(gameHandler.currentQuestion);
            gameHandler.currentQuestion = null;
            controlTwo = false;
            isButtonPressed = false;
        }
    }

    void OnFillItem(int index, GameObject item)
    {
        //Leaderboard prefabi dolduruluyor
        item.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text= gameHandler.datumList[index].rank.ToString();
        item.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = gameHandler.datumList[index].nickname;
        item.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = gameHandler.datumList[index].score.ToString();

    }

    int OnHeightItem(int index)
    {
        //prefabin büyüklüðü scripte iletiliyor
        return 200;
    }
    public void SetTimer()
    {
        /*
         * Timer bu fonksiyon çarýldýðýnda sýfýrlanýp 20den düþmeye baþlýyor
         * */
        _currentTime = _duration;
        _timeText.text = _currentTime.ToString();
        if (timerOn == false)
        {
            StartCoroutine(CountdownTime());
        }
        if (timerOn == true)
        {
            StopAllCoroutines();
            //StopCoroutine(CountdownTime());
            StartCoroutine(CountdownTime());
        }
    }

    //Aþaðýdakiler Ui larý açýp kapayan buttonlar için fonksiyonlar
    public void OpenLeaderboad()
    {
        leadboardCanvas.SetActive(true);
        Scroll.OnFill += OnFillItem;
        Scroll.OnHeight += OnHeightItem;
        Scroll.InitData(20);
    }

    public void CloseLeaderboard()
    {
        leadboardCanvas.SetActive(false);
    }

    public void OpenEndMenu()
    {
        endMenuCanvas.SetActive(true);
    }

    public void StartTheGame()
    {
        
        startMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameHandler.StartGame();
    }

    public void RepeatTheGame()
    {
        endMenuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameHandler.ResetScore();
        gameHandler.StartGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
