﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Generateur : MonoBehaviour, IActivable {
    public IconDisplayer icon;
    public bool On = true;
    public Machine machine;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        machine.GenerateurSwitch(On);
        if (On)
        {
            animator.Play("ToOn");
            AkSoundEngine.PostEvent("generator", gameObject);
        }
    }

	public void Switch()
    {
        if (On)
        {
            animator.Play("ToIdle");
            On = false;
            AkSoundEngine.StopAll(gameObject);
        }
        else
        {
            animator.Play("ToOn");
            On = true;
            AkSoundEngine.PostEvent("generator", gameObject);
        }
        machine.GenerateurSwitch(On);
    }

    public bool IsActivable(CannibalObject cannibal)
    {
        return true;
    }

    public void Activate(CannibalObject cannibal)
    {
        Switch();
    }

    public void ShowIcon()
    {
        icon.Show();
    }
}
