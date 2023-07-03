using UnityEngine;

public class personAnimator : MonoBehaviour
{
    public AnimationClip idleAnimation;
    public AnimationClip walkingAnimation;
    public float movementSpeed = 3f;

    public TaxiDialog taxiDialog;
    private Animator animator;
    private bool isWalking;
    private Transform target;

    private void Start()
    {
        animator = GetComponent<Animator>();
        taxiDialog = GameObject.FindAnyObjectByType<TaxiDialog>();
        isWalking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isWalking = true;
            animator.Play(walkingAnimation.name);
            target = other.transform;
        }
    }

    private void Update()
    {
        if (isWalking && target != null)
        {
            // Face the Taxi
            Vector3 targetDirection = target.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, movementSpeed * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection);

            // Move towards the Taxi
            transform.position = Vector3.MoveTowards(transform.position, target.position, movementSpeed * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= 1f)
            {
                taxiDialog.SetAnimationDetails("a");
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isWalking = false;
            animator.Play(idleAnimation.name);
            target = null;
        }
    }
}