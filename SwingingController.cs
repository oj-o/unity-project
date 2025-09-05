
using UnityEngine;

public class SwingingController : MonoBehaviour
{
    [Header("Swing Settings")]
    public float swingSpeed = 100f;
    public float swingAngle = 60f;

    private HingeJoint2D hinge;
    private JointMotor2D motor;

    void Start()
    {
        // 자식 오브젝트(Boom)에서 HingeJoint2D를 찾습니다.
        hinge = GetComponentInChildren<HingeJoint2D>();

        if (hinge == null)
        {
            Debug.LogError("HingeJoint2D component not found on children!");
            return;
        }

        // 관절의 모터를 설정합니다.
        motor = hinge.motor;
        motor.motorSpeed = swingSpeed;
        motor.maxMotorTorque = 10000; // 모터가 장애물에 걸려도 버틸 수 있는 힘
        hinge.motor = motor;
        hinge.useMotor = true;

        // 관절의 움직임 범위를 설정합니다.
        JointAngleLimits2D limits = hinge.limits;
        limits.min = -swingAngle;
        limits.max = swingAngle;
        hinge.limits = limits;
        hinge.useLimits = true;
    }

    void Update()
    {
        if (hinge == null) return;

        // 관절의 각도가 한계에 도달하면 모터의 방향을 반대로 바꿉니다.
        if (hinge.jointAngle >= hinge.limits.max - 1 || hinge.jointAngle <= hinge.limits.min + 1)
        {
            motor.motorSpeed = -motor.motorSpeed;
            hinge.motor = motor;
        }
    }
}
