#region

using UnityEditor;
using UnityEngine;

#endregion

namespace Gamekit2D
{
    [CustomEditor(typeof(PlayerCharacter))]
    public class PlayerCharacterEditor : Editor
    {
        private readonly GUIContent m_AirborneAccelProportionContent = new GUIContent("Airborne Accel Proportion");
        private readonly GUIContent m_AirborneDecelProportionContent = new GUIContent("Airborne Decel Proportion");
        private readonly GUIContent m_AirborneSettingsContent = new GUIContent("Airborne Settings");
        private readonly GUIContent m_AudioSettingsContent = new GUIContent("Audio Settings");
        private readonly GUIContent m_BulletPoolContent = new GUIContent("Bullet Pool");
        private readonly GUIContent m_BulletSpeedContent = new GUIContent("Bullet Speed");
        private readonly GUIContent m_CameraFollowSettingsContent = new GUIContent("Camera Follow Settings");
        private readonly GUIContent m_CameraFollowTargetContent = new GUIContent("Camera Follow Target");

        private readonly GUIContent m_CameraHorizontalFacingOffsetContent =
            new GUIContent("Camera Horizontal Facing Offset");

        private readonly GUIContent m_CameraHorizontalSpeedOffsetContent =
            new GUIContent("Camera Horizontal Speed Offset");

        private readonly GUIContent m_CameraVerticalInputOffsetContent = new GUIContent("Camera Vertical Input Offset");
        private readonly GUIContent m_DamageableContent = new GUIContent("Damageable");
        private readonly GUIContent m_DashWhileAirborneContent = new GUIContent("Dash While Airborne");

        private readonly GUIContent m_FacingLeftBulletSpawnPointContent =
            new GUIContent("Facing Left Bullet Spawn Point");

        private readonly GUIContent m_FacingRightBulletSpawnPointContent =
            new GUIContent("Facing Right Bullet Spawn Point");

        private readonly GUIContent m_FlickeringDurationContent = new GUIContent("Flicking Duration",
            "When the player is hurt she becomes invulnerable for a short time and the SpriteRenderer flickers on and off to indicate this.  This field is the duration in seconds the SpriteRenderer stays either on or off whilst flickering.  To adjust the duration of invulnerability see the Damageable component.");

        private readonly GUIContent m_FootstepPlayerContent = new GUIContent("Footstep Audio Player");
        private readonly GUIContent m_GravityContent = new GUIContent("Gravity");
        private readonly GUIContent m_GroundAccelerationContent = new GUIContent("Ground Acceleration");
        private readonly GUIContent m_GroundDecelerationContent = new GUIContent("Ground Deceleration");
        private readonly GUIContent m_HoldingGunTimeoutDurationContent = new GUIContent("Holding Gun Timeout Duration");
        private readonly GUIContent m_HurtAudioPlayerContent = new GUIContent("Hurt Audio Player");

        private readonly GUIContent m_HurtJumpAngleContent = new GUIContent("Hurt Jump Angle");
        private readonly GUIContent m_HurtJumpSpeedContent = new GUIContent("Hurt Jump Speed");
        private readonly GUIContent m_HurtSettingsContent = new GUIContent("Hurt Settings");
        private readonly GUIContent m_JumpAbortSpeedReductionContent = new GUIContent("Jump Abort Speed Reduction");
        private readonly GUIContent m_JumpSpeedContent = new GUIContent("Jump Speed");
        private readonly GUIContent m_LandingAudioPlayerContent = new GUIContent("Landing Audio Player");

        private readonly GUIContent m_MaxHorizontalDeltaDampTimeContent =
            new GUIContent("Max Horizontal Delta Damp Time");

        private readonly GUIContent m_MaxSpeedContent = new GUIContent("Max Speed");
        private readonly GUIContent m_MaxVerticalDeltaDampTimeContent = new GUIContent("Max Vertical Delta Damp Time");
        private readonly GUIContent m_MeleeAttackAudioPlayerContent = new GUIContent("Melee Attack Audio Player");

        private readonly GUIContent m_MeleeAttackDashSpeedContent = new GUIContent("Melee Attack Dash Speed");
        private readonly GUIContent m_MeleeDamagerContent = new GUIContent("Melee Damager");
        private readonly GUIContent m_MeleeSettingsContent = new GUIContent("Melee Settings");
        private readonly GUIContent m_MiscSettingsContent = new GUIContent("Misc Settings");
        private readonly GUIContent m_MovementSettingsContent = new GUIContent("Movement Settings");
        private readonly GUIContent m_PushingSpeedProportionContent = new GUIContent("Pushing Speed Proportion");
        private readonly GUIContent m_RangedAttackAudioPlayerContent = new GUIContent("Ranged Attack Audio Player");
        private readonly GUIContent m_RangedSettingsContent = new GUIContent("Ranged Settings");

