using UnityEngine;
using Rain.Core;
using Rain.Launcher;

namespace Rain.Tests
{
    public class DemoGameObjectPool : MonoBehaviour
    {
        // 预制体或者组件
        private GameObject _gameObjectPrefab;

        private DemoGameObjectPool _componentPrefab;

        // 粒子特效
        private ParticleSystem _particleSystemPrefab;

        // 预设对象池ScriptableObject
        private PoolsPreset _poolsPreset;

        void Start()
        {
            /*------------------------------使用GameObjectPool对象池------------------------------*/

            // 使用名称或GameObject或者Component创建对象
            GameObject spawnedClone = RA.GameObjectPool.Spawn("name");
            spawnedClone = RA.GameObjectPool.Spawn(_gameObjectPrefab);
            DemoGameObjectPool component =
                RA.GameObjectPool.Spawn(_componentPrefab, Vector3.zero, Quaternion.identity, this.transform);

            // 销毁
            RA.GameObjectPool.Despawn(gameObject, delay: 0.5f);

            // 粒子特效播放完成后立即销毁
            RA.GameObjectPool
                .Spawn(_particleSystemPrefab)
                .DespawnOnComplete();

            // 如何获取对象池
            F8GameObjectPool _pool = RA.GameObjectPool.GetPoolByPrefab(_gameObjectPrefab);
            _pool = RA.GameObjectPool.GetPoolByPrefabName(_gameObjectPrefab.name);

            // 对每个池执行操作。
            RA.GameObjectPool.ForEachPool(RLog.Log);

            // 对每个克隆执行操作。
            RA.GameObjectPool.ForEachClone(RLog.Log);

            // 尝试获取克隆的状态（已生成 / 已回收 / 已生成超过容量）。
            PoolableStatus cloneStatus = RA.GameObjectPool.GetCloneStatus(spawnedClone);

            // 游戏对象是否是克隆（使用 GameObjectPool 生成）？
            bool isClone = RA.GameObjectPool.IsClone(spawnedClone);

            // 如果要销毁克隆但不回收克隆，请使用此方法以避免错误！
            RA.GameObjectPool.DestroyClone(spawnedClone);

            // 销毁所有池。
            RA.GameObjectPool.DestroyAllPools(immediately: false);



            /*------------------------------GameObjectPool对象池内功能------------------------------*/

            // 手动初始化池。如果您通过 Awake 方法访问池，而该方法在池初始化之前被调用，则可能需要这样做。
            _pool.Init();

            _pool.Init(_gameObjectPrefab);

            // 填充池。
            _pool.PopulatePool(16);

            // 设置池的容量。
            _pool.SetCapacity(32);

            // 设置池的溢出行为。
            _pool.SetBehaviourOnCapacityReached(BehaviourOnCapacityReached.Recycle);

            // 设置池中游戏对象的回收类型。
            _pool.SetDespawnType(DespawnType.DeactivateAndHide);

            // 设置池的回调类型，用于游戏对象的生成或回收。
            _pool.SetCallbacksType(CallbacksType.Interfaces);

            // 设置池的警告是否激活。
            _pool.SetWarningsActive(true);

            // 对池中的每个克隆执行操作。
            _pool.ForEachClone(RLog.Log);

            // 对池中的每个已生成的克隆执行操作。
            _pool.ForEachSpawnedClone(RLog.Log);

            // 对池中的每个已回收的克隆执行操作。
            _pool.ForEachDespawnedClone(RLog.Log);

            // 销毁已生成的克隆。
            _pool.DestroySpawnedClones();

            // 销毁已回收的克隆。
            _pool.DestroyDespawnedClones();

            // 销毁池中的所有克隆。
            _pool.DestroyAllClones();

            // 立即销毁已生成的克隆。
            _pool.DestroySpawnedClonesImmediate();

            // 立即销毁已回收的克隆。
            _pool.DestroyDespawnedClonesImmediate();

            // 立即销毁池中的所有克隆。
            _pool.DestroyAllClonesImmediate();

            // 销毁池。
            _pool.DestroyPool();

            // 立即销毁池。
            _pool.DestroyPoolImmediate();

            // 回收池中的所有克隆。
            _pool.DespawnAllClones();

            // 清除池。
            _pool.Clear();

            // 对象池事件
            void DoSomething(GameObject go)
            {

            }

            // 监听
            _pool.GameObjectInstantiated.AddListener(DoSomething);
            _pool.GameObjectSpawned.AddListener(DoSomething);
            _pool.GameObjectDespawned.AddListener(DoSomething);
            // 移除
            _pool.GameObjectInstantiated.RemoveListener(DoSomething);
            _pool.GameObjectSpawned.RemoveListener(DoSomething);
            _pool.GameObjectDespawned.RemoveListener(DoSomething);
        }

        /*------------------------------继承IPoolable的物体拥有回调------------------------------*/
        public class DemoPoolCallBack : MonoBehaviour, IPoolable
        {
            public void OnSpawn()
            {
                // Do something on spawn.
            }

            public void OnDespawn()
            {
                // Do something on despawn.
            }
        }
    }
}
