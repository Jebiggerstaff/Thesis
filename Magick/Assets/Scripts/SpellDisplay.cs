using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;


public class SpellDisplay : MonoBehaviour
{
    public GameObject fireball;
    public GameObject Crate;
    public GameObject Ball;
    public GameObject pushBox;
    public GameObject pullBox;
    public GameObject player;
    public GameObject playerCamera;
    public GameObject spellTarget;
    public GameObject spellGuide;
    public GameObject UI;
    private GameObject spawnedCrate = null;

    public Text SpellProgressText;

    #region UISpellGameobjects
    public GameObject fireballUI;
    public GameObject telekinesisUI;
    public GameObject polymorphUI;
    public GameObject ConjureCrateUI;
    public GameObject PushUI;
    public GameObject PullUI;
    public GameObject ResizeUI;
    public GameObject DuplicateUI;
    #endregion

    Transform unchild;
    Text text; //The text object used to display spells on screen
    string activeSpell = null; //Determines if there is a spell effect currently active
    string input = ""; //Current spell input
    string displayText = ""; //Used in conjunction with Text text
    public float ForceMod = 1000f; //Used to move objects in telekinesis
    public float spellLongevity = 5f; //Length of spell effects
    float targetDistance; //Distance between target and object (Telekinesis)
    bool playerTouching; //Determines if player is touching object that is being targeted
    Vector3 move; //Used to move objects in telekinesis
    double spellEffectsTimer = 0; //Used to countdown how long a spell lasts
    Material mat = null;//Used for telekinesis to get material of spellTarget;
    Vector3 polyTemp;   

    private MagickPathToMastery controls;

    void Start()
    {
        text = GetComponent<Text>();
    }
    private void Awake()
    {
        controls = new MagickPathToMastery();
        controls.Enable();
    }


