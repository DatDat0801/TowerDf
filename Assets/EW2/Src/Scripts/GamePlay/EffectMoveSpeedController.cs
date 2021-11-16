using System;
using System.Collections;
using System.Collections.Generic;
using EW2;
using UnityEngine;

public class EffectMoveSpeedController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] particleSystems;

    private EnemyBase _owner;
    private bool _isHide;

    public void SetOwner(EnemyBase enemy)
    {
        this._owner = enemy;
    }

    private void Update()
    {
        if (this._owner != null && this._owner.IsAlive)
        {
            if (!this._owner.Renderer.enabled && !this._isHide)
            {
                this._isHide = true;
                foreach (var particle in this.particleSystems)
                {
                    particle.Stop();
                    particle.Clear();
                    particle.gameObject.SetActive(false);
                }
            }
            else if (this._owner.Renderer.enabled && this._isHide)
            {
                this._isHide = false;
                foreach (var particle in this.particleSystems)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play(true);
                }
            }
        }
    }
}