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
    public GameObject spellDisplay;
    public GameObject HUD;
    public GameObject SpellbookUI;
    #endregion

    public int spellsknown=0;

    public GameObject[] SpellSlots;

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