        private readonly GUIContent m_ReferencesContent = new GUIContent("References");

        private readonly GUIContent m_RightBulletSpawnPointAnimatedContent =
            new GUIContent("Right Bullet Spawn Point Animated");

        private readonly GUIContent m_ShotsPerSecondContent = new GUIContent("Shots Per Second");

        private readonly GUIContent m_SpriteOriginallyFacesLeftContent = new GUIContent("Sprite Originally Faces Left");

        private readonly GUIContent m_SpriteRendererContent = new GUIContent("Sprite Renderer");
        private readonly GUIContent m_VerticalCameraOffsetDelayContent = new GUIContent("Vertical Camera Offset Delay");

        private SerializedProperty m_AirborneAccelProportionProp;
        private SerializedProperty m_AirborneDecelProportionProp;
        private bool m_AirborneSettingsFoldout;
        private bool m_AudioSettingsFoldout;
        private SerializedProperty m_BulletPoolProp;
        private SerializedProperty m_BulletSpeedProp;
        private bool m_CameraFollowSettingsFoldout;
        private SerializedProperty m_CameraFollowTargetProp;

        private SerializedProperty m_CameraHorizontalFacingOffsetProp;
        private SerializedProperty m_CameraHorizontalSpeedOffsetProp;
        private SerializedProperty m_CameraVerticalInputOffsetProp;
        private SerializedProperty m_DamageableProp;
        private SerializedProperty m_DashWhileAirborneProp;
        private SerializedProperty m_FacingLeftBulletSpawnPointProp;
        private SerializedProperty m_FacingRightBulletSpawnPointProp;
        private SerializedProperty m_FlickeringDurationProp;

        private SerializedProperty m_FootstepAudioPlayerProp;
        private SerializedProperty m_GravityProp;
        private SerializedProperty m_GroundAccelerationProp;
        private SerializedProperty m_GroundDecelerationProp;
        private SerializedProperty m_HoldingGunTimeoutDurationProp;
        private SerializedProperty m_HurtAudioPlayerProp;

        private SerializedProperty m_HurtJumpAngleProp;
        private SerializedProperty m_HurtJumpSpeedProp;
        private bool m_HurtSettingsFoldout;
        private SerializedProperty m_JumpAbortSpeedReductionProp;
        private SerializedProperty m_JumpSpeedProp;
        private SerializedProperty m_LandingAudioPlayerProp;
        private SerializedProperty m_MaxHorizontalDeltaDampTimeProp;

        private SerializedProperty m_MaxSpeedProp;
        private SerializedProperty m_MaxVerticalDeltaDampTimeProp;
        private SerializedProperty m_MeleeAttackAudioPlayerProp;

        private SerializedProperty m_MeleeAttackDashSpeedProp;
        private SerializedProperty m_MeleeDamagerProp;
        private bool m_MeleeSettingsFoldout;
        private bool m_MiscSettingsFoldout;
        private bool m_MovementSettingsFoldout;
        private SerializedProperty m_PushingSpeedProportionProp;
        private SerializedProperty m_RangedAttackAudioPlayerProp;
        private bool m_RangedSettingsFoldout;

        private bool m_ReferencesFoldout;
        private SerializedProperty m_RightBulletSpawnPointAnimatedProp;

        private SerializedProperty m_ShotsPerSecondProp;

        private SerializedProperty m_SpriteOriginallyFacesLeftProp;
        private SerializedProperty m_SpriteRendererProp;
        private SerializedProperty m_VerticalCameraOffsetDelayProp;

