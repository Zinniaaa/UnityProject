using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emoter : MonoBehaviour
{

    public int interval = 10;

    private float time = 0;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        time = Time.time + interval;
    }

    // Update is called once per frame
    void Update()
    {
        if (time < Time.time)
        {
            time = Random.Range(0.7f, 1.5f) * interval + Time.time;
            int random = Random.Range(0, 150);
            string animName = random <= 100 ? "eyesClosed" : "laught";
            animator.Play(animName);
        }
    }
}
