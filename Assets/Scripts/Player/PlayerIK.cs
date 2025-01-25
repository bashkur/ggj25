using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    public Transform LeftHandIKTarget;
    public Transform RightHandIKTarget;
    public Transform LeftElbowIKTarget;
    public Transform RightElbowIKTarget;

    [Range(0, 1f)]
    public float HandIKAmount = 1f;
    [Range(0, 1f)]
    public float ElbowIKAmount = 1f;

    [SerializeField]
    private Animator _animator;

    private RigBuilder _rigBuilder;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigBuilder = GetComponent<RigBuilder>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (LeftHandIKTarget != null)
        {
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIKAmount);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIKAmount);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
        }
        if (RightHandIKTarget != null)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIKAmount);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIKAmount);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
        }
        if (LeftElbowIKTarget != null)
        {
            _animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
            _animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIKAmount);
        }

        if (RightElbowIKTarget != null)
        {
            _animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);
            _animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIKAmount);
        }
    }

    public void Setup(Transform GunParent)
    {
        Transform[] allChildren = GunParent.GetComponentsInChildren<Transform>();
        LeftElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftElbow");
        RightElbowIKTarget = allChildren.FirstOrDefault(child => child.name == "RightElbow");
        LeftHandIKTarget = allChildren.FirstOrDefault(child => child.name == "LeftHand");
        RightHandIKTarget = allChildren.FirstOrDefault(child => child.name == "RightHand");
        _rigBuilder.enabled = true;
    }
    
    public void RemoveWeaponIK()
    {
        LeftHandIKTarget = null;
        RightHandIKTarget = null;
        LeftElbowIKTarget = null;
        RightElbowIKTarget = null;
        _rigBuilder.enabled = false;

    }

}