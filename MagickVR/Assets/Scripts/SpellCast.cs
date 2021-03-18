using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCast : MonoBehaviour
{
    public GameObject TrackingSpace;
    public OVRInput.Controller Controller;
    public OVRInput.Button gestureActiveButton;
    public OVRInput.Button TelekinesisFarther;
    public OVRInput.Button TelekinesisCloser;

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
    float startRot= 0;

    int layerMask = 0 << 8;
    RaycastHit hit;

    private GameObject CreatedFireball;

    private bool OngoingTelekinesis;
    private bool OngoingResize;
    private bool OngoingFireball;

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
            if (OVRInput.Get(TelekinesisCloser) && Vector3.Distance(spellGuide.transform.position, transform.position) > 3)
            {
                move.z -= .1f;
                spellGuide.transform.localPosition = move;
            }
            else if (OVRInput.Get(TelekinesisFarther) && Vector3.Distance(spellGuide.transform.position, transform.position) < 20)
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

        //When You Grip
        if (OVRInput.Get(gestureActiveButton)) {

            #region Telekinesis
            if (!OngoingTelekinesis || !OngoingFireball)
            {
                Aimline.SetActive(false);
                getTarget();
                if (spellTarget != null)
                {
                    if (spellTarget.GetComponent<Rigidbody>() != null)
                    {
                        OngoingTelekinesis = true;
                    }
                    telekinesisUI.SetActive(false);
                }
            }
            #endregion

            #region Fireball
            getTarget();
            if (spellTarget == null)
            {
                if (startRot == 0f)
                    startRot = this.transform.rotation.z;

                if(this.transform.rotation.z - startRot > .5|| this.transform.rotation.z - startRot < -.5)
                {
                    if (CreatedFireball == null)
                    {
                        if (this.gameObject.name == "Right Hand")
                            CreatedFireball = Instantiate(fireball, this.transform.position + new Vector3(0, .3f, .3f), new Quaternion(0, 0, 0, 0), this.transform);
                        else
                            CreatedFireball = Instantiate(fireball, this.transform.position + new Vector3(0, -.3f, -.3f), new Quaternion(0, 0, 0, 0), this.transform);
                    }
                    OngoingFireball = true;
                }
            }
            #endregion

        }
        //When you Let go of the Grip
        else
        {
            if (OngoingFireball && CreatedFireball!=null)
            {
                CreatedFireball.GetComponent<Fireball>().enabled = true;
                CreatedFireball.GetComponent<Rigidbody>().useGravity = true;
                CreatedFireball.transform.parent = null;
                CreatedFireball.GetComponent<Rigidbody>().velocity = TrackingSpace.transform.rotation * OVRInput.GetLocalControllerVelocity(Controller)*5;
                CreatedFireball.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);
                CreatedFireball = null;
            }

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

            startRot = 0;

            Aimline.SetActive(true);
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
            if (hit.collider != null && hit.collider.gameObject.layer != 8 && hit.collider.gameObject.layer != 9 && hit.collider.gameObject.layer != 2)
            {
                spellTarget = hit.transform.gameObject;
                unchild = spellTarget.transform.parent;
            }
        }
        /*
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
        */
    }

}
