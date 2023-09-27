using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChainAction : MonoBehaviour
{
      #region Events
    
        public delegate void ChainActionDelegate();
    
        public event ChainActionDelegate ChainActionDone;
        public event ChainActionDelegate ChainActionStarted;
    
        #endregion

        /// <summary>
        /// When true, the action will be completed when the user has performed a certain action.
        /// </summary>
        [SerializeField, Tooltip("When true, the action will be completed when the user has performed a certain action.")]
        protected bool _useUserBasedAction = false;
        
        protected float _maxTime = -1.0f;
        private float _elapsedTime = 0.0f;
        protected bool _userBasedActionCompleted = false;
        
        public virtual void Execute()
        {
        }

        public virtual void UpdateAction(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
        }

        public virtual bool IsCompleted()
        {
            if (_useUserBasedAction) return _userBasedActionCompleted;
            return _elapsedTime >= _maxTime + Time.deltaTime;
        }
        
        public virtual void OnEnter()
        {
            ChainActionStarted?.Invoke();
            //Debug.Log("ChainAction: " + _nameChainAction + " started.");
        }
        
        public virtual void OnExit()
        {
            ChainActionDone?.Invoke();
            //Debug.Log("ChainAction: " + _nameChainAction + " finished.");
        }
}
