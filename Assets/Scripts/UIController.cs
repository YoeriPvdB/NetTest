using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;


public class UIController : MonoBehaviour
{
    [SerializeField] Canvas canvas;
    Slider timerSlider;
    PlayerAction pAction;
    TMP_Text healthText;
    public enum UIModes
    { 
        Normal
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
        timerSlider = canvas.gameObject.GetComponentInChildren<Slider>();
        healthText = GameObject.FindGameObjectWithTag("Health").GetComponent<TMP_Text>();
        pAction = GetComponent<PlayerAction>();
        healthText.SetText("5");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateHealth(int value)
    {
        healthText.SetText(value.ToString());
    }

    public void ShowCountdown()
    {
        if (canvas != null)
        {
            timerSlider.value = Mathf.Lerp(timerSlider.value, 1, Time.deltaTime * 2);
            if(timerSlider.value > 0.95f)
            {
                timerSlider.value = 0;
                pAction.StartMoveRpc("going");
               
            }
        }
    }
}
