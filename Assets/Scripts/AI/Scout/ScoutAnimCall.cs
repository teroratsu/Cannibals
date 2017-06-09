﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutAnimCall : MonoBehaviour {
    public AI.Scout scout;

	void Call()
    {
        scout.AnimCall();
    }

    void FootSteps()
    {
        AkSoundEngine.PostEvent("scout_steps", scout.gameObject);
    }
}
