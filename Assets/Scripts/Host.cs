using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Host : MonoBehaviour
{
    [SerializeField] private float lifeDuration;
    [SerializeField] private float lifeStart;

    protected abstract void Movement();

    protected void CheckLifeTime()
    {
        if (lifeDuration + lifeStart < Time.time)
        {
            Die();
        }
    }

    public void Die()
    {
        gameObject.transform.SetParent(null);
        gameObject.SetActive(false);
    }
}
