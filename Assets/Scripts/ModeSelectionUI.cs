using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeSelectionUI : MonoBehaviour
{
    [Header("References")]
    public Scrollbar modeScrollbar;
    public Image displayImage; 
    public Image modeScrollBarHandle;

    [Header("Sprites for Modes")]
    public Sprite easySprite;
    public Sprite mediumSprite;
    public Sprite hardSprite;
    private int modeSelectedIndex;

    void Start()
    {
        // Add a listener so we don't have to run logic in Update()
        modeScrollbar.onValueChanged.AddListener(OnScrollValueChanged);
        
        // Initialize the first image
        OnScrollValueChanged(modeScrollbar.value);
    }

    void OnScrollValueChanged(float value)
    {
        // value ranges from 0.0 to 1.0
        if (value <= 0.33f)
        {
            displayImage.sprite = easySprite;
            displayImage.color = Color.green; 
            modeScrollBarHandle.color = Color.green;
            modeSelectedIndex =0;
        }
        else if (value > 0.33f && value <= 0.66f)
        {
            displayImage.sprite = mediumSprite;
            displayImage.color = Color.yellow;
            modeScrollBarHandle.color = Color.yellow;
            modeSelectedIndex =1;
        }
        else
        {
            displayImage.sprite = hardSprite;
            displayImage.color = Color.red;
            modeScrollBarHandle.color = Color.red;
            modeSelectedIndex =2;
        }
    }

    private void OnDestroy()
    {
        // Clean up listener
        modeScrollbar.onValueChanged.RemoveListener(OnScrollValueChanged);
    }

    public void OnPressPlay(){
        PlayerPrefs.SetInt("SelectedDifficulty",modeSelectedIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GameScene");
    }
}