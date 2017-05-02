﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckIKnifeKillableTrigger : CheckTriggerExt<IKnifeKillable>
{
    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
        {
            for (int i = savedList.value.Count - 1; i >= 0; i--)
            {
                if (savedList.value[i].IsKnifeVulnerable())
                {
                    savedList.value[i].ShowKnifeIcon();
                }
                else
                    savedList.value.RemoveAt(i);
            }
        }

        return savedList.value.Count > 0;
    }
}