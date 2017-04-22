﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeCanvas.Tasks.Conditions;
using NodeCanvas.Framework;

public class CheckCannibalObjectTrigger : CheckTriggerExt<CannibalObject> {



    protected override bool OnCheck()
    {
        bool check = base.OnCheck();

        if (check)
        {
            foreach (CannibalObject c in savedList.value)
            {
                c.ShowIcon();
            }
        }

        return check;
    }
}