using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EW2
{
    public class MultiMove
    {
        private Unit owner;
        
        private readonly List<Vector3> targetList;
        
        private readonly Queue<Vector3> targetQueue;
        public List<Vector3> TargetList => targetQueue.ToList();

        public Vector3 currentTarget;

        private readonly SingleMove singleMove;

        private bool isExecuting;
        private float _speed;

        public Action onFinish = delegate { };


        public MultiMove(Unit owner)
        {
            this.owner = owner;
            
            isExecuting = false;

            targetList = new List<Vector3>();
            
            targetQueue = new Queue<Vector3>();
            
            singleMove = new SingleMove(owner)
            {
                onFinish = () =>
                {
                    if (targetQueue.Count > 0)
                    {
                        currentTarget = targetQueue.Dequeue();

                        singleMove.SetTarget(currentTarget);
                    }
                    else
                    {
                        isExecuting = false;

                        onFinish?.Invoke();
                    }
                }
            };
        }

        public void SetTarget(List<Vector3> targets)
        {
            targetList.Clear();
            
            targetList.AddRange(targets);
            
            this.targetQueue.Clear();

            foreach (var target in targets)
            {
                this.targetQueue.Enqueue(target);
            }
            
            isExecuting = false;
        }

        public void ResetMove()
        {
            if (targetList.Count == 0 || owner == null)
            {
                return;
            }

            this.owner.Transform.position = this.targetList[0];
            
            this.targetQueue.Clear();

            foreach (var target in targetList)
            {
                this.targetQueue.Enqueue(target);
            }

            isExecuting = false;
            
            this.Execute();
        }

        public void Execute()
        {
            if (isExecuting == false)
            {
                if (targetQueue.Count > 0)
                {
                    isExecuting = true;

                    currentTarget = targetQueue.Dequeue();

                    singleMove.SetTarget(currentTarget);
                }
                else
                {
                    //throw new Exception("targets is null");
                }
            }

            singleMove.Execute();
        }
        
        public void Stop()
        {
            this._speed = this.singleMove.Speed;
            this.singleMove.Speed = 0;
        }

        public void Resume()
        {
            this.singleMove.Speed = this._speed;
        }
    }
}