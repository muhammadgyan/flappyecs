using System;
using Svelto.ECS.Hybrid;
using UnityEngine;

namespace Implementor
{
    public class DestroyableImplementor : MonoBehaviour, IDestroyableComponent, IImplementor
    {
        private GameObject _gameObject;

        private void Awake()
        {
            this._gameObject = this.gameObject;
        }

        public void Destroy()
        {
            Destroy(_gameObject);
        }
    }
}