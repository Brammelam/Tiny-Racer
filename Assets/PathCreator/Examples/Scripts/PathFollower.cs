using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 0f;
        public float distanceTravelled;
        public bool touching = false;
        public float offsetValue;

        //public newAI2 ghostAI;
        public checkShit check;

        private void Start()
        {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        private void FixedUpdate()
        {
            if (check == null) check = checkShit.FindObjectOfType<checkShit>();
            if (transform.tag == "target")
            {
                if (Input.touchCount > 0 || Input.GetMouseButton(0) || Input.GetKey("space"))
                {
                    touching = true;
                }
                else touching = false;

                if (touching)
                {
                    speed += 2f;
                    if (speed >= 100f) speed = 100f;
                }
                else
                {
                    speed -= 1.5f;
                    if (speed <= 0f) speed = 0f;
                }
            }

            distanceTravelled += speed * Time.deltaTime;

            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);

            if (distanceTravelled >= pathCreator.path.length) distanceTravelled = 0;
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        private void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}