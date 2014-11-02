using UnityEngine;
using System.Collections;

public class AnimationTestSceneController : MonoBehaviour {
    public Animator Animator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI() {
        bool walking = Animator.GetBool("Walking");
        string buttonText = (walking ? "Idle" : "Walk");

        if(GUI.Button(new Rect(20,40,80,20), buttonText))
            Animator.SetBool("Walking", !walking);
	}
}
