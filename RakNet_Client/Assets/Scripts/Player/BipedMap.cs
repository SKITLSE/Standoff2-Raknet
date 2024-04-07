using RiseRakNet.Misc;
using UnityEngine;

namespace RiseRakNet.Player
{
    public class BipedMap : MonoBehaviour
    {
        public enum Bip
        {
            Head = 0,
            Neck = 1,
            Spine1 = 2,
            Spine2 = 3,
            LeftUpperarm = 4,
            LeftForearm = 5,
            LeftHand = 6,
            LeftShoulder = 7,
            RightShoulder = 8,
            RightUpperarm = 9,
            RightForearm = 10,
            RightHand = 11,
            Hip = 12,
            LeftThigh = 13,
            LeftCalf = 14,
            LeftFoot = 15,
            RightThigh = 16,
            RightCalf = 17,
            RightFoot = 18
        }


        public const string HeadNaming = "BipBase Head";
        public const string NeckNaming = "BipBase Neck1";
        public const string Spine1Naming = "BipBase Spine1";
        public const string Spine2Naming = "BipBase Spine2";
        public const string Left1ShoulderNaming = "BipBase L Clavicle";
        public const string LeftUpperarmNaming = "BipBase L UpperArm";
        public const string LeftForearmNaming = "BipBase L Forearm";
        public const string LeftHandNaming = "BipBase L Hand";
        public const string RightShoulderNaming = "BipBase R Clavicle";
        public const string RightUpperarmNaming = "BipBase R UpperArm";
        public const string RightForearmNaming = "BipBase R Forearm";
        public const string RightHandNaming = "BipBase R Hand";
        public const string HipNaming = "BipBase Pelvis";
        public const string LeftThighNaming = "BipBase L Thigh";
        public const string LeftCalfNaming = "BipBase L Calf";
        public const string LeftFootNaming = "BipBase L Foot";
        public const string RightThighNaming = "BipBase R Thigh";
        public const string RightCalfNaming = "BipBase R Calf";
        public const string RightFootNaming = "BipBase R Foot";
        public const string RFinger0Naming = "BipBase R Finger0";
        public const string RFinger1Naming = "BipBase R Finger1";
        public const string RFinger2Naming = "BipBase R Finger2";
        public const string RFinger3Naming = "BipBase R Finger3";
        public const string RFinger4Naming = "BipBase R Finger4";

        public Transform Head;

        public Transform Neck;

        public Transform Spine1;

        public Transform Spine2;

        public Transform LeftShoulder;

        public Transform LeftUpperarm;

        public Transform LeftForearm;

        public Transform LeftHand;

        public Transform RightShoulder;

        public Transform RightUpperarm;

        public Transform RightForearm;

        public Transform RightHand;

        public Transform Hip;

        public Transform LeftThigh;

        public Transform LeftCalf;

        public Transform LeftFoot;

        public Transform RightThigh;

        public Transform RightCalf;

        public Transform RightFoot;

        public Transform RFinger0;

        public Transform RFinger1;

        public Transform RFinger2;

        public Transform RFinger3;

        public Transform RFinger4;

        public Transform RFinger5 => RFinger0;

        public Transform RFinger6 => RFinger1;

        public Transform RFinger7 => RFinger2;

        public Transform RFinger8 => RFinger3;

        public Transform RFinger9 => RFinger4;

