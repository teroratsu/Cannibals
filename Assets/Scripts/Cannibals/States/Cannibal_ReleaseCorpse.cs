﻿using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;

[Category("Cannibal")]
public class Cannibal_ReleaseCorpse :ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;

    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.value.m_cannibalSkill.ReleaseCorpse();
    }

    /// <summary>
    /// Resuscitate the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be resuscitate for the moment</returns>
    public bool Resurrect() { return false; }

    /// <summary>
    /// Kill the cannibal
    /// </summary>
    /// <returns>false if the cannibal can't be killed in the current state</returns>
    public bool Kill() { this.FSM.SendEvent("Death"); return true; }

    /// <summary>
    /// Return if in the currentState the cannibal is considered dead
    /// </summary>
    /// <returns></returns>
    public bool IsDead() { return false; }
}