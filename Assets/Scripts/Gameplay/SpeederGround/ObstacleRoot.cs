using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(MultiplierTimerComponent))]
public class ObstacleRoot : MonoBehaviour
{
    [SerializeField]
    private MultiplierTimerComponent _knockBackMultiplierComponent;

    [SerializeField] private bool _destroyOnHit = false;

    [SerializeField] private bool _disableColliderOnHit = false;
    
    private ObstacleCollision _obstacleCollision;
    
    [SerializeField, HideInInspector]
    private List<ObstacleCollision> ObstacleColliders = new List<ObstacleCollision>();

    private void OnValidate()
    {
        if (_obstacleCollision == null) _obstacleCollision = GetComponent<ObstacleCollision>();
        if (_knockBackMultiplierComponent == null) _knockBackMultiplierComponent = GetComponent<MultiplierTimerComponent>();
    }

    private void Reset()
    {
        OnValidate();
        _knockBackMultiplierComponent.Time = 0.75f;
    }

    private void Start()
    {
        if (ObstacleColliders.Count == 0)
        {
            var obstacleColliders = GetComponentsInChildren<ObstacleCollision>();
            ObstacleColliders = obstacleColliders.ToList();
        }
        
        for (int i = ObstacleColliders.Count - 1; i >= 0; i--)
        {
            if (ObstacleColliders[i] == null)
            {
                ObstacleColliders.Remove(ObstacleColliders[i]);
                continue;
            }

            ObstacleColliders[i].CollidedEvent += OnPlayerCollided;
        }
        
        _knockBackMultiplierComponent = GetComponent<MultiplierTimerComponent>();
    }

    private void OnPlayerCollided(PlayerController player)
    {
        RaycastHit hitInfo = new RaycastHit();
        var playerLayerMask = ServiceLocator.Instance.GetService<GameManager>().PlayerLayermask;

        if (Physics.Raycast(player.transform.position, player.transform.forward, out hitInfo, 2.0f, ~playerLayerMask))
        {
            var obCol = hitInfo.transform.GetComponent<ObstacleCollision>();
            if (obCol == null) return;

            var angle = Vector3.Angle(player.transform.forward, -hitInfo.normal);
            Debug.Log("Angle: " + angle);

            if (angle > player.CollisionAngle) return;
            
            player.Collide(_knockBackMultiplierComponent);
        
            if (_destroyOnHit)
            {
                Destroy(gameObject);
            }

            if (_disableColliderOnHit)
            {
                foreach (var collider in ObstacleColliders)
                {
                    collider.SetCollider(false);
                }
            }
        }
    }

    public void CollectColliders()
    {
        var colliders = GetComponentsInChildren<Collider>().ToList();
        ResetObstacleRoot();

        foreach (var collider in colliders)
        {
            // Check if this object has a root already
            var root = collider.gameObject.GetComponent<ObstacleRoot>();
            
            if (root != null)
            {
                // Collect the children of this root
                var children = collider.gameObject.GetComponentsInChildren<Collider>();
                
                // Remove the children from this collection
                foreach (var child in children)
                {
                    colliders.Remove(child);
                }

                continue;
            }
            
            var obstacleCollision = collider.gameObject.GetComponent<ObstacleCollision>();
            if (obstacleCollision == null)
            {
              //  Undo.RecordObject((UnityEngine.Object)collider.gameObject, "Add Obstacle Collision to gameobject");
                // If the component does not exist, add it and keep track of it
                obstacleCollision = Undo.AddComponent<ObstacleCollision>(collider.gameObject);
            }

            ObstacleColliders.Add(obstacleCollision);
        }
    }

    public void PrintColliders()
    {
        Debug.Log(ObstacleColliders.Count);
    }

    public void ResetObstacleRoot()
    {
        ObstacleColliders.Clear();
    }
}