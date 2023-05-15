using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviourSingleton<ServiceLocator>
{
    private Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

    public InputManager InputManager => GetService<InputManager>();

    public void Register(Service service)
    {
        if(!Contains(service.GetType()))
        {
            _services[service.GetType()] = service;
        }
    }

    public void UnRegister(Service service)
    {
        if(Contains(service.GetType()))
        {
            _services.Remove(service.GetType());
        }
    }

    public bool Contains(Type type)
    {
        return _services.ContainsKey(type);
    }

    public bool Contains(Service service)
    {
        return Contains(service.GetType());
    }

    private Service GetService(Type type)
    {
        if (Contains(type))
        {
            return _services[type];
        }
        return CreateService(type);
    }
    
    public T GetService<T>() where T : Service
    {
        return (T)GetService(typeof(T));
    }

    private Service CreateService(Type type)
    {
        // Create new object and component
        var newObject = new GameObject(type.Name);
        var newComponent = newObject.AddComponent(type) as Service;

        // Set parent to this gameobject
        newObject.transform.parent = transform;

        // Add service
        _services[type] = newComponent;
        return newComponent;
    }
}