        [ContextMenu("Auto Detect")]
        private void AutoDetect()
        {
            Head = UnityUtility.FindDeepChild(transform, "BipBase Head");
            Neck = UnityUtility.FindDeepChild(transform, "BipBase Neck1");
            Spine1 = UnityUtility.FindDeepChild(transform, "BipBase Spine1");
            Spine2 = UnityUtility.FindDeepChild(transform, "BipBase Spine2");
            LeftShoulder = UnityUtility.FindDeepChild(transform, "BipBase L Clavicle");
            LeftUpperarm = UnityUtility.FindDeepChild(transform, "BipBase L UpperArm");
            LeftForearm = UnityUtility.FindDeepChild(transform, "BipBase L Forearm");
            LeftHand = UnityUtility.FindDeepChild(transform, "BipBase L Hand");
            RightShoulder = UnityUtility.FindDeepChild(transform, "BipBase R Clavicle");
            RightUpperarm = UnityUtility.FindDeepChild(transform, "BipBase R UpperArm");
            RightForearm = UnityUtility.FindDeepChild(transform, "BipBase R Forearm");
            RightHand = UnityUtility.FindDeepChild(transform, "BipBase R Hand");
            Hip = UnityUtility.FindDeepChild(transform, "BipBase Pelvis");
            LeftThigh = UnityUtility.FindDeepChild(transform, "BipBase L Thigh");
            LeftCalf = UnityUtility.FindDeepChild(transform, "BipBase L Calf");
            LeftFoot = UnityUtility.FindDeepChild(transform, "BipBase L Foot");
            RightThigh = UnityUtility.FindDeepChild(transform, "BipBase R Thigh");
            RightCalf = UnityUtility.FindDeepChild(transform, "BipBase R Calf");
            RightFoot = UnityUtility.FindDeepChild(transform, "BipBase R Foot");
            RFinger0 = UnityUtility.FindDeepChild(transform, "BipBase R Finger0");
            RFinger1 = UnityUtility.FindDeepChild(transform, "BipBase R Finger1");
            RFinger2 = UnityUtility.FindDeepChild(transform, "BipBase R Finger2");
            RFinger3 = UnityUtility.FindDeepChild(transform, "BipBase R Finger3");
            RFinger4 = UnityUtility.FindDeepChild(transform, "BipBase R Finger4");
        }
        private void CopyLocalRotation(BipedMap target, bool fixLeftForearm)
        {
            var boneCount = target.transform.childCount;
            if (boneCount < 1)
            {
                return;
            }

            for (var i = 0; i < boneCount; i++)
            {
                var sourceBone = GetBone(Bip.RightForearm).transform;
                if (sourceBone == null || !fixLeftForearm) continue;
                var sourceRotation = target.GetBone(Bip.LeftForearm).transform.localRotation;
                sourceBone.localRotation = new Quaternion { x = sourceRotation.x, y = sourceRotation.y, z = sourceRotation.z, w = sourceRotation.w };
            }
        }

        public void CopyBipedMap(BipedMap target, bool moveRoot, bool fixLeftForearm = false)
        {
            if (moveRoot)
            {
                CopyTransform(target.transform, transform);
                CopyTransform(target.Hip.transform, Hip.transform);
            }

            CopyLocalRotation(target, fixLeftForearm);
        }

        private static void CopyTransform(Transform source, Transform destination)
        {
            source.GetPositionAndRotation(out var sourcePosition, out var sourceRotation);

            destination.SetPositionAndRotation(position: new Vector3 { x = sourcePosition.x, y = sourcePosition.y, z = sourcePosition.z },
                rotation: new Quaternion { x = sourceRotation.x, y = sourceRotation.y, z = sourceRotation.z, w = sourceRotation.w });
        }

        public Transform GetBone(Bip bone)
        {
            switch (bone)
            {
                case Bip.Head:
                    return Head;
                case Bip.Neck:
                    return Neck;
                case Bip.LeftUpperarm:
                    return LeftUpperarm;
                case Bip.LeftForearm:
                    return LeftForearm;
                case Bip.LeftHand:
                    return LeftHand;
                case Bip.RightForearm:
                    return RightUpperarm;
                case Bip.RightUpperarm:
                    return RightForearm;
                case Bip.RightHand:
                    return RightHand;
                case Bip.Spine2:
                    return Spine2;
                case Bip.Spine1:
                    return Spine1;
                case Bip.LeftThigh:
                    return LeftThigh;
                case Bip.LeftCalf:
                    return LeftCalf;
                case Bip.RightThigh:
                    return RightThigh;
                case Bip.RightCalf:
                    return RightCalf;
                case Bip.LeftShoulder:
                    return LeftShoulder;
                case Bip.RightShoulder:
                    return RightShoulder;
                case Bip.Hip:
                    return Hip;
                case Bip.LeftFoot:
                    return LeftFoot;
                case Bip.RightFoot:
                    return RightFoot;
                default:
                    return null;
            }
        }

        public static bool IsHead(Bip bone)
        {
            return bone is Bip.Head or Bip.Neck;
        }

        public static bool IsChestAndArmsDamage(Bip bone)
        {
            return bone is Bip.Spine2 or Bip.LeftShoulder or Bip.LeftUpperarm or Bip.LeftForearm or Bip.LeftHand or Bip.RightShoulder or Bip.LeftUpperarm or Bip.RightForearm or Bip.RightHand or Bip.RightUpperarm;
        }

        public static bool IsStomach(Bip bone)
        {
            return bone is Bip.Hip or Bip.Spine1;
        }

        public static bool IsLegs(Bip bone)
        {
            return bone is Bip.LeftCalf or Bip.LeftThigh or Bip.LeftFoot or Bip.RightCalf or Bip.RightThigh or Bip.RightFoot;
        }

        public static bool IsTorso(Bip bone)
        {
            return bone is Bip.Spine2 or Bip.Spine1;
        }
    }
}