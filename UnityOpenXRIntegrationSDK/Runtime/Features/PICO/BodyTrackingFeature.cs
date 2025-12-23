using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.XR.PXR;
using UnityEngine;
#if AR_FOUNDATION_5||AR_FOUNDATION_6
using UnityEngine.XR.ARSubsystems;
#endif
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Unity.XR.OpenXR.Features.PICOSupport
{
    /// <summary>
    /// Body joint enumerations.
    /// * For leg tracking mode, joints numbered from 0 to 15 return data.
    /// * For full body tracking mode, all joints return data.
    /// </summary>
    public enum BodyTrackerRole
    {
        Pelvis = 0,
        LEFT_HIP = 1,
        RIGHT_HIP = 2,
        SPINE1 = 3,
        LEFT_KNEE = 4,
        RIGHT_KNEE = 5,
        SPINE2 = 6,
        LEFT_ANKLE = 7,
        RIGHT_ANKLE = 8,
        SPINE3 = 9,
        LEFT_FOOT = 10,
        RIGHT_FOOT = 11,
        NECK = 12,
        LEFT_COLLAR = 13,
        RIGHT_COLLAR = 14,
        HEAD = 15,
        LEFT_SHOULDER = 16,
        RIGHT_SHOULDER = 17,
        LEFT_ELBOW = 18,
        RIGHT_ELBOW = 19,
        LEFT_WRIST = 20,
        RIGHT_WRIST = 21,
        LEFT_HAND = 22,
        RIGHT_HAND = 23,
        NONE_ROLE = 24,                // unvalid
        MIN_ROLE = 0,                 // min value
        MAX_ROLE = 23,                // max value
        ROLE_NUM = 24,
    }
     /// <summary>
    /// The struct that defines the lengths (in centimeters) of different body parts of the avatar.
    /// </summary>
    public struct BodyTrackingBoneLength
    {
        /// <summary>
        /// The length of the head, which is from the top of the head to the upper area of the neck.
        /// </summary>
        public float headLen;
        /// <summary>
        /// The length of the neck, which is from the upper area of the neck to the lower area of the neck.
        /// </summary>
        public float neckLen;
        /// <summary>
        /// The length of the torso, which is from the lower area of the neck to the navel.
        /// </summary>
        public float torsoLen;
        /// <summary>
        /// The length of the hip, which is from the navel to the center of the upper area of the upper leg.
        /// </summary>
        public float hipLen;
        /// <summary>
        /// The length of the upper leg, which from the hip to the knee-joint.
        /// </summary>
        public float upperLegLen;
        /// <summary>
        /// The length of the lower leg, which is from the knee-joint to the ankle.
        /// </summary>
        public float lowerLegLen;
        /// <summary>
        /// The length of the foot, which is from the ankle to the tiptoe.
        /// </summary>
        public float footLen;
        /// <summary>
        /// The length of the shoulder, which is between the left and right shoulder joints.
        /// </summary>
        public float shoulderLen;
        /// <summary>
        /// The length of the upper arm, which is from the sholder joint to the elbow joint.
        /// </summary>
        public float upperArmLen;
        /// <summary>
        /// The length of the lower arm, which is from the elbow joint to the wrist.
        /// </summary>
        public float lowerArmLen;
        /// <summary>
        /// The length of the hand, which is from the wrist to the finger tip.
        /// </summary>
        public float handLen;
    }
    public enum BodyJointSet
    {
        BODY_JOINT_SET_BODY_START_WITHOUT_ARM = 1, //- For PICO Motion Tracker, nodes numbered 0 to 15 in `BodyTrackerRole` enum will return data.
        BODY_JOINT_SET_BODY_FULL_START = 2, /// - For PICO Motion Tracker, nodes numbered 0 to 23 in `BodyTrackerRole` enum will return data.
    }
    public struct BodyTrackingStartInfo
    {
        public BodyJointSet jointSet;
        public BodyTrackingBoneLength BoneLength;
    }
    /// <summary>Status code for body tracking data.</summary>
    public enum BodyTrackingStatusCode
    {
        /// <summary>There is no body tracking data.</summary>
        BT_INVALID = 0,
        /// <summary>There is body tracking data, and the data is accurate.</summary>
        BT_VALID = 1,
        /// <summary>There is body tracking data, but the data is not very accurate.</summary>
        BT_LIMITED = 2
    }
    /// <summary>Error codes for body tracking.<summary>
    public enum BodyTrackingMessage
    {
        BT_MESSAGE_UNKNOWN = 0,
        /// <summary>PICO Motion Tracker not calibrated.</summary>
        BT_MESSAGE_TRACKER_NOT_CALIBRATED = 1,
        /// <summary>The number of connected PICO Motion Trackers is not enough.</summary>
        BT_MESSAGE_TRACKER_NUM_NOT_ENOUGH = 2,
        /// <summary>PICO Motion Tracker's status is abnormal.</summary>
        BT_MESSAGE_TRACKER_STATE_NOT_SATISFIED = 3,
        /// <summary>PICO Motion Tracker is always invisible.</summary>
        BT_MESSAGE_TRACKER_PERSISTENT_INVISIBILITY = 4,
        /// <summary>PICO Motion Tracker's data is abnormal.</summary>
        BT_MESSAGE_TRACKER_DATA_ERROR = 5,
        /// <summary>The user may have changed.</summary>
        BT_MESSAGE_USER_CHANGE = 6,
        /// <summary>The body tracking pose is abnormal.</summary>
        BT_MESSAGE_TRACKING_POSE_ERROR = 7
    }
    /// <summary>Information about body tracking state.</summary>
    public unsafe struct BodyTrackingStatus
    {
        /// <summary>Status code for body tracking data.</summary>
        public BodyTrackingStatusCode stateCode;
        /// <summary>Body tracking error code.</summary>
        public BodyTrackingMessage message;
        public override string ToString()
        {
            string str = string.Format("stateCode:{0},errorCode:{1}\n",  stateCode, message);
            return str;
        }
    }

    public struct BodyTrackingGetDataInfo
    {
        public long displayTime;
    }
    public enum BodyActionList
    {
        PxrTouchGround = 0x00000001,
        PxrKeepStatic = 0x00000002,
        PxrTouchGroundToe = 0x00000004,
        PxrFootDownAction = 0x00000008,
    }
    /// <summary>
    /// Contains data about the position and rotation of a body joint.
    /// </summary>
    public struct BodyTrackerTransPose
    {
        /// <summary>
        /// IMU timestamp.
        /// </summary>
        public Int64 TimeStamp;
        /// <summary>
        /// The joint's position on the X axis.
        /// </summary>
        public double PosX;
        /// <summary>
        /// The joint's position on the Y axis.
        /// </summary>
        public double PosY;
        /// <summary>
        /// The joint's position on the Z axis.
        /// </summary>
        public double PosZ;
        /// <summary>
        /// The joint's rotation on the X component of the Quaternion.
        /// </summary>
        public double RotQx;
        /// <summary>
        /// The joint's rotation on the Y component of the Quaternion.
        /// </summary>
        public double RotQy;
        /// <summary>
        /// The joint's rotation on the Z component of the Quaternion.
        /// </summary>
        public double RotQz;
        /// <summary>
        /// The joint's rotation on the W component of the Quaternion.
        /// </summary>
        public double RotQw;
        public override string ToString()
        {
            return string.Format("TimeStamp :{0}, PosX:{1}, PosY:{2}, PosZ:{3}, RotQx:{4}, RotQy:{5}, RotQz:{6}, RotQw:{7}\n", TimeStamp, PosX, PosY, PosZ, RotQx, RotQy, RotQz, RotQw);
        }
    }

    /// <summary>Information about the tracked bone node.</summary>
    public unsafe struct BodyTrackingRoleData
    {
        private int apiVersion;
        /// <summary>Bone name. if bone = `NONE_ROLE`, this bone is not calculated.</summary>
        public BodyTrackerRole role;
        /// <summary>Multiple actions can be supported at the same time by means of `OR BodyActionList`.</summary>
        public BodyActionList bodyAction;
        /// <summary>The bone's local transform.</summary>
        public BodyTrackerTransPose localPose;
        /// <summary>The bone's global transform.</summary>
        public BodyTrackerTransPose globalPose;
        /// <summary>The velocity of X, Y, and Z.</summary>
        public fixed double velo[3];
        /// <summary>The acceleration of X, Y, and Z.</summary>
        public fixed double acce[3];
        /// <summary>The angular velocity of X, Y, and Z.</summary>
        public fixed double wvelo[3];
        /// <summary>The angular acceleration of X, Y, and Z.</summary>
        public fixed double wacce[3];
        public override string ToString()
        {
            string str = string.Format("apiVersion :{0}, role:{1}, bodyAction:{2}, localPose:{3}, globalPose:{4}\n", apiVersion, role, bodyAction, localPose, globalPose);
            for (int i = 0; i < 3; i++)
            {
                str += string.Format(" velo[{0}]:{1}", i, velo[i].ToString("F6"));
                str += string.Format(" acce[{0}]:{1}", i, acce[i].ToString("F6"));
                str += string.Format(" wvelo[{0}]:{1}", i, wvelo[i].ToString("F6"));
                str += string.Format(" wacce[{0}]:{1}", i, wacce[i].ToString("F6"));
                str += "\n";
            }
            return str;
        }
    }
    /// <summary>Body tracking data.</summary>
    public struct BodyTrackingData
    {
        private int apiVersion;
        /// <summary>Information about the tracked bone node.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)BodyTrackerRole.NONE_ROLE)]
        public BodyTrackingRoleData[] roleDatas;
        public override string ToString()
        {
            string str = string.Format("apiVersion :{0}\n", apiVersion);
            for (int i = 0; i < (int)BodyTrackerRole.NONE_ROLE; i++)
            {
                str += string.Format(" roleData[{0}]:{1}", i, roleDatas[i].ToString());
            }

            return str;
        }
    }
    
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "PICO Body Tracking",
        Hidden = false,
        BuildTargetGroups = new[] { UnityEditor.BuildTargetGroup.Android },
        Company = "PICO",
        OpenxrExtensionStrings = extensionString,
        Version = "1.0.0",
        FeatureId = featureId)]
