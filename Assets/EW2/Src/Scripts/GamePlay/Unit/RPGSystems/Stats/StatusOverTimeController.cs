using System.Collections.Generic;
using Lean.Pool;
using Spine.Unity.Examples;
using UnityEngine;

namespace EW2
{
    public class StatusOverTimeController
    {
        private readonly List<StatusOverTime> _statusOverTimes;

        private readonly Dictionary<StatusType, GameObject> _dictFxStatus = new Dictionary<StatusType, GameObject>();

        private readonly Unit _owner;

        private StatusType _currentHardCc = StatusType.ModifierStatOverTime;

        public StatusOverTimeController(Unit owner)
        {
            this._statusOverTimes = new List<StatusOverTime>();

            this._owner = owner;
        }

        public void AddStatus(StatusOverTime status)
        {
            if (!status.CanApplyStatus())
            {
                return;
            }

            //Debug.Log("Add status: " + status);
            if (IsHardCC(status.Status) && CanChangeHardCC(status.Status))
            {
                this._currentHardCc = status.Status;
            }

            if (status.Stacks == false)
            {
                var allStatus = this._statusOverTimes.FindAll(x => x.Status == status.Status);
                foreach (var s in allStatus)
                {
                    s.Stop();

                    RemoveStatus(s);
                }
            }

            status.Execute();

            this._statusOverTimes.Add(status);

            AddFx(status.Status);
        }

        public void RemoveStatus(StatusOverTime status)
        {
            var result = this._statusOverTimes.Remove(status);

            if (result == false)
            {
                Debug.Log("Status is not exist");
            }
            else
            {
                Debug.Log($"<color=yellow>Remove Status: </color> {status}");
                if (IsHardCC(status.Status) && (int)this._currentHardCc == (int)status.Status)
                {
                    this._currentHardCc = GetHardCCPriorityHigher();

                    if (this._owner is EnemyBase && !IsHardCC(_currentHardCc))
                    {
                        ((EnemyBase)this._owner).CheckActionAfterStatus();
                    }
                }

                RemoveFx(status.Status);

                status.Remove();
            }
        }

        public void RemoveStatus(StatusType statusType)
        {
            for (var i = 0; i < this._statusOverTimes.Count; i++)
            {
                if (this._statusOverTimes[i].Status == statusType)
                {
                    RemoveStatus(this._statusOverTimes[i]);
                }
            }
        }

        public bool ExistStatus(StatusType statusType)
        {
            return this._statusOverTimes.Exists(s => s.Status == statusType);
        }

        public bool CanUseSkill()
        {
            return this._statusOverTimes.FindIndex(x =>
                x.Status == StatusType.Silent || x.Status == StatusType.Taunt ||
                x.Status == StatusType.Freeze) == -1;
        }

        public void RemoveAll()
        {
            this._owner.UnitState.IsLockState = false;
            foreach (var status in this._statusOverTimes)
            {
                RemoveFx(status.Status);
                status.Stop();
            }

            this._currentHardCc = StatusType.ModifierStatOverTime;
            this._statusOverTimes.Clear();
        }

        private void AddFx(StatusType statusType)
        {
            if (this._owner == null) return;

            // if (statusType == StatusType.BuffMoveSpeed)
            // {
            //     AddEffectGhost();
            //     return;
            // }

            if (statusType == StatusType.Terrify)
            {
                ((Dummy)this._owner).StatusBar.AddIconStatus(StatusType.Terrify);
                return;
            }

            if (!this._dictFxStatus.ContainsKey(statusType) ||
                (this._dictFxStatus.ContainsKey(statusType) && this._dictFxStatus[statusType] == null))
            {
                var nameFx = GetNameFx(statusType);

                if (!string.IsNullOrEmpty(nameFx))
                {
                    var posFx = GetPosFx(statusType);

                    var go = ResourceUtils.GetVfx("Status", nameFx, posFx, Quaternion.identity,
                        this._owner.Transform);

                    if (go)
                    {
                        var dummy = this._owner as Dummy;

                        if (dummy)
                        {
                            var sizeEffect = UnitSizeConstants.GetUnitSize(dummy.MySize);

                            go.transform.localScale = new Vector3(sizeEffect, sizeEffect, sizeEffect);
                        }

                        if (statusType == StatusType.BuffMoveSpeed)
                        {
                            var control = go.GetComponent<EffectMoveSpeedController>();
                            if (control != null)
                            {
                                control.SetOwner((EnemyBase)this._owner);
                            }
                        }

                        this._dictFxStatus.Add(statusType, go);
                    }
                }
            }
            else
            {
                var fx = this._dictFxStatus[statusType];

                fx.GetComponentInChildren<ParticleSystem>().Clear();

                fx.GetComponentInChildren<ParticleSystem>().Play();
            }
        }

