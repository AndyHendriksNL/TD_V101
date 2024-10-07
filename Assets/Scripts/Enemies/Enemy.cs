using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 6f;
    private Vector3 targetPosition;
    private Animator animator;

    private void Start()
    {
        // Haal de mainBuildingPosition op uit de BuildManager en zet deze om naar een Vector3
        Vector2Int mainBuildingGridPos = BuildManager.Instance.mainBuildingPosition;
        float cellSize = BuildManager.Instance.cellSize;
        targetPosition = new Vector3(mainBuildingGridPos.x * cellSize, 0, mainBuildingGridPos.y * cellSize); // Stel de Y-waarde in op 0 (grondniveau)
    
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", true);
        animator.SetBool("isDying", false);

    }

    private void Update()
    {
        MoveTowardsTarget();
        RotateTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        // Beweeg richting het gebouw
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Check of de vijand nog beweegt en speel de animatie af
        if (direction.magnitude > 0.1f)
        {
            animator.SetBool("isMoving", true); // Animatie afspelen
        }
        else
        {
            animator.SetBool("isMoving", false); // Animatie stoppen als hij stil staat
            animator.SetBool("isDying", true);
        }
    }

    private void RotateTowardsTarget()
    {
        // Zorg ervoor dat de vijand in de richting van de target kijkt
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // Houd de Y-waarde op 0 om te voorkomen dat hij omhoog of omlaag kijkt
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f); // Smooth rotatie
        }
    }
}
