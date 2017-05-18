using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class gazeChessBoard : MonoBehaviour
{
    public static gazeChessBoard Instance { get; private set; }
    public static bool selected = false;

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }
//   private Color oldColor;
    GestureRecognizer recognizer;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
//        oldColor = new Color(0, 0, 0, 0);
        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.TappedEvent += (source, tapCount, ray) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect");
            }
        };
        recognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;
        
        // Do a raycast into the world based on the user's
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
            updateLookingObject(FocusedObject.name);       
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
            updateLookingObject("");
            //            if (oldColor.a != 0)
            //                oldFocusObject.GetComponent<Renderer>().material.color = oldColor;
            //            oldColor.a = 0;
        }



        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject != oldFocusObject)
        {
//            if (!selected)
//            {
//                if (oldColor.a != 0)
//                    oldFocusObject.GetComponent<Renderer>().material.color = oldColor;
//               oldColor.r = FocusedObject.GetComponent<Renderer>().material.color.r;
//               oldColor.g = FocusedObject.GetComponent<Renderer>().material.color.g;
//               oldColor.b = FocusedObject.GetComponent<Renderer>().material.color.b;
//               oldColor.a = FocusedObject.GetComponent<Renderer>().material.color.a;
//               FocusedObject.GetComponent<Renderer>().material.color = Color.yellow;
//            }
            //FocusedObject.GetComponent<Renderer>().material.color = Color.yellow;
            recognizer.CancelGestures();
            recognizer.StartCapturingGestures();
        }

    }

    void updateLookingObject( string name )
    {
        CustomMessages.Instance.SendlookingTransform(name);
    }

}
