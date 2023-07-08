using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Traps
{
    // Parent trap class
    public abstract class Trap : MonoBehaviour
    {
        [SerializeField] protected List<Vector3Int> _leftGridPoints, _rightGridPoints;
        [SerializeField] protected int _validationScore;
        [SerializeField] protected Vector3 _leftSpawnOffset, _rightSpawnOffset;

        public List<Vector3Int> GetLeftGridPoints()
        {
            return _leftGridPoints;
        }
        
        public List<Vector3Int> GetRightGridPoints()
        {
            return _rightGridPoints;
        }
        
        public bool IsValidScore(int score)
        {
            return score >= _validationScore;
        }
        
        // Adjusts the trap spawn position relative to an origin
        public abstract Vector3 GetLeftSpawnPoint(Vector3 origin);
        public abstract Vector3 GetRightSpawnPoint(Vector3 origin);
        
        public abstract void Detonate();
    }
}