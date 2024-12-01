using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health;
    public float currentHealth;
    private Animator anima;
    void Start()
    {
        anima= GetComponent<Animator>();
        currentHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        if(health< currentHealth)
        {
            currentHealth=health;
            anima.SetTrigger("Attacked");
        }
        if (health <=0 )
        {
            anima.SetBool("IsDead", true);
            Debug.Log("Enemy is dead");
        }
    }
}
