using Destruction.Common;
using UnityEngine;

namespace Destruction.Dynamic
{
    public class ShardPool : MonoBehaviour
    {
        /// <summary>
        /// Self reference.
        /// </summary>
        private static ShardPool pool;
        
        [HideInInspector]
        [SerializeField]
        private Shard[] shards;

        [SerializeField]
        private int maxPoolSize = 800;

        /// <summary> Pointer to the index of the next available shard in the pool. </summary>
        private int nextFreeShard;

        /// <summary> A quick helper for previewing how many shards are currently in use from this pool. </summary>
        public static int ShardsUsed
        {
            get
            {
                CheckPool();

                return pool.nextFreeShard;
            }
        }

        /// <summary> A quick helper checking that the pool has been populated. </summary>
        public bool IsPopulated
        {
            get { return shards != null; }
        }

        /// <summary>
        /// Pops a shard out of the pool.
        /// </summary>
        public static Shard NextShard
        {
            get
            {
                CheckPool();

                if (CanGetNextShard)
                {
                    return pool.shards[pool.nextFreeShard++];
                }

                // If we reach here then there arent any shards left in the pool.

                return null;
            }
        }

        /// <summary>
        /// Checks if there are enough shards in the pool to get another one.
        /// </summary>
        private static bool CanGetNextShard
        {
            get { return (pool.nextFreeShard + 1) < pool.shards.Length; }
        }

        /// <summary>
        /// Sets the static reference and builds the pool.
        /// </summary>
        private void Awake()
        {
            pool = this;

            PopulatePool(); 
        }

        private void Start()
        {
            WarmUpDestruction();
        }

        private void WarmUpDestruction()
        {
            ConvexFracture temp = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<ConvexFracture>();
            temp.transform.position = new Vector3(100000, 100000, 10000000);
            ((IDestructable)temp).Destroy(temp.transform.position, 0.1f);
            foreach(var s in temp.finalizedBrokenPieces)
            {
                Destroy(s.gameObject);
            }
        }

        #region Aux
        /// <summary>
        /// Finds the pool in the scene and populates it.
        /// </summary>
        public static void CheckPool()
        {
            // If there is no static reference, try and find on in the scene.
            if (pool == null)
            {
                FindPoolInScene();
            }

            // If there are no shards in the pool, then populate it.
            pool.PopulatePool();
        }

        /// <summary>
        /// Finds an instance of me in the scene, if one doesn't exist, then create one.
        /// </summary>
        private static void FindPoolInScene()
        {
            pool = FindObjectOfType(typeof(ShardPool)) as ShardPool;

            if(pool == null)
            {
                GameObject newPool = new GameObject("_Shard Pool");
                pool = newPool.AddComponent<ShardPool>();
            }
        }

        /// <summary>
        /// Initialize the arrays and fill them with blank shards.
        /// </summary>
        public void PopulatePool()
        {
            if (IsPopulated && shards.Length == maxPoolSize) return;

            // Clear old shard pool if one exists.
            if (shards != null)
            {
                foreach (var shard in shards)
                {
                    if (shard == null) continue;

                    DestroyImmediate(shard.gameObject);
                }
            }

            shards = new Shard[maxPoolSize];

            for (int i = 0; i < maxPoolSize; i++)
            {
                shards[i] = new GameObject("Shard").AddComponent<Shard>();
                shards[i].transform.parent = transform;
            }
        }

        #endregion
    }
}