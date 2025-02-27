﻿using System.Collections;
using UnityEngine;

public class Survivor : Character
{
    #region EXPOSED_FIELDS

    [SerializeField] private float crazinessPoints = 0f;
    [SerializeField] private int points = 0;
    [SerializeField] private float deathTimer = 0f;
    [SerializeField] private Animator anim = null;
    [SerializeField] private ParticleSystem blood = null;

    #endregion

    #region PRIVATE_FIELDS

    private bool dodged = false;
    private float checkDistance = 5f;
    private Vector3 startPos = Vector3.zero;
    private MovableObject movable = null;

    #endregion

    #region PROPERTIES

    public float CrazinessPoints => crazinessPoints;

    public int Points => points;

    #endregion

    #region UNITY_CALLS

    protected override void Awake()
    {
        base.Awake();

        movable = GetComponent<MovableObject>();
    }

    private void Update()
    {
        if (!dead)
            DodgeObstacle();
    }

    private void OnEnable()
    {
        Init();
    }

    #endregion

    #region PUBLIC_METHODS

    public void Death()
    {
        if (dead)
            return;

        dead = true;
        anim.SetBool("Death", true);
        blood.Play();
        AkSoundEngine.PostEvent("cha_axe_success", gameObject);

        rigid.useGravity = false;
        rigid.velocity = Vector3.zero;
        capsule.isTrigger = true;

        Invoke(nameof(DestroySurvivor), deathTimer);
    }

    #endregion

    #region PRIVATE_METHODS

    private void Init()
    {
        dead = false;
        anim.SetBool("Death", false);
        rigid.useGravity = true;
        capsule.isTrigger = false;
    }

    private void DodgeObstacle()
    {
        if (dodged)
            return;

        startPos = transform.position;
        startPos.y -= capsule.height / 4;

        if (Physics.Raycast(startPos, Vector3.forward, out RaycastHit hit, checkDistance, obstacleMask))
        {
            dodged = true;

            Vector3 hitPos = hit.transform.position;
            BoxCollider col = hit.transform.gameObject.GetComponent<BoxCollider>();
            float colWidth = col.bounds.size.x;

            IEnumerator MovePosition()
            {
                if (transform.position.x < hitPos.x)
                {
                    while (transform.position.x > hitPos.x - colWidth)
                    {
                        transform.Translate(Vector3.left * (horizontalSpeed * Time.deltaTime));
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    while (transform.position.x < hitPos.x + colWidth)
                    {
                        transform.Translate(Vector3.right * (horizontalSpeed * Time.deltaTime));
                        yield return new WaitForEndOfFrame();
                    }
                }

                dodged = false;
                yield return null;
            }

            StartCoroutine(MovePosition());
        }
    }

    private void DestroySurvivor()
    {
        movable.Destroy();
    }

    #endregion
}
