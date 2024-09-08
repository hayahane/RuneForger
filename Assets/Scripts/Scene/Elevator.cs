using KinematicCharacterController;
using UnityEngine;

namespace RuneForger.Scene
{
    public class Elevator : MonoBehaviour, IMoverController
    {
        [SerializeField]
        private float speed = 5f;
        [SerializeField]
        private Vector3[] waypointsOffset = new Vector3[2];
        private readonly Vector3[] _waypoints = new Vector3[2];
        private int _currentWaypointIndex = 0;
        private bool _isOperating = false;
        
        private void Awake()
        {
            var physicsMover = GetComponent<PhysicsMover>();
            if (physicsMover)
                physicsMover.MoverController = this;
        }

        private void Start()
        {
            _waypoints[0] = waypointsOffset[0] + transform.position;
            _waypoints[1] = waypointsOffset[1] + transform.position;
        }

        void IMoverController.UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            goalRotation = transform.rotation;
            if (!_isOperating)
            {
                goalPosition = transform.position;
            }
            var targetPos = Vector3.MoveTowards(transform.position, _waypoints[_currentWaypointIndex], 
                    speed * deltaTime);
            goalPosition = targetPos;
            
            if (Vector3.Distance(targetPos, _waypoints[_currentWaypointIndex]) < 0.1f)
            {
                _isOperating = false;
            }
        }
        
        // Start the elevator, 由外部调用
        public void NextLevel()
        {
            _currentWaypointIndex += 1;
            _currentWaypointIndex %= 2;
            _isOperating = true;
        }

        public void ChangeToLevel(int level)
        {
            if (level < 0 || level >= _waypoints.Length)
            {
                Debug.LogError("Invalid level");
                return;
            }
            _currentWaypointIndex = level;
        }
        

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.5f, 0.4f, 1f, 0.3f);
            Gizmos.DrawCube(waypointsOffset[0] + transform.position, new Vector3(3,0.1f,3));
            Gizmos.DrawCube(waypointsOffset[1] + transform.position, new Vector3(3,0.1f,3));
            Gizmos.DrawLine(waypointsOffset[0] + transform.position, waypointsOffset[1] + transform.position);;
        }
    }
}