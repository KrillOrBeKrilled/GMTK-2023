using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Input
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class TrapOverlap : MonoBehaviour
    {
        private Collider2D _currentCollision;

        public Collider2D GetCollisionData()
        {
            return _currentCollision;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Trap"))
            {
                _currentCollision = other;
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Trap"))
            {
                _currentCollision = null;
            }
        }
    }

}
