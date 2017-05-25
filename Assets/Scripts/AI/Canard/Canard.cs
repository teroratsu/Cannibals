﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Canard : AIAgent {
        public Transform wanderPosition;
        public float wanderRadius = 3;
        public Transform deadPosition;

        private bool dead = false;

        public float eatTime = 4;
        public float dieTime = 4;

	    // Use this for initialization
	    new void Start () {
            base.Start();

            ActionTask root = new ActionTask();
            root.OnExecute = () =>
            {
                if(WanderAround(wanderPosition.position, wanderRadius))
                {
                    Wait(2);
                }
            };
            Play(root);
	    }

        protected void Eating()
        {
            dead = true;
            agent.ResetPath();
            animator.Play("ToEat");
            Stop();
            Wait(eatTime).Next = () =>
            {
                animator.Play("ToSwim");
                Wait(dieTime).Next = () =>
                {
                    animator.Play("Die");
                    Wait(0.1f).Next = () =>
                    {
                        Wait(animator.GetCurrentAnimatorStateInfo(0).length).Next = Move;
                    };
                };
            };
        }

        protected void Move()
        {
            ActionTask action = new ActionTask();
            action.OnExecute = () =>
            {
                if (MoveTo(deadPosition.position, 2))
                {
                    StopAll();
                }
            };
            Play(action);
            
        }

        public bool DeadDuck()
        {
            return dead;
        }

        public void CallEat()
        {
            Eating();
        }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
	    }
    }
}