    void Update()
    {
        //Detects Spell Inputs
        if (activeSpell == null)
        {
            if (displayText.Length == 3)
                input = "";
            else if (controls.Player._1stSpellKey.triggered)
                input = "q";
            else if (controls.Player._2ndSpellKey.triggered)
                input = "e";

            if (input == "q" || input == "e")
            {

                //ensures spells only go to 3 commands
                if (displayText.Length < 3)
                {
                    //Display rune
                    switch (input)
                    {
                        case "q":
                            if (displayText.Length == 0)
                                SpellProgressText.text += "Positive";
                            else if (displayText.Length == 1)
                                SpellProgressText.text += " -- Positive";
                            else if (displayText.Length == 2)
                                SpellProgressText.text += " -- Positive";
                            break;

                        case "e":
                            if (displayText.Length == 0)
                                SpellProgressText.text += "Negative";
                            else if (displayText.Length == 1)
                                SpellProgressText.text += " -- Negative";
                            else if (displayText.Length == 2)
                                SpellProgressText.text += " -- Negative";
                            break;

                        default:
                            break;
                    }
                    displayText += input;
                    input = "";
                }
            }
        
        }
        
        //Detects if spell timer is up or the user is ending a spell
        if ((spellEffectsTimer <= 0 || controls.Player.CancelSpell.triggered) && activeSpell != null)
        {
            Clear();
            activeSpell = null;
            spellEffectsTimer = 0;
        }

        //Detects if user is clearing their spell
        else if (controls.Player.CancelSpell.triggered)
        {
            Clear();
            SpellProgressText.text = "";
        }

        //Counts down spell timer
        else if (spellEffectsTimer > 0)
        {
            //finds if the object has been destroyed
            if (spellTarget == null)
            {
                activeSpell = null;
            }

            spellEffectsTimer -= Time.deltaTime;
        }

        //Activates spell effects
        if (spellEffectsTimer > 0 && activeSpell != null)
        {
            switch (activeSpell)
            {
                #region TELEKINESIS
                case "TELEKINESIS":
                    targetDistance = Vector3.Distance(spellGuide.transform.position, spellTarget.transform.position);
                    ForceMod = 500;
                    ForceMod = ForceMod * targetDistance;
                    if (targetDistance >= .2 && playerTouching == false)
                    {
                        spellTarget.GetComponent<Rigidbody>().velocity = spellTarget.GetComponent<Rigidbody>().velocity / 4f;
                        spellTarget.GetComponent<Rigidbody>().AddForce((spellGuide.transform.position - spellTarget.transform.position).normalized * (ForceMod) * Time.smoothDeltaTime, mode: ForceMode.Impulse);
                    }
                    else if (playerTouching == true)
                    {
                        spellTarget.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                    }
                    else if (targetDistance <= .15 && spellTarget.GetComponent<Rigidbody>().velocity.x <= .2f && spellTarget.GetComponent<Rigidbody>().velocity.y <= .2f && spellTarget.GetComponent<Rigidbody>().velocity.z <= .2f)
                    {
                        spellTarget.GetComponent<Rigidbody>().velocity += new Vector3(Random.Range(-.05f, .05f), Random.Range(-.05f, .05f), Random.Range(-.05f, .05f));
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }

        //Detects mouse wheel scroll, and then applies it to a spell if possible
        if (controls.Player.Scroll.ReadValue<float>() != 0f)
        {

            switch (activeSpell)
            {
                #region TELEKINESIS
                case "TELEKINESIS":

                    move = spellGuide.transform.localPosition;
                    if (controls.Player.Scroll.ReadValue<float>() <= 0 && Vector3.Distance(spellGuide.transform.position, player.transform.position) > 10)
                    {
                        if (controls.Player.Scroll.ReadValue<float>() < -1)
                            move.z -= 1f;
                        else
                            move.z -= .01f;
                        spellGuide.transform.localPosition = move;   
                    }
                    else if(controls.Player.Scroll.ReadValue<float>() >= 0 && Vector3.Distance(spellGuide.transform.position, player.transform.position) < 30)
                    {
                        if (controls.Player.Scroll.ReadValue<float>() > 1)
                            move.z += 1f;
                        else
                            move.z += .01f;
                        spellGuide.transform.localPosition = move;
                    }

                    break;
                #endregion
                #region RESIZE
                case "RESIZE":
                    if(spellTarget.transform.lossyScale.magnitude <= polyTemp.magnitude*1.5 && controls.Player.Scroll.ReadValue<float>() >= 0)
                    {
                        if (controls.Player.Scroll.ReadValue<float>() > 1)
                            spellTarget.transform.localScale += new Vector3(.1f, .1f, .1f);
                        else
                            spellTarget.transform.localScale += new Vector3(.001f, .001f, .001f);
                    }
                    else if (spellTarget.transform.lossyScale.magnitude >= polyTemp.magnitude * .5 && controls.Player.Scroll.ReadValue<float>() <= 0)
                    {
                        if (controls.Player.Scroll.ReadValue<float>() < -1)
                            spellTarget.transform.localScale -= new Vector3(.1f, .1f, .1f);
                        else
                            spellTarget.transform.localScale -= new Vector3(.001f, .001f, .001f);
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }

        //Displays spells
        if (displayText.Length == 3 && activeSpell == null)
        {
            switch (displayText)
            {
                #region Fireball
                case "qqq":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    fireballUI.SetActive(true);

                    break;
                #endregion
                #region Telekinesis
                case "eee":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    if (activeSpell == null)
                    {
                        telekinesisUI.SetActive(true);
                    }
                    break;
                #endregion
                #region Resize
                case "eeq":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    ResizeUI.SetActive(true);
                    break;
                #endregion
                #region Conjure Crate
                case "eqq":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    ConjureCrateUI.SetActive(true);
                    break;
                #endregion
                #region Push
                case "qee":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    PushUI.SetActive(true);
                    break;
                #endregion
                #region Pull
                case "qeq":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    PullUI.SetActive(true);
                    break;
                #endregion
                #region Polymorph
                case "qqe":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    if (activeSpell == null)
                    {
                        polymorphUI.SetActive(true);
                    }
                    break;
                #endregion
                #region Duplicate
                case "eqe":
                    UI.GetComponent<UIController>().AddToSpellbook(displayText);
                    if (activeSpell == null)
                    {
                        DuplicateUI.SetActive(true);
                    }
                    break;
                #endregion
                default:
                    break;
            }
        }

        //Activates Spells
        if (controls.Player.Fire.triggered && displayText.Length == 3 && activeSpell == null)
        {

            int layerMask = 0 << 8;
            RaycastHit hit;

            switch (displayText)
            {
                #region Fireball
                case "qqq":
                    layerMask = ~layerMask;
                    if (Physics.Raycast(new Ray(playerCamera.transform.position, playerCamera.transform.forward), out hit, 30f, layerMask))
                    {
                        spawnedCrate = Instantiate(fireball, hit.point, transform.rotation);
                    }
                    else
                        spawnedCrate = Instantiate(fireball, spellGuide.transform.position, transform.rotation);

                    //Instantiate(fireball, new Vector3(playerCamera.transform.position.x, playerCamera.transform.position.y + .1f, playerCamera.transform.position.z), playerCamera.transform.rotation);
                    fireballUI.SetActive(false);
                    displayText = "";
                    break;
                #endregion
                #region Telekinesis
                case "eee":
                    if (activeSpell == null)
                    {
                        displayText = "";
                        telekinesisUI.SetActive(false);
                        getTarget();
                        //Determines if object is allowed to be targeted
                        if (spellTarget.GetComponent<Rigidbody>() != null && spellTarget != player)
                        {
                            //Determines if object has the renderer or if its children do, then lights the renderer up
                            if (spellTarget.GetComponent<Renderer>() != null)
                            {
                                mat = spellTarget.GetComponent<Renderer>().material;
                                mat.EnableKeyword("_UseEmissiveIntensity");
                                mat.SetColor("_EmissiveColor", new Color(1, 1, 1, 1) * .2f);
                            }
                            else if (spellTarget.GetComponentsInChildren<Renderer>() != null)
                            {
                                mat = spellTarget.GetComponentInChildren<Renderer>().material;
                                mat.EnableKeyword("_UseEmissiveIntensity");
                                mat.SetColor("_EmissiveColor", new Color(1, 1, 1, 1) * .2f);
                            }
                            else
                            {
                                displayText = "";
                            }
                            //activates effects of telekinesis
                            spellTarget.GetComponent<Rigidbody>().useGravity = false;
                            spellEffectsTimer = spellLongevity * 10;
                            activeSpell = "TELEKINESIS";
                        }
                    }
                    break;
                #endregion
                #region Resize
                case "eeq":
                    displayText = "";
                    ResizeUI.SetActive(false);
                    getTarget();

                    polyTemp = spellTarget.transform.lossyScale;

                    if (spellTarget.GetComponent<Rigidbody>() != null && spellTarget != player)
                    {
                        spellEffectsTimer = spellLongevity * 10;
                        activeSpell = "RESIZE";

                    }
                    else
                    {
                        displayText = "";
                        activeSpell = "";
                    }
                    break;
                #endregion
                #region Conjure Crate
                case "eqq":
                    if (activeSpell == null)
                    {
                        displayText = "";
                        ConjureCrateUI.SetActive(false);
                        layerMask = ~layerMask;
                        if (Physics.Raycast(new Ray(playerCamera.transform.position, playerCamera.transform.forward), out hit, 9.9f, layerMask))
                        {
                            spawnedCrate= Instantiate(Crate, hit.point, transform.rotation);
                        }
                        else
                            spawnedCrate = Instantiate(Crate, spellGuide.transform.position, transform.rotation);
                    }
                    break;
                #endregion
                #region Push
                case "qee":
                    spawnedCrate = Instantiate(pushBox, transform.position, transform.rotation);
                    PushUI.SetActive(false);
                    displayText = "";
                    break;
                #endregion
                #region Pull
                case "qeq":
                    spawnedCrate = Instantiate(pullBox, transform.position, transform.rotation);
                    PullUI.SetActive(false);
                    displayText = "";
                    break;
                #endregion
                #region Polymorph
                case "qqe":
                    polymorphUI.SetActive(false);
                    getTarget();
                    
                    if (spellTarget.GetComponent<Rigidbody>() != null && spellTarget != player)
                    {
                        if (spellTarget.name == "Cube" ||spellTarget.name=="Cube(Clone)")
                            Instantiate(Ball, spellTarget.transform.position, spellTarget.transform.rotation);
                        else
                            Instantiate(Crate, spellTarget.transform.position,spellTarget.transform.rotation);

                        Destroy(spellTarget);
                    }
                    displayText = "";
                    break;
                #endregion
                #region Duplicate
                case "eqe":
                    DuplicateUI.SetActive(false);
                    getTarget();

                    if (spellTarget.GetComponent<Rigidbody>() != null && spellTarget != player)
                    {
                        string name = spellTarget.name; 
                        GameObject clone = Instantiate(spellTarget, spellTarget.transform.position, spellTarget.transform.rotation);
                        clone.name = name;
                    }
                    displayText = "";
                    break;
                #endregion
                default:
                    displayText = "";
                    break;
            }
            SpellProgressText.text = "";
        }
    }

    //Uses raycasts to find what a spell is targeting
    void getTarget()
    {
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        int layerMask = 0 << 8;
        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out hit, 50, layerMask))
        {
            if (hit.collider != null && hit.collider.gameObject.tag != "Enemy" && hit.collider.gameObject.layer != 2)
            {
                spellTarget = hit.transform.gameObject;
                unchild = spellTarget.transform.parent;
            }
        }
        else
            Debug.Log("Already have a target");

        //Debug code
        //Debug.DrawRay(ray.origin, ray.direction, Color.red, 5f);
        if (spellTarget != null && spellTarget.GetComponent<Rigidbody>() != null)
            Debug.Log("Target: " + spellTarget.name);
        else if (spellTarget != null)
            Debug.Log(spellTarget.name + " cannot be targeted");
        else
            Debug.Log("No Target | " + ray.GetPoint(10f) + " | " + playerCamera.transform.position);
    }

    //Determines if spellTarget of telekinesis would hit the player (Uses collision events from the playerobject)
    public void telekinesisCollide(Collider other, bool touch)
    {
        Debug.Log(other);
        Debug.Log(touch);

        if (other = spellTarget.GetComponent<Collider>())
        {
            if (touch == true)
            {
                Debug.Log("Touching: " + other);
                playerTouching = true;
            }
            else if (touch == false)
            {
                Debug.Log("Stopped touching: " + other);
                playerTouching = false;
            }
        }
    }

    //Clears all the stuff
    public void Clear()
    {

        switch (activeSpell)
        {
            case "TELEKINESIS":
                mat.SetColor("_EmissiveColor", new Color(0, 0, 0, 0));
                mat.DisableKeyword("_UseEmissiveIntensity");
                spellTarget.GetComponent<Rigidbody>().useGravity = true;
                //spellTarget.transform.parent = unchild;
                break;
            default:
                displayText = "";
                break;
        }

        fireballUI.SetActive(false);
        telekinesisUI.SetActive(false);
        polymorphUI.SetActive(false);
        ConjureCrateUI.SetActive(false);
        ResizeUI.SetActive(false);
        PushUI.SetActive(false);
        DuplicateUI.SetActive(false);
        PullUI.SetActive(false);

        displayText = "";
        spellTarget = player;
        activeSpell = null;
    }
}

