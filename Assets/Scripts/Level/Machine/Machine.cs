﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine : MonoBehaviour, IActivable {

    private bool On = false;
    public IconDisplayer icon;
    public bool Working = false;
    public float time = 30;
    private float timer = 0;
    private Animator animator;
    public delegate void Finish(GameObject can);
    public event Finish finish;
    public GameObject prefabCanette;
    public Transform positionCanette;
    public int production = 3;
    int produced = 0;

    bool hasCan = false;

    public bool poisoned = false;

	void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (On && Working)
        {
            timer += Time.deltaTime;
            if (timer > time)
            {
                GameObject newCan = Instantiate(prefabCanette);
                newCan.transform.position = positionCanette.position;
                if (poisoned) newCan.GetComponent<Canette>().poisoned = true;
                //Generate canette 
                if (finish != null)
                    finish(newCan);
                produced++;
                if (produced == production)
                {
                    Working = false;
                    produced = 0;
                    animator.Play("Idle");
                }
                else
                {
                    timer = 0;
                }
                hasCan = true;
                //poisoned = true;
            }
        }
    }

    public void takeCan()
    {
        hasCan = false;
    }

    public bool CanReady()
    {
        return hasCan;
    }



    public void GenerateurSwitch(bool val)
    {
        if (val)
        {
            On = true;
            if(Working)
                animator.Play("ToOn");
        }
        else
        {
            On = false;
            animator.Play("Idle");
        }
    }

    public bool IsOn()
    {
        return On;
    }


    public void Launch()
    {
        timer = 0;
        Working = true;
        if(On)
            animator.Play("ToOn");
    }

    public bool IsActivable(CannibalObject cannibalObject)
    {
        if (cannibalObject is Champignon)
            return true;

        return false;
    }

    public void Activate(CannibalObject cannibalObject)
    {
        if (cannibalObject is Champignon)
        {
            if (((Champignon)cannibalObject).type == Champignon.Type.Champoison)
                poisoned = true;

            ((Champignon)cannibalObject).gameObject.SetActive(false);
        }
    }

    public void ShowIcon()
    {
        icon.Show();
    }
}
