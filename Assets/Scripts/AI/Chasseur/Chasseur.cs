﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AI
{
    public class Chasseur : AIAgent {
        public float waypointDistance = 2;
        public float shootingRange = 1;
        public LayerMask shootingMask = 0;
        
        public float lostTargetTime = 3;
        public float defendTime = 5;
        public Animator animator;
        public ParticleSystem fireBurst;

        int anim_speedId = Animator.StringToHash("Speed");
        int anim_shoot = Animator.StringToHash("Shoot");

        
        

        public bool alerte = false;

        public Waypoint patrouille;
        public float stunTime = 10;
        
        enum ChasseurTask
        {
            Normal,
            Chase,
            Defend,
            Look,
            LostTarget,
            Shoot,
            Drink,
            Etourdi
        }

	    // Use this for initialization
	    new void Start () {
            base.Start();
            type = AIType.Hunter;
            Bottle.OnBottleShaked += OnBottleShaked;
            
            tasks.Push(new Task((int)ChasseurTask.Normal));
	    }
	
	    // Update is called once per frame
	    new void Update () {
            base.Update();
		    if(animator != null && agent!=null)
            {
                animator.SetFloat(anim_speedId, agent.velocity.magnitude);
            }
            else
            {
                Debug.Log("Chasseur.cs: animator ou navmeshagent non présent");
            }

            //Check the navmesh for footsteps
            NavMeshHit hit;
            if(NavMesh.SamplePosition(transform.position, out hit, 10, ~0))
            {
                AkSoundEngine.SetSwitch("Steps", "Grass", gameObject);
            }


            AnalyseSight();

            Vector3 targetPosition;

            //Manage tasks
            switch (CurrentTask.id)
            {
                case (int)ChasseurTask.Normal:
                //Follow waypoints
                    if (patrouille != null)
                    {
                        if(CurrentTask.elapsed > 4 && MoveTo(patrouille.getCurrentDestination(), 5))
                        {
                            patrouille.Next();
                            CurrentTask.elapsed = 0;
                        }
                    }
                    break;

                //Poursuis le joueur repéré
                case (int)ChasseurTask.Chase:
                    if(MoveTo(CurrentTask.target.transform.position, shootingRange))
                    {
                        agent.ResetPath();
                        GameObject target = CurrentTask.target;
                        tasks.Pop();
                        tasks.Push(new Task((int)ChasseurTask.Shoot, target));
                    }
                    else if((CurrentTask.target.transform.position - transform.position).sqrMagnitude >= Mathf.Pow((los.camera.farClipPlane), 2))
                    {
                        agent.ResetPath();
                        tasks.Push(new Task((int)ChasseurTask.LostTarget, CurrentTask.target));
                    }
                    break;

                //Regarde vers le joueur jusqu'à détection
                case (int)ChasseurTask.Look:
                    if (Look())
                    {
                        GameObject target = CurrentTask.target;
                        tasks.Pop();
                        tasks.Push(new Task((int)ChasseurTask.Chase, target));
                    }
                    break;
                    
                //Le chasseur a perdu sa cible de vue
                case (int)ChasseurTask.LostTarget:
                    //Si il la voit à nouveau, retourne dans Chase
                    if (los.sighted.Contains(CurrentTask.target))
                    {
                        tasks.Pop();
                    }
                    else
                    {
                        //Après un certain temps, retourne à l'état précédent le chase
                        if (CurrentTask.elapsed > lostTargetTime)
                        {
                            ResetDetect(CurrentTask.target);
                            tasks.Pop();
                            tasks.Pop();
                        }
                        else //Sinon se déplace vers l'objet
                        {
                            MoveTo(CurrentTask.target.transform.position, shootingRange);
                        }
                    }
                    break;

                case (int)ChasseurTask.Shoot:
                    if (CurrentTask.count == 0)
                    {
                        animator.Play(anim_shoot);
                        CurrentTask.count++;
                    }
                    targetPosition = CurrentTask.target.transform.position;
                    targetPosition.y = agent.transform.position.y;
                    agent.transform.LookAt(targetPosition);
                    if (CurrentTask.elapsed > 7)
                    {
                        tasks.Pop();
                    }
                    break;

                //Attends à coté du cadavre du cannibal
                case (int)ChasseurTask.Defend:
                    WanderAround(CurrentTask.target.transform.position, 5);
                    if (CurrentTask.elapsed > defendTime)
                    {
                        agent.ResetPath();
                        tasks.Pop();
                    }
                    break;

                case (int)ChasseurTask.Drink:
                    if (MoveTo(CurrentTask.target.transform.position, 2))
                    {
                        CurrentTask.target.gameObject.SetActive(false);
                        animator.Play("IdleToKo");
                        AkSoundEngine.PostEvent("hunter_objects", gameObject);
                        tasks.Pop();
                        tasks.Push(new Task((int)ChasseurTask.Etourdi));
                    }
                    break;

                case (int)ChasseurTask.Etourdi:
                    if (CurrentTask.elapsed > stunTime)
                    {
                        animator.Play("Resurrect");
                        tasks.Pop();
                    }
                    break;
            }

	    }

        public void Shoot()
        {
            AkSoundEngine.PostEvent("hunter_rifle", gameObject);
            fireBurst.Play();
            Vector3 position = agent.transform.position;
            Vector3 direction = agent.transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(position, direction, out hit, shootingRange, shootingMask))
            {
                AIAgent agent = hit.collider.gameObject.GetComponent<AIAgent>();
                if (agent != null)
                {
                    agent.Kill();
                }
                else
                {
                    Cannibal cannibal = hit.collider.gameObject.GetComponentInParent<Cannibal>();
                    if (cannibal != null)
                    {
                        cannibal.Kill();
                    }
                    else
                    {
                        Bush bush = hit.collider.gameObject.GetComponent<Bush>();
                        if (bush != null)
                        {
                            bush.KillACannibal();
                        }
                    }
                }
            }
        }

        public void EndShoot()
        {
            if(CurrentTask.id == (int)ChasseurTask.Shoot)
            {
                Cannibal cannibal = CurrentTask.target.GetComponentInParent<Cannibal>();
                GameObject target = CurrentTask.target;
                tasks.Pop();
                if (cannibal != null)
                {
                    if (cannibal.IsDead())
                    {
                        ResetDetect(target);
                        tasks.Push(new Task((int)ChasseurTask.Defend, target));
                    }
                    else
                    {
                        tasks.Push(new Task((int)ChasseurTask.Chase, target));
                    }
                }
                Debug.Log("EndShoot:" + CurrentTask.id);
            }
        }

        /// <summary>
        /// The dog calls the hunter
        /// </summary>
        public void Call(GameObject target)
        {
            if (CurrentTask.id == (int)ChasseurTask.Normal)
            {
                tasks.Push(new Task((int)ChasseurTask.Chase, target));
            }
        }

        void OnBottleShaked(Bottle bot)
        {
            if (Vector3.SqrMagnitude(bot.transform.position - this.transform.position) < Mathf.Pow(los.getSeeDistance(),2))
            {
                tasks.Push(new Task((int)ChasseurTask.Drink, bot.gameObject));
            }
        }

        void AnalyseSight()
        {
            if (los.Updated)
            {
                //Analyse de la vue du chasseur
                foreach (GameObject obj in los.sighted)
                {
                    Cannibal cannibal = obj.GetComponentInParent<Cannibal>();
                    if (cannibal != null && !cannibal.IsDead())
                    {
                        if (CurrentTask.id == (int)ChasseurTask.Normal)
                        {
                            agent.ResetPath();
                            tasks.Push(new Task((int)ChasseurTask.Look, obj));
                        }
                        else if (CurrentTask.id == (int)ChasseurTask.LostTarget && obj != CurrentTask.target) //Change de cible
                        {
                            agent.ResetPath();
                            tasks.Pop();
                            CurrentTask.target = obj;
                        }
                    }
                }
            }
        }

        void FootSteps()
        {
            AkSoundEngine.PostEvent("hunter_steps", gameObject);
        }
        
    }
}