        private void OnEnable()
        {
            m_SpriteRendererProp = serializedObject.FindProperty("spriteRenderer");
            m_DamageableProp = serializedObject.FindProperty("damageable");
            m_MeleeDamagerProp = serializedObject.FindProperty("meleeDamager");
            m_FacingLeftBulletSpawnPointProp = serializedObject.FindProperty("facingLeftBulletSpawnPoint");
            m_FacingRightBulletSpawnPointProp = serializedObject.FindProperty("facingRightBulletSpawnPoint");
            m_BulletPoolProp = serializedObject.FindProperty("bulletPool");
            m_CameraFollowTargetProp = serializedObject.FindProperty("cameraFollowTarget");

            m_MaxSpeedProp = serializedObject.FindProperty("maxSpeed");
            m_GroundAccelerationProp = serializedObject.FindProperty("groundAcceleration");
            m_GroundDecelerationProp = serializedObject.FindProperty("groundDeceleration");
            m_PushingSpeedProportionProp = serializedObject.FindProperty("pushingSpeedProportion");

            m_AirborneAccelProportionProp = serializedObject.FindProperty("airborneAccelProportion");
            m_AirborneDecelProportionProp = serializedObject.FindProperty("airborneDecelProportion");
            m_GravityProp = serializedObject.FindProperty("gravity");
            m_JumpSpeedProp = serializedObject.FindProperty("jumpSpeed");
            m_JumpAbortSpeedReductionProp = serializedObject.FindProperty("jumpAbortSpeedReduction");

            m_HurtJumpAngleProp = serializedObject.FindProperty("hurtJumpAngle");
            m_HurtJumpSpeedProp = serializedObject.FindProperty("hurtJumpSpeed");
            m_FlickeringDurationProp = serializedObject.FindProperty("flickeringDuration");

            m_MeleeAttackDashSpeedProp = serializedObject.FindProperty("meleeAttackDashSpeed");
            m_DashWhileAirborneProp = serializedObject.FindProperty("dashWhileAirborne");

            m_ShotsPerSecondProp = serializedObject.FindProperty("shotsPerSecond");
            m_BulletSpeedProp = serializedObject.FindProperty("bulletSpeed");
            m_HoldingGunTimeoutDurationProp = serializedObject.FindProperty("holdingGunTimeoutDuration");
            m_RightBulletSpawnPointAnimatedProp = serializedObject.FindProperty("rightBulletSpawnPointAnimated");

            m_FootstepAudioPlayerProp = serializedObject.FindProperty("footstepAudioPlayer");
            m_LandingAudioPlayerProp = serializedObject.FindProperty("landingAudioPlayer");
            m_HurtAudioPlayerProp = serializedObject.FindProperty("hurtAudioPlayer");
            m_MeleeAttackAudioPlayerProp = serializedObject.FindProperty("meleeAttackAudioPlayer");
            m_RangedAttackAudioPlayerProp = serializedObject.FindProperty("rangedAttackAudioPlayer");

            m_CameraHorizontalFacingOffsetProp = serializedObject.FindProperty("cameraHorizontalFacingOffset");
            m_CameraHorizontalSpeedOffsetProp = serializedObject.FindProperty("cameraHorizontalSpeedOffset");
            m_CameraVerticalInputOffsetProp = serializedObject.FindProperty("cameraVerticalInputOffset");
            m_MaxHorizontalDeltaDampTimeProp = serializedObject.FindProperty("maxHorizontalDeltaDampTime");
            m_MaxVerticalDeltaDampTimeProp = serializedObject.FindProperty("maxVerticalDeltaDampTime");
            m_VerticalCameraOffsetDelayProp = serializedObject.FindProperty("verticalCameraOffsetDelay");

            m_SpriteOriginallyFacesLeftProp = serializedObject.FindProperty("spriteOriginallyFacesLeft");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_ReferencesFoldout = EditorGUILayout.Foldout(m_ReferencesFoldout, m_ReferencesContent);

            if (m_ReferencesFoldout)
            {
                EditorGUILayout.PropertyField(m_SpriteRendererProp, m_SpriteRendererContent);
                EditorGUILayout.PropertyField(m_DamageableProp, m_DamageableContent);
                EditorGUILayout.PropertyField(m_MeleeDamagerProp, m_MeleeDamagerContent);
                EditorGUILayout.PropertyField(m_FacingLeftBulletSpawnPointProp, m_FacingLeftBulletSpawnPointContent);
                EditorGUILayout.PropertyField(m_FacingRightBulletSpawnPointProp, m_FacingRightBulletSpawnPointContent);
                EditorGUILayout.PropertyField(m_BulletPoolProp, m_BulletPoolContent);
                EditorGUILayout.PropertyField(m_CameraFollowTargetProp, m_CameraFollowTargetContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_MovementSettingsFoldout = EditorGUILayout.Foldout(m_MovementSettingsFoldout, m_MovementSettingsContent);

            if (m_MovementSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_MaxSpeedProp, m_MaxSpeedContent);
                EditorGUILayout.PropertyField(m_GroundAccelerationProp, m_GroundAccelerationContent);
                EditorGUILayout.PropertyField(m_GroundDecelerationProp, m_GroundDecelerationContent);
                EditorGUILayout.PropertyField(m_PushingSpeedProportionProp, m_PushingSpeedProportionContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_AirborneSettingsFoldout = EditorGUILayout.Foldout(m_AirborneSettingsFoldout, m_AirborneSettingsContent);

            if (m_AirborneSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_AirborneAccelProportionProp, m_AirborneAccelProportionContent);
                EditorGUILayout.PropertyField(m_AirborneDecelProportionProp, m_AirborneDecelProportionContent);
                EditorGUILayout.PropertyField(m_GravityProp, m_GravityContent);
                EditorGUILayout.PropertyField(m_JumpSpeedProp, m_JumpSpeedContent);
                EditorGUILayout.PropertyField(m_JumpAbortSpeedReductionProp, m_JumpAbortSpeedReductionContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_HurtSettingsFoldout = EditorGUILayout.Foldout(m_HurtSettingsFoldout, m_HurtSettingsContent);

            if (m_HurtSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_HurtJumpAngleProp, m_HurtJumpAngleContent);
                EditorGUILayout.PropertyField(m_HurtJumpSpeedProp, m_HurtJumpSpeedContent);
                EditorGUILayout.PropertyField(m_FlickeringDurationProp, m_FlickeringDurationContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_MeleeSettingsFoldout = EditorGUILayout.Foldout(m_MeleeSettingsFoldout, m_MeleeSettingsContent);

            if (m_MeleeSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_MeleeAttackDashSpeedProp, m_MeleeAttackDashSpeedContent);
                EditorGUILayout.PropertyField(m_DashWhileAirborneProp, m_DashWhileAirborneContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_RangedSettingsFoldout = EditorGUILayout.Foldout(m_RangedSettingsFoldout, m_RangedSettingsContent);

            if (m_RangedSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_ShotsPerSecondProp, m_ShotsPerSecondContent);
                EditorGUILayout.PropertyField(m_BulletSpeedProp, m_BulletSpeedContent);
                EditorGUILayout.PropertyField(m_HoldingGunTimeoutDurationProp, m_HoldingGunTimeoutDurationContent);
                EditorGUILayout.PropertyField(m_RightBulletSpawnPointAnimatedProp,
                    m_RightBulletSpawnPointAnimatedContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_AudioSettingsFoldout = EditorGUILayout.Foldout(m_AudioSettingsFoldout, m_AudioSettingsContent);

            if (m_AudioSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_FootstepAudioPlayerProp, m_FootstepPlayerContent);
                EditorGUILayout.PropertyField(m_LandingAudioPlayerProp, m_LandingAudioPlayerContent);
                EditorGUILayout.PropertyField(m_HurtAudioPlayerProp, m_HurtAudioPlayerContent);
                EditorGUILayout.PropertyField(m_MeleeAttackAudioPlayerProp, m_MeleeAttackAudioPlayerContent);
                EditorGUILayout.PropertyField(m_RangedAttackAudioPlayerProp, m_RangedAttackAudioPlayerContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_CameraFollowSettingsFoldout =
                EditorGUILayout.Foldout(m_CameraFollowSettingsFoldout, m_CameraFollowSettingsContent);

            if (m_CameraFollowSettingsFoldout)
            {
                EditorGUILayout.PropertyField(m_CameraHorizontalFacingOffsetProp,
                    m_CameraHorizontalFacingOffsetContent);
                EditorGUILayout.PropertyField(m_CameraHorizontalSpeedOffsetProp, m_CameraHorizontalSpeedOffsetContent);
                EditorGUILayout.PropertyField(m_CameraVerticalInputOffsetProp, m_CameraVerticalInputOffsetContent);
                EditorGUILayout.PropertyField(m_MaxHorizontalDeltaDampTimeProp, m_MaxHorizontalDeltaDampTimeContent);
                EditorGUILayout.PropertyField(m_MaxVerticalDeltaDampTimeProp, m_MaxVerticalDeltaDampTimeContent);
                EditorGUILayout.PropertyField(m_VerticalCameraOffsetDelayProp, m_VerticalCameraOffsetDelayContent);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;

            m_MiscSettingsFoldout = EditorGUILayout.Foldout(m_MiscSettingsFoldout, m_MiscSettingsContent);

            if (m_MiscSettingsFoldout)
                EditorGUILayout.PropertyField(m_SpriteOriginallyFacesLeftProp, m_SpriteOriginallyFacesLeftContent);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
    }
}