        private void RemoveFx(StatusType statusType)
        {
            if (this._owner == null) return;

            // if (statusType == StatusType.BuffMoveSpeed)
            // {
            //     RemoveEffectGhost();
            //     return;
            // }

            if (statusType == StatusType.Terrify)
            {
                ((Dummy)this._owner).StatusBar.RemoveIconStatus(StatusType.Terrify);
                return;
            }

            if (statusType == StatusType.Freeze)
            {
                ResourceUtils.GetVfx("Status", "fx_status_freezee_impact", GetPosFx(StatusType.Freeze),
                    Quaternion.identity,
                    this._owner.Transform);
            }

            if (this._dictFxStatus.ContainsKey(statusType))
            {
                if (this._dictFxStatus[statusType] != null)
                {
                    if (statusType == StatusType.BuffMoveSpeed)
                    {
                        var control = this._dictFxStatus[statusType].GetComponent<EffectMoveSpeedController>();
                        if (control != null)
                        {
                            control.SetOwner(null);
                        }
                    }

                    LeanPool.Despawn(this._dictFxStatus[statusType]);
                }

                this._dictFxStatus.Remove(statusType);
            }
        }

        private string GetNameFx(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.Bleed:
                    return "fx_status_bleed";
                case StatusType.Silent:
                    return "fx_status_silence";
                case StatusType.Taunt:
                    return "fx_status_taunt";
                case StatusType.Stun:
                    return "fx_status_stun";
                case StatusType.HealInstantOverTime:
                    return "fx_status_bufhealth";
                case StatusType.DOT:
                    return "fx_status_burn";
                case StatusType.Poison:
                    return "fx_status_poison";
                case StatusType.BuffMoveSpeed:
                    return "fx_common_move_speed";
                case StatusType.HealPercentOverTime:
                    return "fx_status_bufhealth";
                case StatusType.Freeze:
                    return "fx_status_freezee";
                default:
                    return "";
            }
        }

        private Vector3 GetPosFx(StatusType statusType)
        {
            switch (statusType)
            {
                case StatusType.Silent:
                case StatusType.Taunt:
                case StatusType.Stun:
                    var healthBar = this._owner.Transform.Find("health_bar_frame");

                    var posFx = healthBar == null ? Vector3.zero : healthBar.localPosition;

                    return posFx;

                case StatusType.Bleed:
                case StatusType.HealInstantOverTime:
                case StatusType.DOT:
                case StatusType.Poison:
                    return Vector3.zero;
                case StatusType.BuffMoveSpeed:
                    return new Vector3(0.2f, 0.2f, 0f);

                default:
                    return Vector3.zero;
            }
        }

        private void AddEffectGhost()
        {
            var ghostController = this._owner.GetComponent<SkeletonGhost>();

            if (ghostController == null)
                ghostController = this._owner.gameObject.AddComponent<SkeletonGhost>();

            if (ghostController)
            {
                if (ghostController.ghostShader == null)
                    ghostController.ghostShader = ResourceUtils.GetShader("Spine-Special-Skeleton-Ghost");

                ghostController.color = new Color(1f, 1f, 1f, 0.8f);

                ghostController.spawnInterval = 0.2f;

                ghostController.maximumGhosts = 8;

                ghostController.fadeSpeed = 3;

                ghostController.textureFade = 0;

                ghostController.ghostingEnabled = true;
            }
        }

        private void RemoveEffectGhost()
        {
            var ghostController = this._owner.GetComponent<SkeletonGhost>();

            if (ghostController)
            {
                ghostController.ghostingEnabled = false;
            }
        }

        private bool IsHardCC(StatusType statusType)
        {
            return statusType == StatusType.Stun || statusType == StatusType.Taunt ||
                   statusType == StatusType.Terrify || statusType == StatusType.Freeze;
        }

        private bool CanChangeHardCC(StatusType statusType)
        {
            return (int)statusType < (int)this._currentHardCc;
        }

        private StatusType GetHardCCPriorityHigher()
        {
            var results = StatusType.ModifierStatOverTime;

            foreach (var status in this._statusOverTimes)
            {
                if (IsHardCC(status.Status) && (int)status.Status < (int)results)
                {
                    results = status.Status;
                }
            }

            return results;
        }

        public bool CanDoActionCCStatus(StatusType statusType)
        {
            return (int)statusType <= (int)this._currentHardCc;
        }

        public void CompleteStatus(StatusOverTime status)
        {
            var result = this._statusOverTimes.Remove(status);

            if (result == false)
            {
                Debug.Log("Status is not exist");
            }
            else
            {
                Debug.Log($"<color=yellow>Remove Status: </color> {status}");
                if (IsHardCC(status.Status) && (int)this._currentHardCc == (int)status.Status)
                {
                    this._currentHardCc = GetHardCCPriorityHigher();
                }

                RemoveFx(status.Status);
                status.Complete();
            }
        }
    }
}