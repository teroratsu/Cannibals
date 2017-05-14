﻿using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using ParadoxNotion.Design;
using UnityEditor;
using UnityEngine;

[Category("Cannibal")]
public class Cannibal_RunCorpse :ActionState, ICannibal_State {

    public BBParameter<Cannibal> m_cannibal;


    protected override void OnEnter()
    {
        base.OnEnter();
        m_cannibal.value.m_cannibalMovement.m_currentMaxRunSpeed = m_cannibal.value.m_cannibalMovement.MaxRunSpeed / 2f;
    }
    protected override void OnExit()
    {
        base.OnExit();
        m_cannibal.value.m_cannibalMovement.ResetMaxSpeed();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        m_cannibal.value.m_cannibalMovement.GroundMove(new Vector2(m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("SideMove"), m_cannibal.value.m_rewiredInput.m_playerInput.GetAxis("FrontMove")).normalized);
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
    public bool Kill() { m_cannibal.value.m_cannibalSkill.ReleaseCorpse(); this.FSM.SendEvent("Death"); return true; }

    /// <summary>
    /// Return if in the currentState the cannibal is considered dead
    /// </summary>
    /// <returns></returns>
    public bool IsDead() { return false; }
}
