using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace Rain.Core
{
    /// <summary>
    /// 日志管理器类
    /// 负责处理游戏运行时的日志记录，将日志写入文件并管理日志文件
    /// </summary>
    [UpdateRefresh]
    public class LogMgr : ModuleSingleton<LogMgr>, IModule
    {
        // 最大日志文件数量，超过此数量将删除旧日志文件
        private int MAX_LOG_FILE_CNT = 7;
        // 日志写入时间间隔（秒）
        private float LOG_TIME = 0.5f;
        // 是否启用日志记录
        private bool isEnableLog = false;

        /// <summary>
        /// 日志信息结构体
        /// 用于存储单条日志的类型、消息内容和堆栈跟踪信息
        /// </summary>
        struct log_info
        {
            public LogType type;      // 日志类型
            public string msg;        // 日志消息
            public string stackTrace;  // 堆栈跟踪信息

            public log_info(LogType type, string msg, string stackTrace = null)
            {
                this.type = type;
                this.msg = msg;
                this.stackTrace = stackTrace;
            }
        }

        // 后台日志列表，用于线程安全地收集日志
        private List<log_info> backList = new List<log_info>(100);
        // 前台日志列表，用于处理收集到的日志
        private List<log_info> frontList = new List<log_info>(100);

        // 存储直接通过Unity日志系统收集的日志信息
        private List<log_info> logInfos = new List<log_info>();

        // 日志文件写入器
        private StreamWriter writer;
        // 上次写入日志的时间
        private float logTime;

        /// <summary>
        /// 模块初始化方法
        /// </summary>
        /// <param name="createParam">创建参数</param>
        public void OnInit(object createParam)
        {
            // 初始化逻辑（当前为空）
        }

        /// <summary>
        /// 每帧更新方法，处理日志写入操作
        /// </summary>
        public void OnUpdate()
        {
            // 仅在非编辑器模式、日志功能启用且写入器可用时执行
            if (!Application.isEditor && isEnableLog && writer != null)
            {
                // 检查是否达到日志写入时间间隔
                if (Time.realtimeSinceStartup - logTime > LOG_TIME)
                {
                    // 处理Unity日志系统收集的日志
                    lock (backList)
                    {
                        if (logInfos.Count > 0)
                        {
                            // 写入所有收集到的日志
                            for (int i = 0; i < logInfos.Count; i++)
                            {
                                var logInfo = logInfos[i];
                                LogOneMessage(logInfo.type, logInfo.msg, logInfo.stackTrace);
                            }

                            // 清空已处理的日志
                            logInfos.Clear();
                            // 刷新写入器，确保日志写入文件
                            writer.Flush();
                        }
                    }

                    // 线程安全地交换前后台日志列表
                    lock (backList)
                    {
                        if (backList.Count > 0)
                        {
                            // 使用元组交换前后台列表，避免复制开销
                            (frontList, backList) = (backList, frontList);
                        }
                    }

                    // 处理前台列表中的日志
                    if (frontList.Count > 0)
                    {
                        for (int i = 0; i < frontList.Count; i++)
                        {
                            var logInfo = frontList[i];
                            LogOneMessage(logInfo.type, logInfo.msg);
                        }

                        // 清空已处理的日志
                        frontList.Clear();
                        // 刷新写入器，确保日志写入文件
                        writer.Flush();
                    }

                    // 更新日志时间
                    logTime = Time.realtimeSinceStartup;
                }
            }
        }

        /// <summary>
        /// 在所有Update函数调用后执行的更新方法
        /// </summary>
        public void OnLateUpdate()
        {
            // 当前未实现
        }

        /// <summary>
        /// 固定时间间隔的更新方法
        /// </summary>
        public void OnFixedUpdate()
        {
            // 当前未实现
        }

        /// <summary>
        /// 模块终止方法，在模块被销毁时调用
        /// </summary>
        public void OnTermination()
        {
            // 调用基类的销毁方法
            base.Destroy();
        }
        
        /// <summary>
        /// 进入游戏时调用，初始化日志系统
        /// </summary>
        public void OnEnterGame()
        {
            // 启用日志记录
            isEnableLog = true;
            
            // 获取当前时间，用于日志文件命名
            var nowTime = DateTime.Now;
            
            // 在编辑器模式下不进行文件日志记录
            if (Application.isEditor) return;
            
            // 注册Unity日志回调函数
            Application.logMessageReceived += (LogHandler);

            // 获取现有日志文件列表
            var files = Directory.GetFiles(Application.persistentDataPath, "log-*.txt",
                SearchOption.TopDirectoryOnly);
            // 如果日志文件数量超过最大限制，删除最旧的文件
            if (files.Length > MAX_LOG_FILE_CNT)
            {
                for (int i = 0; i < files.Length - MAX_LOG_FILE_CNT; i++)
                {
                    File.Delete(files[i]);
                }
            }

            // 生成新日志文件路径，格式为：log-年-月-日.txt
            var logFilePath = string.Format("{0}/log-{1}-{2}-{3}.txt", Application.persistentDataPath, nowTime.Year,
                nowTime.Month, nowTime.Day);

            try
            {
                // 尝试创建日志文件写入器，追加模式，UTF8编码
                writer = new StreamWriter(logFilePath, true, Encoding.UTF8);
            }
            catch (Exception e)
            {
                try
                {
                    // 如果创建失败，尝试使用带有唯一标识符的文件名再次创建
                    writer = new StreamWriter(string.Format("{0}/log-{1}-{2}-{3}-{4}.txt", Application.persistentDataPath, nowTime.Year,
                        nowTime.Month, nowTime.Day, Guid.NewGuid().GetHashCode()), true, Encoding.UTF8);
                }
                catch (Exception e2)
                {
                    // 如果再次失败，禁用日志写入器并记录异常
                    writer = null;
                    RLog.LogException(e2);
                }
            }
            
            // 重置日志时间
            logTime = 0;
        }

        /// <summary>
        /// 退出游戏时调用，清理日志系统资源
        /// </summary>
        public void OnQuitGame()
        {
            // 禁用日志记录
            isEnableLog = false;
            // 线程安全地清空后台日志列表
            lock (backList)
            {
                backList.Clear();
            }

            // 清空前台日志列表
            frontList.Clear();
            // 关闭并释放日志写入器
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }
        
        /// <summary>
        /// 将单条日志消息写入文件
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="msg">日志消息</param>
        /// <param name="stackTrace">堆栈跟踪信息（可选）</param>
        private void LogOneMessage(LogType type, string msg, string stackTrace = null)
        {
            // 是否需要记录堆栈跟踪
            var logStackTrace = false;
            // 日志类型简写
            var typeStr = "";
            // 根据日志类型设置对应的标识和是否需要堆栈跟踪
            switch (type)
            {
                case LogType.Log:
                    typeStr = "L"; // 普通日志
                    break;
                case LogType.Warning:
                    typeStr = "W"; // 警告
                    break;
                case LogType.Error:
                case LogType.Exception:
                case LogType.Assert:
                    typeStr = "E"; // 错误、异常和断言
                    logStackTrace = true; // 需要记录堆栈跟踪
                    break;
            }

            // 格式化日志消息：[LOG]|时间|类型|消息
            var str = string.Format("[LOG]|{0}|{1}|{2}\n", DateTime.Now, typeStr, msg);
            // 如果需要且有堆栈跟踪信息，则添加到日志中
            if (logStackTrace && stackTrace != null)
            {
                str += stackTrace;
            }

            // 写入日志文件
            writer.Write(str);
        }

        /// <summary>
        /// Unity日志回调处理函数
        /// </summary>
        /// <param name="condition">日志消息</param>
        /// <param name="stackTrace">堆栈跟踪信息</param>
        /// <param name="type">日志类型</param>
        private void LogHandler(string condition, string stackTrace, LogType type)
        {
            // 在编辑器模式下不处理日志
            if (Application.isEditor)
            {
                return;
            }
            // 忽略警告类型的日志，只记录其他类型
            if (type != LogType.Warning)
            {
                // 将日志添加到待处理列表
                logInfos.Add(new log_info(type, condition, stackTrace));
            }
        }
        
        /// <summary>
        /// 从其他线程安全地将日志发送到主线程处理
        /// </summary>
        /// <param name="type">日志类型</param>
        /// <param name="msg">日志消息</param>
        public void LogToMainThread(LogType type, string msg)
        {
            // 使用锁保证线程安全
            lock (backList)
            {
                // 将日志添加到后台列表，等待主线程处理
                backList.Add(new log_info(type, msg));
            }
        }

    }
}

