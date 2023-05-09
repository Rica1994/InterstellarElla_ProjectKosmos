using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviourSingleton<ServiceLocator>
{
    private Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

    public InputManager InputManager
    {
        get
        {
            return (InputManager)GetService(typeof(InputManager));
        }
    }

    public void Register(Service service)
    {
        if(!Contains(service))
        {
            _services[service.GetType()] = service;
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

    private Service CreateService(Type type)
    {
        // Create new object and component
        var newObject = new GameObject();
        var newComponent = newObject.AddComponent(type) as Service;

        // Set name and parent to this gameobject
        newObject.name = type.Name;
        newObject.transform.parent = transform;

        // Add service
        _services[type] = newComponent;
        return newComponent;
    }
}