#endif
    
    public class BodyTrackingFeature : OpenXRFeatureBase
    {
        public const string featureId = "com.pico.openxr.feature.PICO_BodyTracking";
        public const string extensionString = "XR_BD_body_tracking XR_PICO_body_tracking2";
        public const int XR_BODY_JOINT_COUNT_BD = 24;
    
        public static bool isEnable => OpenXRRuntime.IsExtensionEnabled("XR_BD_body_tracking");

        public override string GetExtensionString()
        {
            return extensionString;
        }
        public override void Initialize(IntPtr intPtr)
        {
            Pxr_BodyTrackingEnable(isEnable);
        }
        [Obsolete("Please use StartBodyTracking(BodyJointSet JointSet, BodyTrackingBoneLength boneLength)")]
        public static bool StartBodyTracking(XrBodyJointSetBD Mode)
        {
            if (!isEnable)
            {
                return false;
            }
            
            BodyTrackingBoneLength boneLength=new BodyTrackingBoneLength();

            return StartBodyTracking((BodyJointSet)Mode, boneLength)==0;
        }
        /// <summary>Starts body tracking.</summary>
        /// <param name="mode">Specifies the body tracking mode (default or high-accuracy).</param>
        /// <param name="boneLength">Specifies lengths (unit: cm) for the bones of the avatar, which is only available for the `BTM_FULL_BODY_HIGH` mode.
        /// Bones that are not set lengths for will use the default values.
        /// </param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StartBodyTracking(BodyJointSet JointSet, BodyTrackingBoneLength boneLength)
        {
            if (!isEnable)
            {
                return 1;
            }
            BodyTrackingStartInfo startInfo = new BodyTrackingStartInfo();
            startInfo.jointSet = JointSet;
            startInfo.BoneLength = boneLength;

            return Pxr_StartBodyTracking(ref startInfo);
        }
        /// <summary>Launches the PICO Motion Tracker app to perform calibration.
        /// - For PICO Motion Tracker (Beta), the user needs to follow the instructions on the home of the PICO Motion Tracker app to complete calibration.
        /// - For PICO Motion Tracker (Official), "single-glance calibration" will be performed. When a user has a glance at the PICO Motion Tracker on their lower legs, calibration is completed.
        /// </summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StartMotionTrackerCalibApp()
        {
            if (!isEnable)
            {
                return 1;
            }
            return Pxr_StartBodyTrackingCalibApp();
        }

        public static bool IsBodyTrackingSupported()
        {
            if (!isEnable)
            {
                return false;
            }
            bool supported=false;
            Pxr_GetBodyTrackingSupported(ref supported);
            return supported;
        }

        /// <summary>
        /// Gets the data about the poses of body joints.
        /// </summary>
        /// <param name="predictTime">Reserved parameter, pass `0`.</param>
        /// <param name="bodyTrackerResult">Contains the data about the poses of body joints, including position, action, and more.</param>
        [Obsolete("Please use GetBodyTrackingData")]
        public static bool GetBodyTrackingPose(ref BodyTrackerResult bodyTrackerResult)
        {
            if (!isEnable)
            {
                return false;
            }

            BodyTrackingGetDataInfo getInfo = new BodyTrackingGetDataInfo();
            getInfo.displayTime = 0;
            BodyTrackingData data = new BodyTrackingData();
            bool state =  GetBodyTrackingData(ref getInfo, ref data)==0;
            
            for (int i = 0; i < XR_BODY_JOINT_COUNT_BD; i++)
            {
                bodyTrackerResult.trackingdata[i].pose.PosX =data.roleDatas[i].localPose.PosX ;
                bodyTrackerResult.trackingdata[i].pose.PosY = -data.roleDatas[i].localPose.PosZ;
                bodyTrackerResult.trackingdata[i].pose.PosZ = -data.roleDatas[i].localPose.PosZ;
                bodyTrackerResult.trackingdata[i].pose.RotQx = -data.roleDatas[i].localPose.RotQz;
                bodyTrackerResult.trackingdata[i].pose.RotQy = -data.roleDatas[i].localPose.RotQw;
                bodyTrackerResult.trackingdata[i].pose.RotQz = -data.roleDatas[i].localPose.RotQz;
                bodyTrackerResult.trackingdata[i].pose.RotQw = -data.roleDatas[i].localPose.RotQw;
                
            }

            return state;
        }
        /// <summary>Stops body tracking.</summary>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int StopBodyTracking()
        {
            return Pxr_StopBodyTracking();
        }
        [Obsolete("Please use StopBodyTracking")]
        private void OnDestroy()
        {
            if (!isExtensionEnabled())
            {
                return;
            }

            StopBodyTracking();
        }
        
        [Obsolete("Please use StartMotionTrackerCalibApp")]
        public static void OpenFitnessBandCalibrationAPP()
        {
            StartMotionTrackerCalibApp();
        }

        /// <summary>Gets body tracking data.</summary>
        /// <param name="getInfo"> Specifies the display time and the data filtering flags.
        /// For the display time, for example, when it is set to 0.1 second, it means predicting the pose of the tracked node 0.1 seconds ahead.
        /// </param>
        /// <param name="data">Returns the array of data for all tracked nodes.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public unsafe static int GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data)
        {
            if (!isEnable)
            {
                return 1;
            }
            int val = -1;
            {
                val = Pxr_GetBodyTrackingData(ref getInfo, ref data);
                for (int i = 0; i < (int)BodyTrackerRole.ROLE_NUM; i++)
                {
                    data.roleDatas[i].localPose.PosZ = -data.roleDatas[i].localPose.PosZ;
                    data.roleDatas[i].localPose.RotQz = -data.roleDatas[i].localPose.RotQz;
                    data.roleDatas[i].localPose.RotQw = -data.roleDatas[i].localPose.RotQw;
                    data.roleDatas[i].velo[3] = -data.roleDatas[i].velo[3];
                    data.roleDatas[i].acce[3] = -data.roleDatas[i].acce[3];
                    data.roleDatas[i].wvelo[3] = -data.roleDatas[i].wvelo[3];
                    data.roleDatas[i].wacce[3] = -data.roleDatas[i].wacce[3];
                }
            }
            return val;
        }
        /// <summary>Gets the state of PICO Motion Tracker and, if any, the reason for an exception.</summary>
        /// <param name="isTracking">Indicates whether the PICO Motion Tracker is tracking normally:
        /// - `true`: is tracking
        /// - `false`: tracking lost
        /// </param>
        /// <param name="state">Returns the information about body tracking state.</param>
        /// <returns>
        /// - `0`: success
        /// - `1`: failure
        /// </returns>
        public static int GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state)
        {
            int val = -1;
            {
                val = Pxr_GetBodyTrackingState(ref isTracking, ref state);
            }
            return val;
        }

