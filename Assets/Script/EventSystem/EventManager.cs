using System;
using System.Collections.Generic;

namespace Script.EventSystem
{
    /// <summary>
    /// 便于触发事件的扩展类
    /// </summary>
    public static class EventTriggerExt
    {
        /// <summary>
        /// 触发事件（无参数）
        /// </summary>
        /// <param name="sender">触发源</param>
        /// <param name="eventName">事件名</param>
        public static void TriggerEvent(this object sender, string eventName)
        {
            EventManager.Instance.TriggerEvent(eventName, sender);
        }
        /// <summary>
        /// 触发事件（有参数）
        /// </summary>
        /// <param name="sender">触发源</param>
        /// <param name="eventName">事件名</param>
        /// <param name="args">事件参数</param>
        public static void TriggerEvent(this object sender, string eventName, EventArgs args)
        {
            EventManager.Instance.TriggerEvent(eventName, sender, args);
        }
    }

    /// <summary>
    /// 全局事件管理类
    /// </summary>
    public class EventManager : Singleton<EventManager>
    {
        // 自定义事件字典
        private readonly Dictionary<string, EventHandler> eventHandlerDict = new Dictionary<string, EventHandler>();

        private void OnDestroy()
        {
            // 被摧毁时清空事件列表
            Clear();
        }

        // 订阅 (监听)
        public void AddListener(string eventName, EventHandler method)
        {
            if (!eventHandlerDict.TryAdd(eventName, method))
            {
                eventHandlerDict[eventName] += method;
            }
        }
    
        // 取消订阅 (监听)
        public void RemoveListener(string eventName, EventHandler method)
        {
            if (eventHandlerDict.ContainsKey(eventName))
            {
                eventHandlerDict[eventName] -= method;
            }
        }
    
        // 发布事件 (无参)
        public void TriggerEvent(string eventName, object sender)
        {
            if (eventHandlerDict.ContainsKey(eventName))
            {
                eventHandlerDict[eventName]?.Invoke(sender, EventArgs.Empty);
            }
        }
    
        // 发布事件 (带参)
        public void TriggerEvent(string eventName, object sender, EventArgs e)
        {
            if (eventHandlerDict.ContainsKey(eventName))
            {
                eventHandlerDict[eventName]?.Invoke(sender, e);
            }
        }
        
        // 清除所有事件
        public void Clear()
        {
            eventHandlerDict.Clear();
        }
    }
}