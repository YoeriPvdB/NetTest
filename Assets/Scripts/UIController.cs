using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class UIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    [SerializeField] Slider timerSlider;
    PlayerAction pAction;
    TMP_Text healthText;
    Camera cam;
    GameObject actionUI;
    public enum UIModes
    { 
        Normal
    }

    Dictionary<int, TMP_Text> actionUIDict;

    // Start is called before the first frame update
    void Start()
    {
        
        
        //Debug.Log("this is local");
        
        //EnableSlider(false);
    }

    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;       
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            
            canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            timerSlider = canvas.gameObject.GetComponentInChildren<Slider>();


            healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TMP_Text>();
            pAction = GetComponent<PlayerAction>();
            healthText.SetText("5");
            cam = Camera.main;
            actionUI = GameObject.Find("ButtonInfo");

            actionUIDict = new Dictionary<int, TMP_Text>()
            {
                { 1, actionUI.transform.GetChild(1).GetComponent<TMP_Text>() },
                { 2, actionUI.transform.GetChild(2).GetComponent<TMP_Text>() },
                { 3, actionUI.transform.GetChild(3).GetComponent<TMP_Text>() }
            };
            //EnableSlider(false);
            
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

   

    public void ZoomCamera(float dir)
    {
        cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, dir, Time.deltaTime * 30f);

    }
    public void EnableSlider(bool status)
    {
      
        timerSlider.gameObject.SetActive(status);
        actionUI.SetActive(status);
       
    }
    public void UpdateHealth(int value)
    {
        healthText.SetText(value.ToString());
    }

    public void HighlightActionChoice(int index)
    {
        actionUIDict[index].color = Color.yellow;
    }

    public void ResetHighlights()
    {
        for(int i = 1; i < actionUIDict.Count + 1; i++)
        {
            actionUIDict[i].color = Color.white;
        }
    }

    
    public void ShowCountdownRpc()
    {
        if (canvas != null)
        {
            timerSlider.value = Mathf.MoveTowards(timerSlider.value, 1, Time.deltaTime);
            
            if(timerSlider.value > 0.95f)
            {
                timerSlider.value = 0;
                EnableSlider(false);
                pAction.StartMoveRpc("going");
                ResetHighlights();
            }
        }
    }
}
