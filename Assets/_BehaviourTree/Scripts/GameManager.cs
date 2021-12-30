using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] ChickenManager chickenManager;

    [Header("Camera")]
    [SerializeField] CinemachineFreeLook cinemachineFreeLookCamera;

    [Header("Animators")]
    [SerializeField] Animator inGameUI;
    [SerializeField] Animator mainMenuUI;
    [SerializeField] Animator postProcessingAnim;
    [SerializeField] Animator gameWon;
    [SerializeField] Animator gameLost;

    [Header("Texts")]
    [SerializeField] GameObject objectiveText;
    [SerializeField] GameObject objectiveSubText;

    [SerializeField] TextMeshProUGUI chickensDeadTextWon;
    [SerializeField] TextMeshProUGUI chickensFedTextWon;

    [SerializeField] TextMeshProUGUI chickensDeadTextLost;
    [SerializeField] TextMeshProUGUI chickensFedTextLost;

    [Header("Music")]
    [SerializeField] AudioSource bgMusicSource;

    [SerializeField] AudioClip[] bgMusicClips;

    [SerializeField] TextMeshProUGUI[] trackTexts;
    [SerializeField] Outline[] trackOutlines;

    [SerializeField] Color selectedColor;
    [SerializeField] Color unSelectedColor;

    private bool hasStarted = false;

    private float tempXCameraSpeed;
    private float tempYCameraSpeed;

    private void Awake()
    {
        tempXCameraSpeed = cinemachineFreeLookCamera.m_XAxis.m_MaxSpeed;
        tempYCameraSpeed = cinemachineFreeLookCamera.m_YAxis.m_MaxSpeed;

        cinemachineFreeLookCamera.m_XAxis.m_MaxSpeed = 0f;
        cinemachineFreeLookCamera.m_YAxis.m_MaxSpeed = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        gameWon.gameObject.SetActive(false);
        gameLost.gameObject.SetActive(false);

        EventSystemNew.Subscribe(Event_Type.GAME_WON, GameWon);
        EventSystemNew.Subscribe(Event_Type.GAME_LOST, GameLost);
    }

    private void Update()
    {
        if (hasStarted)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                objectiveText.SetActive(!objectiveText.activeInHierarchy);
                objectiveSubText.SetActive(!objectiveSubText.activeInHierarchy);
            }
        }
    }

    public void ChangeTrack(int _trackID)
    {
        if (bgMusicClips[_trackID] != null && bgMusicSource.clip != bgMusicClips[_trackID])
        {
            foreach (var trackText in trackTexts)
            {
                trackText.color = unSelectedColor;
            }

            foreach (var trackOutline in trackOutlines)
            {
                trackOutline.effectColor = unSelectedColor;
            }

            trackTexts[_trackID].color = selectedColor;
            trackOutlines[_trackID].effectColor = selectedColor;

            bgMusicSource.clip = bgMusicClips[_trackID];
            bgMusicSource.Play();
        }
    }

    public void StartGame()
    {
        if (!hasStarted)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            inGameUI.SetBool("FadeIn", true);
            inGameUI.SetBool("FadeOut", false);

            mainMenuUI.SetBool("FadeOut", true);
            postProcessingAnim.SetBool("BlurOut", true);

            cinemachineFreeLookCamera.m_XAxis.m_MaxSpeed = tempXCameraSpeed;
            cinemachineFreeLookCamera.m_YAxis.m_MaxSpeed = tempYCameraSpeed;

            hasStarted = true;

            EventSystemNew.RaiseEvent(Event_Type.START_GAME);
        }
    }

    private void GameWon()
    {
        gameWon.gameObject.SetActive(true);

        chickensFedTextWon.text = "<color=#00FF0D>You</color> Fed <color=#FF4545>" + chickenManager.GetChickensFed() + "</color><color=#FFE500> Chicken(s)</color><color=#FF4545>!</color>";
        chickensDeadTextWon.text = "<color=#FFE500>Only</color> <color=#FF4545>" + chickenManager.GetChickensDied() + "</color> <color=#00FF0D>Chicken(s)</color><color=#FFE500> Died</color><color=#FF4545>!</color>";

        GameEnded();

        gameWon.SetBool("FadeIn", true);
    }

    private void GameLost()
    {
        gameLost.gameObject.SetActive(true);

        chickensFedTextLost.text = "<color=#00FF0D>Only</color> <color=#FF4545>" + chickenManager.GetChickensFed() + "</color><color=#FFE500> Chicken(s)</color> <color=#FF70EF>were</color> <color=#FFE500>Fed</color><color=#FF4545>!</color>";
        chickensDeadTextLost.text = "<color=#FF4545>" + chickenManager.GetChickensDied() + "</color> <color=#00FF0D>Chicken(s)</color><color=#FFE500> Died</color><color=#FF4545>!</color>";

        GameEnded();

        gameLost.SetBool("FadeIn", true);
    }

    private void GameEnded()
    {
        hasStarted = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        cinemachineFreeLookCamera.m_XAxis.m_MaxSpeed = 0f;
        cinemachineFreeLookCamera.m_YAxis.m_MaxSpeed = 0f;

        postProcessingAnim.SetBool("BlurIn", true);
        postProcessingAnim.SetBool("BlurOut", false);

        inGameUI.SetBool("FadeOut", true);
        inGameUI.SetBool("FadeIn", false);
    }

    public void LoadScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
