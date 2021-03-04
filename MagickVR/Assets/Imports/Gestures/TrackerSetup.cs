using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gestures;
using UnityEngine.Events;


public class TrackerSetup : MonoBehaviour {

    private GestureMonitor tracker;
    public LineRenderer lineRenderer;
    public IController controller;
    public SpellCast RightHand;

    void Start () {
        tracker = gameObject.AddComponent<GestureMonitor>();
        tracker.controller = controller;
        tracker.lineRenderer = lineRenderer;

        GenerateGestures();

        tracker.AddGestureCompleteCallback(GestureComplete);
        tracker.AddGestureFailedCallback(GestureFailed);
        tracker.AddGestureStartCallback(GestureStart);

    }

    
    void GestureStart() {
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
        lineRenderer.enabled = true;
    }

	
    void GestureComplete(GestureMetaData data) {
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
        lineRenderer.enabled = false;
        RightHand.CastMagic(data);
    }


    void GestureFailed(GestureMetaData data) {
        lineRenderer.enabled = false;
    }


    void GenerateGestures() {

        tracker.AddGesture("Square", new SquareGesture(.8f));
        tracker.AddGesture("L", new LGesture(.8f));
        tracker.AddGesture("Inverted L", new UpsideDownLGesture(.8f));
        tracker.AddGesture("Circle", new CircleGesture(.6f));
        tracker.AddGesture("Triangle", new TriangleGesture(1.3f));
        tracker.AddGesture("Heart", new HeartGesture(.8f));
        tracker.AddGesture("Letter-S", new Gesture().AddChecks(new List<Check> {
            new ArcCheck(new Vector3(.5f, .5f, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(0, 1, 0), -90, new Vector3(0,.5f,0)),
            new ArcCheck(new Vector3(-.5f,.5f,0), -90, new Vector3(0,.5f,0)),

            new ArcCheck(new Vector3(0, 0, 0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(.5f,-.5f,0), 90, new Vector3(0,-.5f,0)),
            new ArcCheck(new Vector3(0,-1,0), 90, new Vector3(0,-.5f,0)) 

        }).SetNormalizer(new FittedNormalizer(new Vector3(-.5f, -1.0f, 0), new Vector3(.5f, 1.0f, 0))));

        tracker.AddGesture("Cross", new Gesture().AddChecks(new List<Check> {
                new LineCheck(new Vector3(-1, 0, 0), new Vector3(1, 0, 0), .8f),
                new LineCheck(new Vector3(1, 0, 0), new Vector3(0, 1, 0), .8f),
                new LineCheck(new Vector3(0, 1, 0), new Vector3(0, -1, 0), .8f),

                new RadiusCheck(new Vector3(1, 0, 0), .8f/2),
                new RadiusCheck(new Vector3(0, 1, 0), .8f/2),
                new RadiusCheck(new Vector3(-1, 0, 0), .8f/2),
                new RadiusCheck(new Vector3(0, -1, 0), .8f/2),
            })
            .SetNormalizer(new FittedNormalizer(new Vector3(-1, -1, 0), new Vector3(1, 1, 0))));
    }

}
