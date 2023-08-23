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

    [SerializeField, Tooltip("Destroys the whole obstacle root when Destroy Trigger is false"), HideInInspector]
    private bool _destroyTrigger = false;

    [SerializeField, Tooltip("If destroy after a while is true, the obstacle root or" +
                             " its trigger will be destroyed after the set time"), HideInInspector]
    private float _destroyAfterWhile = 0.0f;

    [SerializeField] private bool _disableColliderOnHit = false;

    private ObstacleCollision _obstacleCollision;

    [SerializeField, HideInInspector]
    private List<ObstacleCollision> ObstacleColliders = new List<ObstacleCollision>();

    public MultiplierTimerComponent KnockBackMultiplierComponent => _knockBackMultiplierComponent;

    private void OnValidate()
    {
        var otherObstacleRoot = gameObject.GetComponent<ObstacleRoot>();
        if (otherObstacleRoot != null && otherObstacleRoot != this)
        {
            Debug.LogError("Can't add more than two obstacle roots on one object!", gameObject);
            DestroyImmediate(this);
            return;
        }

        if (_obstacleCollision == null) _obstacleCollision = GetComponent<ObstacleCollision>();
        if (_knockBackMultiplierComponent == null)
            _knockBackMultiplierComponent = GetComponent<MultiplierTimerComponent>();
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

    private void OnPlayerCollided(PlayerController player, ObstacleCollision obstacle)
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

            // do not call this if I'm in space, only ground
            if (player.GetComponent<SpeederGround>() != null)
            {
                ServiceLocator.Instance.GetService<LevelManager>().PlayerHitObstacle();
            }
            
            
            if (_disableColliderOnHit)
            {
                foreach (var collider in ObstacleColliders)
                {
                    collider.SetCollider(false);
                }
            }

            if (_destroyOnHit)
            {
                GameObject toDestroyGameObject = _destroyTrigger ? obstacle.gameObject : gameObject;
                StartCoroutine(DestroyAfterWhile(_destroyAfterWhile, toDestroyGameObject));
            }
        }
    }

    private IEnumerator DestroyAfterWhile(float time, GameObject gameObjectToDestroy)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObjectToDestroy);
    }
    
#if UNITY_EDITOR
    public void CollectColliders()
    {
        var children = GetComponentsInChildren<Transform>().ToList();
        ResetObstacleRoot();

        var otherRootColliders = new List<Collider>();

        foreach (var t in children)
        {
            // skip ourselves
            if (t == transform) continue;

            // Check if this object has a root already
            var root = t.GetComponent<ObstacleRoot>();

            if (root != null)
            {
                // Collect the children of this root
                var rootChildren = root.GetComponentsInChildren<Collider>();
                otherRootColliders.AddRange(rootChildren);

                //// Remove the children from this collection
                //foreach (var child in children)
                //{
                //    colliders.Remove(child);
                //}

                continue;
            }

            // Check if t has a collider
            var col = t.GetComponent<Collider>();
            if (col == null) continue;

            // Check if this collider is part of another root collider
            if (otherRootColliders.Find(x => x == col))
            {
                otherRootColliders.Remove(col);
                continue;
            }

            var obstacleCollision = t.GetComponent<ObstacleCollision>();
            if (obstacleCollision == null)
            {
                //  Undo.RecordObject((UnityEngine.Object)collider.gameObject, "Add Obstacle Collision to gameobject");
                // If the component does not exist, add it and keep track of it
                obstacleCollision = Undo.AddComponent<ObstacleCollision>(t.gameObject);
            }

            ObstacleColliders.Add(obstacleCollision);
        }
    }
#endif

    public void PrintColliders()
    {
        Debug.Log(ObstacleColliders.Count);
    }

    public void ResetObstacleRoot()
    {
        ObstacleColliders.Clear();
    }
}