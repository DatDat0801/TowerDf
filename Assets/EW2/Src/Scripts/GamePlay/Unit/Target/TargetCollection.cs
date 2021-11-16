using System.Collections.Generic;
using UnityEngine;
using Zitga.Observables;

namespace EW2
{
    public abstract class TargetCollection<T> : MonoBehaviour
    {
        
        private ObservableList<T> targets;

        protected MoveType targetType;
        public ObservableList<T> Targets => targets ?? (targets = new ObservableList<T>());

        private void OnTriggerEnter2D(Collider2D other)
        {
            var target = GetTarget(other);
            
            FilterTarget(target);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var target = GetTarget(other);
            
            RemoveTarget(target);
        }

        protected abstract T GetTarget(Collider2D other);
        

        protected abstract void FilterTarget(T target);
        
        public abstract T SelectTarget();

        public abstract void RemoveTarget(T target);
        
        public void SetTargetType(MoveType moveType)
        {
            this.targetType = moveType;
        }
    }
}