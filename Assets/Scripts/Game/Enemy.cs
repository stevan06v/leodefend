﻿using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Health,AttackPower,MoveSpeed
    public int health,attackPower,incomeValue;
    public float moveSpeed;

    public Animator animator;
    public float attackInterval;
    Coroutine attackOrder;
    Tower detectedTower;

    void Update()
    {
        if(!detectedTower)
        {
            Move();
        }
        
    }

    IEnumerator Attack()
    {
        animator.Play("Attack", 0, 0);
        //Wait attackInterval 
        yield return new WaitForSeconds(attackInterval);
        //Attack Again
        attackOrder = StartCoroutine(Attack());
    }

    //Moving forward
    void Move()
    {
        animator.Play("Move", 0);
        transform.Translate(-transform.right*moveSpeed*Time.deltaTime);
    }

    public void InflictDamage()
    {
        if (detectedTower != null){
                        
            bool towerDied = detectedTower.LoseHealth(attackPower);
            if (towerDied)
            {
                detectedTower = null;
                StopCoroutine(attackOrder);
            }
        }
    }

    //Lose health
    public void LoseHealth()
    {
        //Decrease health value
        health--;
        animator.SetTrigger("TakeDamage");
        StartCoroutine(BlinkRed());
        animator.SetTrigger("DoMove");

        //Check if health is zero => destroy enemy
        if(health<=0){
            GameManager.instance.currency.Gain(incomeValue);
            Destroy(gameObject);
        }
    }

    IEnumerator BlinkRed()
    {
        //Change the spriterendere color to red
        GetComponent<SpriteRenderer>().color=Color.red;
        //Wait for really small amount of time 
        yield return new WaitForSeconds(0.2f);
        //Revert to default color
        GetComponent<SpriteRenderer>().color=Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (detectedTower)
            return;

        if(collision.tag == "Tower")
        {
            detectedTower = collision.GetComponent<Tower>();
            attackOrder = StartCoroutine(Attack());
        }   

        if(collision.tag == "Out"){
            Destroy(gameObject);
            GameManager.instance.health.LoseHealth();
        }
    }


}
