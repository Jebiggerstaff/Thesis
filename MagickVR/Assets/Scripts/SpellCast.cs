using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellCast : MonoBehaviour
{
    public GameObject TrackingSpace;
    public OVRInput.Controller Controller;
    public OVRInput.Button PointerFingerTrigger;
    public OVRInput.Button SecondaryTrigger;
    public OVRInput.Button TelekinesisFarther;
    public OVRInput.Button TelekinesisCloser;
    public GameObject Otherhand;

    public GameObject Aimline;

    public GameObject fireball;
    public GameObject Crate;    
    public GameObject Ball;
    public GameObject pushBox;
    public GameObject pullBox;

    public GameObject spellGuide;
    public GameObject spellTarget;

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

    public float ForceMod = 1000f; //Used to move objects in telekinesis
    public float spellLongevity = 5f; //Length of spell effects
    float targetDistance; //Distance between target and object (Telekinesis)
    bool playerTouching; //Determines if player is touching object that is being targeted
    Vector3 move; //Used to move objects in telekinesis
    float startRot= 0;
    float dist = 0f;
    float prevDist = 0f;

    private GameObject CreatedFireball;

    private bool OngoingTelekinesis=false;
    private bool OngoingResize;
    private bool OngoingFireball;
    public bool Conjuringbox = false;
    public bool Resizing = false;
    bool BoxSpawned = false;
    Vector3 polyTemp;
    bool duplicated = false;
    bool polymorphing = false;
    public Material boxTeleMat;
    public Material boxNorm;

    private void Update()
    {
        dist = Vector3.Distance(this.gameObject.transform.position, Otherhand.transform.position);

        if (OngoingTelekinesis)
        {
            targetDistance = Vector3.Distance(spellGuide.transform.position, spellTarget.transform.position);
            ForceMod = 10;
            if (targetDistance >= .2 && playerTouching == false){
                spellTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                spellTarget.transform.position = Vector3.Lerp(spellTarget.transform.position, spellGuide.transform.position, ForceMod * Time.deltaTime);
            }
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

        if (OVRInput.Get(SecondaryTrigger) && OVRInput.Get(PointerFingerTrigger))
            Resizing = true;
        else
            Resizing = false;

        //When You Grip
        if (OVRInput.Get(PointerFingerTrigger)) {

            #region Telekinesis
            if (!OngoingTelekinesis && !OngoingFireball && !Conjuringbox)
            {
                Aimline.SetActive(false);

                if(!OngoingTelekinesis)
                    getTarget();

                if (spellTarget != null)
                {
                    if (spellTarget.GetComponent<Rigidbody>() != null)
                    {
                        spellTarget.GetComponent<Renderer>().material = boxTeleMat;
                        OngoingTelekinesis = true;
                    }
                }
            }
            #endregion

            #region Fireball
            if (!OngoingTelekinesis || !Conjuringbox || !OngoingResize)
            {
                getTarget();
                if (spellTarget == null)
                {
                    if (startRot == 0f)
                        startRot = this.transform.rotation.z;

                    if (this.transform.rotation.z - startRot > .5 || this.transform.rotation.z - startRot < -.5)
                    {
                        if (CreatedFireball == null)
                        {
                            if (this.gameObject.name == "Right Hand")
                                CreatedFireball = Instantiate(fireball, this.transform.position + new Vector3(0, .3f, .3f), new Quaternion(0, 0, 0, 0), this.transform);
                            else
                                CreatedFireball = Instantiate(fireball, this.transform.position + new Vector3(0, .3f, -.3f), new Quaternion(0, 0, 0, 0), this.transform);
                        }
                        OngoingFireball = true;
                    }
                }
            }
            #endregion

            #region Resize
            getTarget();
            if (spellTarget == Otherhand.GetComponent<SpellCast>().spellTarget)
            {
                if (Resizing == true && Otherhand.GetComponent<SpellCast>().Resizing == true)
                {

                    if (prevDist == 0f)
                    {
                        prevDist = dist;
                        polyTemp = spellTarget.transform.lossyScale;
                    }

                    if (this.gameObject.name == "Right Hand")
                    {

                        if (dist > prevDist && spellTarget.transform.lossyScale.magnitude <= polyTemp.magnitude * 1.5)
                        {
                            spellTarget.transform.localScale += new Vector3(.01f, .01f, .01f);
                        }
                        else if (dist < prevDist && spellTarget.transform.lossyScale.magnitude >= polyTemp.magnitude * .5)
                        {
                            spellTarget.transform.localScale -= new Vector3(.01f, .01f, .01f);
                        }
                        

                    }
                }
                
            }
            #endregion//LIMIT RESIZE

            #region ConjureBox
            if (!OngoingTelekinesis && !OngoingFireball && !OngoingResize && !Otherhand.GetComponent<SpellCast>().OngoingFireball)
            {
                getTarget();
                if (spellTarget == null)
                {
                    Conjuringbox = true;
                    if (Otherhand.GetComponent<SpellCast>().Conjuringbox == true && BoxSpawned != true)
                    {
                        if (this.gameObject.name == "Right Hand")
                        {
                            Vector3 midPoint = (this.transform.position + Otherhand.transform.position) / 2f;
                            Instantiate(Crate, midPoint, new Quaternion(0, 0, 0, 0));
                            BoxSpawned = true;
                        }
                    }
                }
            }
            #endregion

            #region Duplicate
            getTarget();
            if (spellTarget == Otherhand.GetComponent<SpellCast>().spellTarget)
            {
                if (startRot == 0f)
                    startRot = this.transform.rotation.y;

                if (this.transform.rotation.y - startRot > .3 || this.transform.rotation.y - startRot < -.3)
                {
                    if (duplicated == false)
                    {
                        string name = spellTarget.name;
                        GameObject clone = Instantiate(spellTarget, spellTarget.transform.position, spellTarget.transform.rotation);
                        clone.name = name;
                        spellTarget = clone;
                        duplicated = true;
                        
                    }
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
                CreatedFireball.GetComponent<Rigidbody>().velocity = TrackingSpace.transform.rotation * OVRInput.GetLocalControllerVelocity(Controller)*30;
                CreatedFireball.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);
                CreatedFireball = null;
            }

            if (OngoingTelekinesis)
            {
                OngoingTelekinesis = false;
                spellTarget.GetComponent<Rigidbody>().useGravity = true;
                spellTarget.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                spellTarget.GetComponent<Rigidbody>().velocity = TrackingSpace.transform.rotation * OVRInput.GetLocalControllerVelocity(Controller) * 5;
                spellTarget.GetComponent<Rigidbody>().angularVelocity = OVRInput.GetLocalControllerAngularVelocity(Controller);
                if (spellTarget != Otherhand.GetComponent<SpellCast>().spellTarget)
                    spellTarget.GetComponent<Renderer>().material = boxNorm;
                spellTarget = null;
            }

            if (OngoingResize)
            {
                OngoingResize = false;
                spellTarget = null;
            }

            startRot = 0;
            prevDist = 0;
            Conjuringbox = false;
            BoxSpawned = false;
            duplicated = false;

            OngoingFireball = false;

            Aimline.SetActive(true);
            spellGuide.transform.localPosition = new Vector3(0, 0, 3f);
        }

    }

    void getTarget()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        int layerMask = 0 << 8;
        layerMask = ~layerMask;

        if (Physics.Raycast(ray, out hit, 50, layerMask) &&!OngoingTelekinesis)
        {
            if (hit.collider != null && hit.collider.gameObject.layer != 8 && hit.collider.gameObject.layer != 9 && hit.collider.gameObject.layer != 2)
            {
                spellTarget = hit.transform.gameObject;
                //unchild = spellTarget.transform.parent;
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
