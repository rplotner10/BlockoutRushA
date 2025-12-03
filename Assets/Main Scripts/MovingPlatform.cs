using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]

public class MovingPlatform : MonoBehaviour
{
   [Tooltip("Movement Speed of Platform.")] 
   [SerializeField] float _speed = 2f;

   Vector3[] _waypointPositions;
   int _currentWaypointIndex = 0;
   bool _movingForward = true;
   Rigidbody _rigidbody;

   void Awake()
   {
      _rigidbody = GetComponent<Rigidbody>();
      //Use kinematic rigidbody so physics objects stick properly
      _rigidbody.isKinematic = true;
   }

   void Start()
   {
      int count = transform.childCount;
      if (count < 2)
      {
         Debug.LogWarning($"MovingPlatform ({name}): Need at least 2 child waypoints to move.");
         return;
      }
      // Cache world-space positions of each waypoint so they remain static
      _waypointPositions = new Vector3[count];
      for (int i = 0; i < count; i++)
      {
         _waypointPositions[i] = transform.GetChild(i).position;
      }
   }
   void FixedUpdate()
   {
      if (_waypointPositions == null || _waypointPositions.Length < 2)
         return;

      Vector3 targetPos = _waypointPositions[_currentWaypointIndex];
      Vector3 newPos = Vector3.MoveTowards(_rigidbody.position, targetPos, _speed * Time.fixedDeltaTime);
      _rigidbody.MovePosition(newPos);

      // If close to the target, advance to next
      if (Vector3.Distance(newPos, targetPos) < 0.05f)
      {
         AdvanceWaypoint();
      }
   }
   void AdvanceWaypoint()
   {
      if (_movingForward)
      {
         _currentWaypointIndex++;
         if (_currentWaypointIndex >= _waypointPositions.Length)
         {
            _movingForward = false;
            _currentWaypointIndex = _waypointPositions.Length - 2;
         }
      }
      else
      {
         _currentWaypointIndex--;
         if (_currentWaypointIndex < 0)
         {
            _movingForward = true;
            _currentWaypointIndex = 1;
         }
      }
   }
   void OnCollisionEnter(Collision collision)
   {
      // Stick the player to the platform when they land on it
      if (collision.collider.CompareTag("Player"))
      {
         collision.collider.transform.SetParent(transform);
         //collision.collider.attachedRigidbody.isKinematic = true;
      }
   }

   void OnCollisionExit(Collision collision)
   {
      // Unparent when the player leaves
      if (collision.collider.CompareTag("Player"))
      {
         collision.collider.transform.SetParent(null);
         //collision.collider.attachedRigidbody.isKinematic = false;
      }
   }
}
