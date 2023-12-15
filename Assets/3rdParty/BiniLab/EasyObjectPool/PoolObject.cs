using UnityEngine;
using System.Collections;

namespace ObjectPool
{
    public class PoolObject : MonoBehaviour
    {
        [HideInInspector]
        public string poolName;
        //defines whether the object is waiting in pool or is in use
        public bool isPooled;
        public bool useObjectPool;

        protected virtual void ReturnObject()
        {
            //ObjectPoolManager.Instance.ReturnObjectToPool(this.gameObject);
        }

        protected virtual void OnParticleSystemStopped()
        {
            this.ReturnObject();
        }
    }
}
