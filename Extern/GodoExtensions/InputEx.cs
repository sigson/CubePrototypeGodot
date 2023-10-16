using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

public partial class InputEx : Node
{
    public Vector2 MousePosition => GetViewport().GetMousePosition();
    private IDictionary<Type, List<HandlerRecord>> eventHandlers = new ConcurrentDictionary<Type, List<HandlerRecord>>();
    private IDictionary<string, int> InputMapState = new ConcurrentDictionary<string, int>();
    public bool LockInput;

    public static InputEx Init(Node parentNode)
    {
        var node = new InputEx();
        parentNode.AddChild(node);
        return node;
    }

    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if(eventHandlers.TryGetValue(@event.GetType(), out var handlers))
        {
            foreach(var handler in handlers)
            {
                handler.Handler(@event);
            }
        }
        if(@event.IsPressed() && !(@event is InputEventMouseMotion) && !(@event is InputEventScreenDrag))
        {
            switch (@event.GetType())
            {
                case var type when type == typeof(InputEventKey):
                    InputMapState[@event.GetType().ToString() + (@event as InputEventKey).Keycode.ToString()] = 0;
                    break;
                case var type when type == typeof(InputEventMouseButton):
                    InputMapState[@event.GetType().ToString() + (@event as InputEventMouseButton).ButtonIndex.ToString()] = 0;
                    break;
                default:
                    InputMapState[@event.GetType().ToString()] = 0;
                    break;
            }
        }
        if(@event.IsReleased() && !(@event is InputEventMouseMotion) && !(@event is InputEventScreenDrag))
        {
            switch (@event.GetType())
            {
                case var type when type == typeof(InputEventKey):
                    InputMapState[@event.GetType().ToString() + (@event as InputEventKey).Keycode.ToString()] = 1;
                    break;
                case var type when type == typeof(InputEventMouseButton):
                    InputMapState[@event.GetType().ToString() + (@event as InputEventMouseButton).ButtonIndex.ToString()] = 1;
                    break;
                default:
                    InputMapState[@event.GetType().ToString()] = 1;
                    break;
            }
        }
    }

    public void AddHandler<T>(Action<InputEvent> handler, string tag = "") where T : InputEvent
    {
        List<HandlerRecord> list = null;
        if(!eventHandlers.TryGetValue(typeof(T), out list))
        {
            eventHandlers[typeof(T)] = new List<HandlerRecord>();
            list = eventHandlers[typeof(T)];
        }

        list.Add(new HandlerRecord() { Handler = handler, Tag = tag });
    }

    public void RemoveHandlerByType<T>() where T : InputEvent
    {
        List<HandlerRecord> list = null;
        if (!eventHandlers.TryGetValue(typeof(T), out list))
        {
            eventHandlers[typeof(T)] = new List<HandlerRecord>();
            list = eventHandlers[typeof(T)];
        }
        eventHandlers.Remove(typeof(T));
    }

    public void RemoveHandlerByTag(string tag)
    {
        for(int i = 0; i < eventHandlers.Values.Count; i++) 
        {
            var list = eventHandlers.Values.ElementAt(i);
            for (int j = 0; j < list.Count; j++ )
            {
                if (list[j].Tag == tag)
                {
                    list.RemoveAt(j);
                    j--;
                }
            }
        }
    }

    public void ClearHandlers()
    {
        eventHandlers.Clear();
    }

    public struct HandlerRecord
    {
        public string Tag;
        public Action<InputEvent> Handler;
    }
}
