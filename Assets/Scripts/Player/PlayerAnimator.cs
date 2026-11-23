using UnityEngine;

namespace TrashRush.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Hierarchical Bone / Limb References")]
        [SerializeField] private Transform torsoBone;
        [SerializeField] private Transform headBone;
        [SerializeField] private Transform armLeftBone;
        [SerializeField] private Transform armRightBone;
        [SerializeField] private Transform legLeftBone;
        [SerializeField] private Transform legRightBone;

        [Header("Animation Tuning")]
        [SerializeField] private float walkAnimFrequency = 7.5f;
        [SerializeField] private float runAnimFrequency = 11.0f;
        [SerializeField] private float walkArmAngle = 32.0f;
        [SerializeField] private float runArmAngle = 55.0f;
        [SerializeField] private float walkLegAngle = 36.0f;
        [SerializeField] private float runLegAngle = 60.0f;

        private float animTime = 0f;
        private float currentMoveRatio = 0f;
        private float currentRunRatio = 0f;
        private Vector3 torsoDefaultLocalPos;

        private void Start()
        {
            if (torsoBone != null)
            {
                torsoDefaultLocalPos = torsoBone.localPosition;
            }
        }

        public void BindBones(Transform torso, Transform head, Transform armL, Transform armR, Transform legL, Transform legR)
        {
            torsoBone = torso;
            headBone = head;
            armLeftBone = armL;
            armRightBone = armR;
            legLeftBone = legL;
            legRightBone = legR;
            if (torso != null) torsoDefaultLocalPos = torso.localPosition;
        }

        public void UpdateAnimation(float speed, bool isSprinting, bool isGrounded)
        {
            // Smoothly interpolate movement state ratios
            float targetMove = (isGrounded && speed > 0.15f) ? 1f : 0f;
            float targetRun = (isGrounded && isSprinting && speed > 6.5f) ? 1f : 0f;

            currentMoveRatio = Mathf.MoveTowards(currentMoveRatio, targetMove, Time.deltaTime * 6f);
            currentRunRatio = Mathf.MoveTowards(currentRunRatio, targetRun, Time.deltaTime * 6f);

            float freq = Mathf.Lerp(walkAnimFrequency, runAnimFrequency, currentRunRatio);
            animTime += Time.deltaTime * freq * Mathf.Max(0.25f, currentMoveRatio);

            if (currentMoveRatio > 0.02f)
            {
                // Walking / Running locomotion cycle
                float sinCycle = Mathf.Sin(animTime);
                float cosCycle = Mathf.Cos(animTime);

                float armMax = Mathf.Lerp(walkArmAngle, runArmAngle, currentRunRatio) * currentMoveRatio;
                float legMax = Mathf.Lerp(walkLegAngle, runLegAngle, currentRunRatio) * currentMoveRatio;

                // Legs swing oppositely
                if (legLeftBone != null)
                    legLeftBone.localRotation = Quaternion.Euler(sinCycle * legMax, 0f, 0f);
                if (legRightBone != null)
                    legRightBone.localRotation = Quaternion.Euler(-sinCycle * legMax, 0f, 0f);

                // Arms swing opposite to their corresponding legs
                if (armLeftBone != null)
                    armLeftBone.localRotation = Quaternion.Euler(-sinCycle * armMax, 0f, 6f);
                if (armRightBone != null)
                    armRightBone.localRotation = Quaternion.Euler(sinCycle * armMax, 0f, -6f);

                // Torso bounce and subtle pelvic twist
                if (torsoBone != null)
                {
                    float bounce = Mathf.Abs(sinCycle) * (0.05f + currentRunRatio * 0.04f);
                    torsoBone.localPosition = torsoDefaultLocalPos + new Vector3(0f, bounce, 0f);
                    torsoBone.localRotation = Quaternion.Euler(currentRunRatio * 6f, cosCycle * 4f, 0f);
                }

                if (headBone != null)
                {
                    headBone.localRotation = Quaternion.Euler(-cosCycle * 2f, 0f, 0f);
                }
            }
            else
            {
                // Idle breathing cycle
                float idleSin = Mathf.Sin(Time.time * 2.2f);

                if (legLeftBone != null)
                    legLeftBone.localRotation = Quaternion.Slerp(legLeftBone.localRotation, Quaternion.identity, Time.deltaTime * 6f);
                if (legRightBone != null)
                    legRightBone.localRotation = Quaternion.Slerp(legRightBone.localRotation, Quaternion.identity, Time.deltaTime * 6f);

                if (armLeftBone != null)
                    armLeftBone.localRotation = Quaternion.Slerp(armLeftBone.localRotation, Quaternion.Euler(idleSin * 2f, 0f, 4f), Time.deltaTime * 5f);
                if (armRightBone != null)
                    armRightBone.localRotation = Quaternion.Slerp(armRightBone.localRotation, Quaternion.Euler(idleSin * 2f, 0f, -4f), Time.deltaTime * 5f);

                if (torsoBone != null)
                {
                    float breath = idleSin * 0.015f;
                    torsoBone.localPosition = Vector3.Lerp(torsoBone.localPosition, torsoDefaultLocalPos + new Vector3(0f, breath, 0f), Time.deltaTime * 5f);
                    torsoBone.localRotation = Quaternion.Slerp(torsoBone.localRotation, Quaternion.identity, Time.deltaTime * 5f);
                }

                if (headBone != null)
                {
                    headBone.localRotation = Quaternion.Slerp(headBone.localRotation, Quaternion.Euler(idleSin * 1.5f, 0f, 0f), Time.deltaTime * 5f);
                }
            }
        }
    }
}
