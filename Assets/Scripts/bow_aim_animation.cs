using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bow_aim_animation : MonoBehaviour
{
    Animator anim;

    public float blendIdle = 0;
    public float blendSpeed = 3.0f;
    public float blendMax = 100f;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate ()
    {
        float aim = Input.GetAxis("Vertical");

        anim.SetFloat("Aim", blendIdle);

        if (aim < 0) anim.SetFloat("Aim", blendIdle -= blendSpeed);
        if (aim > 0) anim.SetFloat("Aim", blendIdle += blendSpeed);

    }
}
