﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public Sprite ImageHover;
    public Sprite ImageNormal;

    public Image buttonImage;

    public Animator anim;

    private bool state;

    public void Awake()
    {
        StartCoroutine(skipFirstAnim());
        state = EventSystem.current.currentSelectedGameObject == gameObject;
    }

    public void Update()
    {
        bool newState = EventSystem.current.currentSelectedGameObject == gameObject;
        if (state != newState)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            if (newState)
                OnPointerEnter(data);
            else
                OnPointerExit(data);
            state = newState;
        }
    }

    IEnumerator skipFirstAnim()
    {
        anim.speed = 50000000f;
        yield return new WaitForSeconds(0.01f);
        anim.speed = 1f;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AkSoundEngine.PostEvent("menu_scroll", gameObject);
        anim.SetBool("Hover", true);
        buttonImage.sprite = ImageHover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("Hover", false);
        buttonImage.sprite = ImageNormal;
    }
}
