using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rain.Core
{
    public static class ModuleCenter
    {
        private class ModuleWrapper
        {
            public int Priority = 0;
            public IModule Module = null;
            public bool ShouldBeRemoved = false;

            public ModuleWrapper(IModule module, int priority)
            {
                Module = module;
                Priority = priority;
            }
        }

        private static MonoBehaviour _behaviour;
        private static List<ModuleWrapper> _coms = new List<ModuleWrapper>(100);
        private static List<ModuleWrapper> _comsUpdate = new List<ModuleWrapper>(100);
        private static List<ModuleWrapper> _comsLateUpdate = new List<ModuleWrapper>(100);
        private static List<ModuleWrapper> _comsFixedUpdate = new List<ModuleWrapper>(100);

        //�Ƿ���Ҫ��������
        private static bool _isDirty = false;
        private static bool _isDirtyLate = false;
        private static bool _isDirtyFixed = false;

        //���е�֡��
        private static long _frame = 0;

        /// <summary>
		/// ��ʼ�����
		/// </summary>
		public static void Initialize(MonoBehaviour behaviour)
        {
            if (behaviour == null)
                Debug.LogError("MonoBehaviour Ϊ�ա�");
            if (_behaviour != null)
            {
                RLog.LogModule($"{nameof(ModuleCenter)} �ѳ�ʼ����");
                return;
            }

            UnityEngine.Object.DontDestroyOnLoad(behaviour.gameObject);
            _behaviour = behaviour;

            behaviour.StartCoroutine(CheckFrame());
        }

        /// <summary>
        /// ���ModuleCenter���·���
        /// </summary>
        private static IEnumerator CheckFrame()
        {
            var wait = new WaitForSeconds(1f);
            yield return wait;

            // ˵������ʼ��֮��������Ǹ���ModuleCenter
            if (_frame == 0)
            {
                RLog.LogModule($"��δ������ѯ������ModuleCenter.Update");
                RLog.LogModule($"��δ������ѯ������ModuleCenter.LateUpdate");
                RLog.LogModule($"��δ������ѯ������ModuleCenter.FixedUpdate");
            }
        }

        /// <summary>
        /// ���¿��
        /// </summary>
        public static void Update()
        {
            _frame++;

            // �������ģ����Ҫ��������
            if (_isDirty)
            {
                _isDirty = false;

                _comsUpdate.Sort((left, right) =>
                {
                    if (left.Priority < right.Priority)
                        return -1;
                    else if (left.Priority == right.Priority)
                        return 0;
                    else
                        return 1;
                });
            }

            // ��������ģ��
            for (int i = 0; i < _comsUpdate.Count; i++)
            {
                if (_comsUpdate[i]?.Module == null || _comsUpdate[i].ShouldBeRemoved)
                {
                    _comsUpdate.RemoveAt(i);
                    i--;
                    continue;
                }

                // ִ��ģ�����
                _comsUpdate[i].Module.OnUpdate();
            }
        }

        /// <summary>
        /// ���¿��
        /// </summary>
        public static void LateUpdate()
        {
            // �������ģ����Ҫ��������
            if (_isDirtyLate)
            {
                _isDirtyLate = false;

                _comsLateUpdate.Sort((left, right) =>
                {
                    if (left.Priority < right.Priority)
                        return -1;
                    else if (left.Priority == right.Priority)
                        return 0;
                    else
                        return 1;
                });
            }

            // ��������ģ��
            for (int i = 0; i < _comsLateUpdate.Count; i++)
            {
                if (_comsLateUpdate[i]?.Module == null || _comsLateUpdate[i].ShouldBeRemoved)
                {
                    _comsLateUpdate.RemoveAt(i);
                    i--;
                    continue;
                }

                // ִ��ģ�����
                _comsLateUpdate[i].Module.OnLateUpdate();
            }
        }

        /// <summary>
        /// ���¿��
        /// </summary>
        public static void FixedUpdate()
        {
            // �������ģ����Ҫ��������
            if (_isDirtyFixed)
            {
                _isDirtyFixed = false;

                _comsFixedUpdate.Sort((left, right) =>
                {
                    if (left.Priority < right.Priority)
                        return -1;
                    else if (left.Priority == right.Priority)
                        return 0;
                    else
                        return 1;
                });
            }

            // ��������ģ��
            for (int i = 0; i < _comsFixedUpdate.Count; i++)
            {
                if (_comsFixedUpdate[i]?.Module == null || _comsFixedUpdate[i].ShouldBeRemoved)
                {
                    _comsFixedUpdate.RemoveAt(i);
                    i--;
                    continue;
                }

                // ִ��ģ�����
                _comsFixedUpdate[i].Module.OnFixedUpdate();
            }
        }

        /// <summary>
        /// ��ѯ��Ϸģ���Ƿ����
        /// </summary>
        public static bool Contains<T>() where T : class, IModule
        {
            System.Type type = typeof(T);
            return Contains(type);
        }

        /// <summary>
        /// ��ѯ��Ϸģ���Ƿ����
        /// </summary>
        public static bool Contains(System.Type moduleType)
        {
            for (int i = 0; i < _coms.Count; i++)
            {
                if (_coms[i].Module.GetType() == moduleType)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// ������Ϸģ��
        /// </summary>
        /// <typeparam name="T">ģ����</typeparam>
        /// <param name="priority">����ʱ�����ȼ������ȼ�ԽСԽ��ִ�С����û���������ȼ�����ô�ᰴ�����˳��ִ��</param>
        public static T CreateModule<T>(int priority = 0) where T : class, IModule
        {
            return CreateModule<T>(null, priority);
        }

        /// <summary>
        /// ������Ϸģ��
        /// </summary>
        /// <typeparam name="T">ģ����</typeparam>
        /// <param name="createParam">��������</param>
        /// <param name="priority">����ʱ�����ȼ������ȼ�ԽСԽ��ִ�С����û���������ȼ�����ô�ᰴ�����˳��ִ��</param>
        public static T CreateModule<T>(System.Object createParam, int priority = 0) where T : class, IModule
        {
            if (priority < 0)
            {
                RLog.LogModule("ģ�����ȼ�����Ϊ��");
                priority = 0;
            }

            if (Contains(typeof(T)))
            {
                RLog.LogModule($"��Ϸģ�� {typeof(T)} �Ѵ���");
                return GetModule<T>();
            }

            // ���û���������ȼ�
            if (priority == 0)
            {
                int minPriority = GetMaxPriority();
                priority = ++minPriority;
            }

            RLog.LogModule($"������Ϸģ��: {typeof(T).Name}");

            T module = null;

            // ��������Ƿ��� MonoBehaviour ������
            if (typeof(MonoBehaviour).IsAssignableFrom(typeof(T)))
            {
                GameObject obj = new GameObject(typeof(T).Name, typeof(T));
                module = obj.GetComponent<T>();
            }
            else
            {
                module = Activator.CreateInstance<T>();
            }

            ModuleWrapper wrapper = new ModuleWrapper(module, priority);
            wrapper.Module.OnInit(createParam);
            _coms.Add(wrapper);
            _coms.Sort((left, right) =>
            {
                if (left.Priority < right.Priority)
                    return -1;
                else if (left.Priority == right.Priority)
                    return 0;
                else
                    return 1;
            });
            if (typeof(T).GetCustomAttributes(typeof(UpdateRefreshAttribute), false).Length > 0)
            {
                _comsUpdate.Add(wrapper);
                _isDirty = true;
            }
            if (typeof(T).GetCustomAttributes(typeof(LateUpdateRefreshAttribute), false).Length > 0)
            {
                _comsLateUpdate.Add(wrapper);
                _isDirtyLate = true;
            }
            if (typeof(T).GetCustomAttributes(typeof(FixedUpdateRefreshAttribute), false).Length > 0)
            {
                _comsFixedUpdate.Add(wrapper);
                _isDirtyFixed = true;
            }
            return module;
        }

        /// <summary>
        /// ����ģ��
        /// </summary>
        /// <typeparam name="T">ģ����</typeparam>
        public static bool DestroyModule<T>()
        {
            var moduleType = typeof(T);
            for (int i = 0; i < _comsUpdate.Count; i++)
            {
                if (_comsUpdate[i].Module.GetType() == moduleType)
                {
                    _comsUpdate[i].ShouldBeRemoved = true;
                }
            }

            for (int i = 0; i < _comsLateUpdate.Count; i++)
            {
                if (_comsLateUpdate[i].Module.GetType() == moduleType)
                {
                    _comsLateUpdate[i].ShouldBeRemoved = true;
                }
            }

            for (int i = 0; i < _comsFixedUpdate.Count; i++)
            {
                if (_comsFixedUpdate[i].Module.GetType() == moduleType)
                {
                    _comsFixedUpdate[i].ShouldBeRemoved = true;
                }
            }

            for (int i = 0; i < _coms.Count; i++)
            {
                if (_coms[i].Module.GetType() == moduleType)
                {
                    _coms[i].Module.OnTermination();
                    _coms.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ��ȡ��Ϸģ��
        /// </summary>
        /// <typeparam name="T">ģ����</typeparam>
        public static T GetModule<T>() where T : class, IModule
        {
            System.Type type = typeof(T);
            for (int i = 0; i < _coms.Count; i++)
            {
                if (_coms[i].Module.GetType() == type)
                    return _coms[i].Module as T;
            }

            RLog.LogError($"δ�ҵ���Ϸģ�� {type}");
            return null;
        }

        /// <summary>
        /// ��ȡ��ǰģ�����������ȼ���ֵ
        /// </summary>
        private static int GetMaxPriority()
        {
            int maxPriority = int.MinValue; // ��ʼ��Ϊ int ���͵���Сֵ
            for (int i = 0; i < _coms.Count; i++)
            {
                if (_coms[i].Priority > maxPriority)
                    maxPriority = _coms[i].Priority;
            }
            return maxPriority; // ���ڵ�����
        }


        /// <summary>
        /// ��ȡ��ʼ��MonoBehaviour
        /// </summary>
        public static MonoBehaviour GetBehaviour()
        {
            if (_behaviour == null)
                RLog.LogError($"{nameof(ModuleCenter)} δ��ʼ����ʹ�� ModuleCenter.Initialize");
            return _behaviour;
        }
    }
}