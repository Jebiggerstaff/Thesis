using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gestures;


public class SpellCast : MonoBehaviour
{
    public OVRInput.Button FireSpellButton;
    public OVRInput.Button gestureActiveButton;

    public GameObject Aimline;


    public GameObject fireball;
    public GameObject Crate;    
    public GameObject Ball;
    public GameObject pushBox;
    public GameObject pullBox;

    public GameObject spellGuide;
    public GameObject spellTarget;
    private string activeSpell="";

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

    int layerMask = 0 << 8;
    RaycastHit hit;

    private bool OngoingTelekinesis;
    private bool OngoingResize;

    public void CastMagic(GestureMetaData data)
    {
        //loads a spell to be cast
        switch (data.name){

            #region Fireball
            case "Plus":
                fireballUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Telekinesis
            case "Triangle":
                telekinesisUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Resize
            case "Letter-S":
                ResizeUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Conjure Crate
            case "Square":
                ConjureCrateUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Push
            case "L":
                PushUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Pull
            case "Inverted L":
                PullUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Polymorph
            case "Circle":
                polymorphUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion
            #region Duplicate
            case "Heart":
                DuplicateUI.SetActive(true);
                activeSpell = data.name;
                Aimline.SetActive(true);
                break;
            #endregion

            default:
                break;
        }
    }
    private void Update()
    {

        

        if (OngoingTelekinesis)
        {
            //Debug.LogWarning("Target: " + spellTarget.name);
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

            //moves object closer/farther
            move = spellGuide.transform.localPosition;
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown) && Vector3.Distance(spellGuide.transform.position, transform.position) > 3)
            {
                move.z -= .1f;
                spellGuide.transform.localPosition = move;
            }
            else if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp) && Vector3.Distance(spellGuide.transform.position, transform.position) < 20)
            {
                move.z += .1f;
                spellGuide.transform.localPosition = move;
            }
        }

        if (OngoingResize)
        {
            if (spellTarget.transform.lossyScale.magnitude <= polyTemp.magnitude * 1.5 && OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp))
            {
                spellTarget.transform.localScale += new Vector3(.01f, .01f, .01f);
            }
            else if (spellTarget.transform.lossyScale.magnitude >= polyTemp.magnitude * .5 && OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
            {
                spellTarget.transform.localScale -= new Vector3(.01f, .01f, .01f);
            }
        }

        //Fires off the Spell
        if (OVRInput.Get(FireSpellButton)) {

            Aimline.SetActive(false);

            switch (activeSpell){

                #region Fireball
                case "Plus":
                    layerMask = ~layerMask;
                    if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 30f, layerMask))
                    {
                        Instantiate(fireball, hit.point, transform.rotation);
                    }
                    else
                    {
                        Instantiate(fireball, spellGuide.transform.position, transform.rotation);
                    }
                    fireballUI.SetActive(false);
                    activeSpell = "";
                    break;

                #endregion
                #region Telekinesis
                case "Triangle":
                    getTarget();
                    if (spellTarget.GetComponent<Rigidbody>() != null)
                    {
                        OngoingTelekinesis = true;
                    }
                    telekinesisUI.SetActive(false);
                    break;
                #endregion
                #region Resize
                case "Letter-S":
                    getTarget();
                    polyTemp = spellTarget.transform.lossyScale;
                    if (spellTarget.GetComponent<Rigidbody>() != null)
                    {
                        OngoingResize = true;
                    }
                    ResizeUI.SetActive(false);
                    break;
                #endregion
                #region Conjure Crate
                case "Square":

                    ConjureCrateUI.SetActive(false);
                    layerMask = ~layerMask;
                    if (Physics.Raycast(new Ray(transform.position, transform.forward), out hit, 9.9f, layerMask))
                    {
                        Instantiate(Crate, hit.point, transform.rotation);
                    }
                    else
                        Instantiate(Crate, spellGuide.transform.position, transform.rotation);

                    ConjureCrateUI.SetActive(false);
                    activeSpell = "";
                    break;
                #endregion
                #region Push
                case "L":
                    Instantiate(pushBox, transform.position, transform.rotation);
                    PushUI.SetActive(false);
                    activeSpell = "";
                    break;
                #endregion
                #region Pull
                case "Inverted L":
                    Instantiate(pullBox, transform.position, transform.rotation);
                    PullUI.SetActive(false);
                    activeSpell = "";
                    break;
                #endregion
                #region Polymorph
                case "Circle":
                    polymorphUI.SetActive(false);
                    getTarget();

                    if (spellTarget.GetComponent<Rigidbody>() != null)
                    {
                        if (spellTarget.name == "Cube" || spellTarget.name == "Cube(Clone)")
                            Instantiate(Ball, spellTarget.transform.position, spellTarget.transform.rotation);
                        else
                            Instantiate(Crate, spellTarget.transform.position, spellTarget.transform.rotation);

                        Destroy(spellTarget);
                    }
                    activeSpell = "";
                    break;
                #endregion
                #region Duplicate
                case "Heart":
                    DuplicateUI.SetActive(false);
                    getTarget();

                    if (spellTarget.GetComponent<Rigidbody>() != null)
                    {
                        string name = spellTarget.name;
                        GameObject clone = Instantiate(spellTarget, spellTarget.transform.position, spellTarget.transform.rotation);
                        clone.name = name;
                    }
                    activeSpell = "";
                    break;
                #endregion
                default:
                    break;
            }
        }
        //Clear All UI when the player starts drawing a new symbol
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && activeSpell != "")
        {
            fireballUI.SetActive(false);
            telekinesisUI.SetActive(false);
            PushUI.SetActive(false);
            PullUI.SetActive(false);
            polymorphUI.SetActive(false);
            ResizeUI.SetActive(false);
            DuplicateUI.SetActive(false);
            ConjureCrateUI.SetActive(false);

            if (OngoingTelekinesis)
            {
                OngoingTelekinesis = false;
                spellTarget.GetComponent<Rigidbody>().useGravity = true;
                spellTarget = null;
            }
            if (OngoingResize)
            {
                OngoingResize = false;
                spellTarget = null;
            }

            Aimline.SetActive(false);
            activeSpell = "";
            spellGuide.transform.localPosition = new Vector3(0, 0, 3f);
        }

    }

    void getTarget()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
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
            Debug.LogWarning("Target: " + spellTarget.name);
        else if (spellTarget != null)
            Debug.LogWarning(spellTarget.name + " cannot be targeted");
        else
            Debug.LogWarning("No Target | " + ray.GetPoint(10f) + " | " + transform.position);
    }
}
