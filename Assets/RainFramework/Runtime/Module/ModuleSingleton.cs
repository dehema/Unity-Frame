﻿namespace Rain.Core
{
	public abstract class ModuleSingleton<T> where T : class, IModule, new()
	{
		private static T _instance;
		public static T Ins
		{
			get
			{
				if (_instance == null)
				{
#if UNITY_EDITOR
					_instance = new T();
					RLog.Log($"模块 {typeof(T)} 不是通过模块中心创建并控制（无法轮询Update），仅在编辑器下可以临时使用");
#else
					RLog.LogError($"模块 {typeof(T)} 未创建。");
#endif
                }
                return _instance;
			}
		}

		protected ModuleSingleton()
		{
			if (_instance != null)
                RLog.LogError($"模块 {typeof(T)} 实例已创建。");
			_instance = this as T;
		}
		
		protected void Destroy()
		{
			_instance = null;
		}
	}
}