#if AR_FOUNDATION_5||AR_FOUNDATION_6
        public  bool isBodyTracking=false;
        static List<XRHumanBodySubsystemDescriptor> s_HumanBodyDescriptors = new List<XRHumanBodySubsystemDescriptor>();
        protected override void OnSubsystemCreate()
        {
            base.OnSubsystemCreate();
            if (isBodyTracking)
            {
                CreateSubsystem<XRHumanBodySubsystemDescriptor, XRHumanBodySubsystem>(
                    s_HumanBodyDescriptors,
                    PXR_HumanBodySubsystem.k_SubsystemId);
           
            }

        }
        protected override void OnSubsystemStart()
        {
            if (isBodyTracking)
            {
                StartSubsystem<XRHumanBodySubsystem>();
            }
        }
        protected override void OnSubsystemStop()
        {
            if (isBodyTracking)
            {
                StopSubsystem<XRHumanBodySubsystem>();
            }
        }
        protected override void OnSubsystemDestroy()
        {
            if (isBodyTracking)
            {
                DestroySubsystem<XRHumanBodySubsystem>();
            }
        }
#endif
        
        
        private const string ExtLib = "openxr_pico";
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Pxr_BodyTrackingEnable(bool enable);
        
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartBodyTrackingCalibApp();
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetBodyTrackingSupported(ref bool supported);
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StartBodyTracking(ref BodyTrackingStartInfo startInfo);
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_StopBodyTracking();
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetBodyTrackingState(ref bool isTracking, ref BodyTrackingStatus state);
        [DllImport(ExtLib, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Pxr_GetBodyTrackingData(ref BodyTrackingGetDataInfo getInfo, ref BodyTrackingData data);
        
        
    }
}