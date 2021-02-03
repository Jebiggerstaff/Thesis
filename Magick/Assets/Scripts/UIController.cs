using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public class UIController : MonoBehaviour
{
    #region Gameobjects
    public GameObject firstPersonGroup;
    public GameObject PauseMenu;
    public GameObject spellDisplay;
    public GameObject HUD;
    public GameObject SpellbookUI;
    #endregion

    public int spellsknown=0;

    public GameObject[] SpellSlots;

    #region Buttons
    public Button PauseToGameButton;
    public Button PauseToOptionsButton;
    public Button PauseToCreditsButton;
    public Button PauseToMainButton;
    #endregion

    #region KnowsSpells
    bool knowsFireball = false;
    bool knowsTelekinesis = false;
    bool knowsResize = false;
    bool knowsConjureCrate = false;
    bool knowsPush = false;
    bool knowsPull = false;
    bool knowsPolymorph = false;
    bool knowsDuplicate = false;
    #endregion

    bool InOptionsMenu = false;

    private MagickPathToMastery controls;

    private void Awake()
    {
        controls = new MagickPathToMastery();
        controls.Enable();
    }

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       
        PauseToGameButton.onClick.AddListener(PauseToGame);
        PauseToOptionsButton.onClick.AddListener(PauseToOptions);
        PauseToCreditsButton.onClick.AddListener(PauseToCredits);
        PauseToMainButton.onClick.AddListener(PauseToMain);


    }
     
    private void Update()
    {
        //open pause menu
        if (controls.UI.PauseButton.triggered && InOptionsMenu == false)
        {
            InOptionsMenu = true;
            firstPersonGroup.GetComponent<FirstPersonAIO>().playerCanMove = false;
            firstPersonGroup.GetComponent<FirstPersonAIO>().enableCameraMovement = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            HUD.SetActive(false);
            PauseMenu.SetActive(true);
            spellDisplay.GetComponent<SpellDisplay>().Clear();
        }
        //close pause menu
        else if (controls.UI.PauseButton.triggered && InOptionsMenu == true)
        {
            InOptionsMenu = false;
            firstPersonGroup.GetComponent<FirstPersonAIO>().playerCanMove = true;
            firstPersonGroup.GetComponent<FirstPersonAIO>().enableCameraMovement = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            HUD.SetActive(true);
            PauseMenu.SetActive(false);
        }
    }

    void PauseToGame()
    {
        InOptionsMenu = false;
        firstPersonGroup.GetComponent<FirstPersonAIO>().playerCanMove = true;
        firstPersonGroup.GetComponent<FirstPersonAIO>().enableCameraMovement = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        HUD.SetActive(true);
        PauseMenu.SetActive(false);
    }

    void PauseToOptions()
    {
        Debug.Log("open Options");
    }

    void PauseToCredits()
    {
        Debug.Log("open Credits");
    }

    void PauseToMain()
    {
        Application.Quit();
    }
    public void AddToSpellbook(string spellname)
    {
        

        switch (spellname)
        {
            #region Fireball
            case "qqq":
                if (knowsFireball == false)
                {
                    knowsFireball = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Fireball: + , + , +";
                    spellsknown++;
                }

                break;
            #endregion
            #region Telekinesis
            case "eee":
                if (knowsTelekinesis == false)
                {
                    knowsTelekinesis = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Telekinesis: - , - , -";
                    spellsknown++;
                }

                break;
            #endregion
            #region Polymorph
            case "qqe":
                if (knowsPolymorph == false)
                {
                    knowsPolymorph = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Polymorph: - ,  - ,  +";
                    spellsknown++;
                }

                break;
            #endregion
            #region Conjure Crate
            case "eqq":
                if (knowsConjureCrate == false)
                {
                    knowsConjureCrate = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Conjure Crate: - , + , +";
                    spellsknown++;
                }

                break;
            #endregion
            #region Push
            case "qee":
                if (knowsPush == false)
                {
                    knowsPush = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Push: + , - , -";
                    spellsknown++;
                }
                break;
            #endregion
            #region Pull
            case "qeq":
                if (knowsPull == false)
                {
                    knowsPull = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Pull: + , - , +";
                    spellsknown++;
                }
                break;
            #endregion
            #region Resize
            case "eeq":
                if (knowsResize == false)
                {
                    knowsResize = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Resize: - , - , +";
                    spellsknown++;
                }
                break;
            #endregion
            #region Duplicate
            case "eqe":
                if (knowsDuplicate == false)
                {
                    knowsDuplicate = true;
                    SpellSlots[spellsknown].GetComponent<Text>().text = "Duplicate: - , + , -";
                    spellsknown++;
                }
                break;
            #endregion

            default:
                break;
        }
    }
}
