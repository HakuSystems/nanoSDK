using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.SDK3.Avatars.Components;
using VRC.Dynamics;
using VRC.SDKBase.Validation;
using VRC.Utility;

public static class PhysBoneMigration
{
    public static CubicBezierLookup ElastToPull;
    public static CubicBezierLookup ElastToSpring;
    public static CubicBezierLookup StiffToMaxAngle;
    public static bool HasInitDBConversionTables = false;

    public static System.Type TypeDynamicBone;
    public static System.Type TypeDynamicBoneCollider;

    enum DB_Bound
    {
        Outside = 0,
        Inside = 1
    }
    enum DB_Direction
    {
        X = 0,
        Y = 1,
        Z = 2,
    }
    enum DB_FreezeAxis
    {
        None, X, Y, Z
    }

    public static void Migrate(VRCAvatarDescriptor descriptor)
    {
        if (!EditorUtility.DisplayDialog("Warning", "This operation will remove all DynamicBone components and replace them with PhysBone components on your avatar.  This process attempts to match settings but the result may not appear be the same.  This is not reversible  so please make a backup before continuing.", "Proceed", "Cancel"))
            return;

        try
        {
            //Find types
            TypeDynamicBone = ValidationUtils.GetTypeFromName("DynamicBone");
            TypeDynamicBoneCollider = ValidationUtils.GetTypeFromName("DynamicBoneCollider");
            if (TypeDynamicBone == null || TypeDynamicBoneCollider == null)
            {
                EditorUtility.DisplayDialog("Error", "DynamicBone not found in the project.", "Okay");
                return;
            }

            //Animator
            var animator = descriptor.gameObject.GetComponent<Animator>();

            //Setup
            InitTables();

            //Colliders
            var dbColliders = descriptor.gameObject.GetComponentsInChildren(TypeDynamicBoneCollider, true);
            var pbColliders = new VRCPhysBoneCollider[dbColliders.Length];
            for (int i = 0; i < dbColliders.Length; i++)
            {
                pbColliders[i] = MigrateDynamicBoneCollider(dbColliders[i], animator);
            }

            //Bones
            var dbBones = descriptor.gameObject.GetComponentsInChildren(TypeDynamicBone, true);
            var pbBones = new VRCPhysBone[dbBones.Length];
            for (int i = 0; i < dbBones.Length; i++)
            {
                pbBones[i] = MigrateDynamicBone(dbBones[i], dbColliders, pbColliders);
            }

            //Cleanup
            foreach (var dbc in dbColliders)
            {
                Component.DestroyImmediate(dbc);
            }
            foreach (var db in dbBones)
            {
                Component.DestroyImmediate(db);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            EditorUtility.DisplayDialog("Error", "Encountered critical error while attempting to this operation.", "Okay");
        }
    }
    static void InitTables()
    {
        if (HasInitDBConversionTables)
            return;

        //Elast to Pull
        var pullTable = new Vector2[]
        {
            new Vector2(0.0f, 0.0f),
            //Small
            new Vector2(0.01f, 0.05f),
            new Vector2(0.025f, 0.15f),
            new Vector2(0.05f, 0.2f),
            new Vector2(0.075f, 0.225f),
            //Regular
            new Vector2(0.1f, 0.25f),
            new Vector2(0.2f, 0.45f),
            new Vector2(0.3f, 0.6f),
            new Vector2(0.4f, 0.7f),
            new Vector2(0.5f, 0.75f),
            new Vector2(0.6f, 0.8f),
            new Vector2(0.7f, 0.85f),
            new Vector2(0.8f, 0.9f),
            new Vector2(0.9f, 0.25f),
            new Vector2(1.0f, 0.95f),
        };
        ElastToPull = new CubicBezierLookup(pullTable);

        //Elast to Spring
        var curve = new CubicBezier(
            new Vector2(0.0f, 1.0f),
            new Vector2(0.271f, 0.79f),
            new Vector2(0.649f, 0.092f),
            new Vector2(1.0f, 0.0f));
        ElastToSpring = new CubicBezierLookup(curve);

        //Stiff to MaxAngle
        var stiffTable = new Vector2[]
            {
                    new Vector2(0.0f, 180f),
                    new Vector2(0.1f, 129f),
                    new Vector2(0.2f, 106f),
                    new Vector2(0.3f, 89f),
                    new Vector2(0.4f, 74f),
                    new Vector2(0.5f, 60f),
                    new Vector2(0.6f, 47f),
                    new Vector2(0.7f, 35f),
                    new Vector2(0.8f, 23f),
                    new Vector2(0.9f, 11f),
                    new Vector2(1.0f, 0f),
            };
        StiffToMaxAngle = new CubicBezierLookup(stiffTable);

        HasInitDBConversionTables = true;
    }
    static VRCPhysBoneCollider MigrateDynamicBoneCollider(Component obj, Animator animator)
    {
        var type = TypeDynamicBoneCollider;

        var bound = Convert.ToInt32(type.GetField("m_Bound").GetValue(obj));
        if (bound == (int)DB_Bound.Inside || IsPartOfHand(animator, obj.transform))
            return null;

        var pbc = obj.gameObject.AddComponent<VRCPhysBoneCollider>();
        pbc.radius = Convert.ToSingle(type.GetField("m_Radius").GetValue(obj));
        pbc.height = Convert.ToSingle(type.GetField("m_Height").GetValue(obj));
        pbc.position = (Vector3)type.GetField("m_Center").GetValue(obj);
        pbc.shapeType = pbc.height > pbc.radius * 2f ? VRCPhysBoneCollider.ShapeType.Capsule : VRCPhysBoneCollider.ShapeType.Sphere;

        switch (Convert.ToInt32(type.GetField("m_Direction").GetValue(obj)))
        {
            default:
            case (int)DB_Direction.X: pbc.rotation = Quaternion.AngleAxis(90, Vector3.forward); break;
            case (int)DB_Direction.Y: pbc.rotation = Quaternion.identity; break;
            case (int)DB_Direction.Z: pbc.rotation = Quaternion.AngleAxis(90, Vector3.right); break;
        }
        return pbc;
    }
    static VRCPhysBone MigrateDynamicBone(Component obj, Component[] dbColliders, VRCPhysBoneCollider[] pbColliders)
    {
        var type = TypeDynamicBone;

        var root = (Transform)type.GetField("m_Root").GetValue(obj);
        var exclusions = (List<Transform>)type.GetField("m_Exclusions").GetValue(obj);
        if (root == null)
            return null;

        List<Transform> affectedTransforms = FindNonExcludedChildren(root, exclusions.ToList<Transform>());
        if (affectedTransforms == null || affectedTransforms.Count > PhysBoneManager.MAX_TRANSFORMS_PER_CHAIN)
            return null;

        float scaleConversion = Mathf.Abs(obj.transform.lossyScale.x) / Mathf.Abs(root.lossyScale.x);

        var physBone = obj.gameObject.AddComponent<VRCPhysBone>();

        physBone.isAnimated = false;
        physBone.rootTransform = root;
        physBone.ignoreTransforms = exclusions;

        //EndLength/EndOffset
        //Dynamic bone uses either endLength or endOffset, but not both, in that order
        float endLength = Convert.ToSingle(type.GetField("m_EndLength").GetValue(obj));
        Vector3 endOffset = (Vector3)type.GetField("m_EndOffset").GetValue(obj);
        if (endLength > 0f)
        {
            //Base off the length and direction of the last bone.
            //Not affected by the leaf bone's scale or rotation.
            foreach (Transform leaf in affectedTransforms.Where(t => t.childCount == 0 && t.parent != null))
            {
                var normal = leaf.position - leaf.parent.position;
                var endpoint = leaf.position + normal * endLength;

                GameObject tipBone = new GameObject("VRCLeafTipBone");
                tipBone.transform.SetParent(leaf);
                tipBone.transform.localRotation = Quaternion.identity;
                tipBone.transform.localScale = Vector3.one;
                tipBone.transform.localPosition = leaf.InverseTransformPoint(endpoint);
            }
        }
        else if (endOffset.magnitude > 0f)
        {
            //Based on the root rotation, but not scale
            foreach (Transform leaf in affectedTransforms.Where(t => t.childCount == 0 && t.parent != null))
            {
                var endpoint = leaf.position + root.TransformDirection(endOffset);

                GameObject tipBone = new GameObject("VRCLeafTipBone");
                tipBone.transform.SetParent(leaf);
                tipBone.transform.localRotation = Quaternion.identity;
                tipBone.transform.localScale = Vector3.one;
                tipBone.transform.localPosition = leaf.InverseTransformPoint(endpoint);
            }
        }
        physBone.InitTransforms();

        //Pull
        var elasticityDistrib = (AnimationCurve)type.GetField("m_ElasticityDistrib").GetValue(obj);
        float elasticity = (float)type.GetField("m_Elasticity").GetValue(obj);
        if (elasticityDistrib == null || elasticityDistrib.length == 0)
        {
            physBone.pull = ElastToPull.EvaluateForY(elasticity);
            physBone.pullCurve = null;
        }
        else
        {
            physBone.pull = 1f;
            var keys = new Keyframe[physBone.maxBoneChainIndex];
            for (int i = 0; i < keys.Length; i++)
            {
                var ratio = (float)i / (float)(keys.Length - 1);
                keys[i] = new Keyframe(ratio, ElastToPull.EvaluateForY(CalcDbElasticity(ratio)));
            }
            physBone.pullCurve = new AnimationCurve(keys);
        }

        //Spring
        var dampingDistrib = (AnimationCurve)type.GetField("m_DampingDistrib").GetValue(obj);
        float damping = (float)type.GetField("m_Damping").GetValue(obj);
        if ((dampingDistrib == null || dampingDistrib.length == 0) && (elasticityDistrib == null || elasticityDistrib.length == 0))
        {
            physBone.spring = Mathf.Clamp(ElastToSpring.EvaluateForY(elasticity) - damping, 0.0f, 1.0f);
            physBone.springCurve = null;
        }
        else
        {
            physBone.spring = 1f;
            var keys = new Keyframe[physBone.maxBoneChainIndex];
            for (int i = 0; i < keys.Length; i++)
            {
                var ratio = (float)i / (float)(keys.Length - 1);
                keys[i] = new Keyframe(ratio, Mathf.Clamp(ElastToSpring.EvaluateForY(CalcDbElasticity(ratio)) - CalcDbDamping(ratio), 0.0f, 1.0f));
            }
            physBone.springCurve = new AnimationCurve(keys);
        }

        float CalcDbElasticity(float t)
        {
            return SafeEvaluate(elasticityDistrib, t) * elasticity;
        }
        float CalcDbDamping(float t)
        {
            return SafeEvaluate(dampingDistrib, t) * damping;
        }
        float SafeEvaluate(AnimationCurve curve, float t)
        {
            if (curve != null && curve.length > 0)
                return curve.Evaluate(t);
            else
                return 1f;
        }

        //Immobile
        physBone.immobile = (float)type.GetField("m_Inert").GetValue(obj);
        physBone.immobileCurve = (AnimationCurve)type.GetField("m_InertDistrib").GetValue(obj);

        //Radius
        physBone.radius = (float)type.GetField("m_Radius").GetValue(obj) * scaleConversion;
        physBone.radiusCurve = (AnimationCurve)type.GetField("m_RadiusDistrib").GetValue(obj);

        //Freeze Axis
        DB_FreezeAxis freezeAxis = (DB_FreezeAxis)(int)type.GetField("m_FreezeAxis").GetValue(obj);
        physBone.freezeAxis = freezeAxis != (int)DB_FreezeAxis.None;
        physBone.freezeAxisAngle = 0f;
        switch (freezeAxis)
        {
            case DB_FreezeAxis.None: physBone.staticFreezeAxis = Vector3.zero; break;
            case DB_FreezeAxis.X: physBone.staticFreezeAxis = Vector3.right; break;
            case DB_FreezeAxis.Y: physBone.staticFreezeAxis = Vector3.up; break;
            case DB_FreezeAxis.Z: physBone.staticFreezeAxis = Vector3.forward; break;
        }

        //Max Angle
        physBone.maxAngle = StiffToMaxAngle.EvaluateForY((float)type.GetField("m_Stiffness").GetValue(obj));
        physBone.maxAngleCurve = (AnimationCurve)type.GetField("m_StiffnessDistrib").GetValue(obj);

        //Gravity
        //We only apply force.  DB gravity does not translate to PB.
        //DB forces are based on the scale factor, and PB is based on world bone length.
        //As a result we apply the root lossy scale. Technically DB is per bone scale, but that doesn't translate to PB.
        var force = (Vector3)type.GetField("m_Force").GetValue(obj);
        physBone.gravity = -force.y * Mathf.Abs(obj.transform.lossyScale.x);

        //Colliders that interact with this bone
        var colliders = (IList)type.GetField("m_Colliders").GetValue(obj);
        foreach (var dbc in colliders)
        {
            //Find index
            var index = -1;
            for (int i = 0; i < dbColliders.Length; i++)
            {
                if ((object)dbColliders[i] == dbc)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
                physBone.colliders.Add(pbColliders[index]);
        }

        return physBone;
    }
    static bool IsPartOfHand(Animator animator, Transform transform)
    {
        if (animator == null || !animator.isHuman || transform == null)
            return false;

        //Left
        var wrist = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        if (wrist != null)
        {
            if (IsChildOf(transform, wrist))
                return true;
        }

        //Right
        wrist = animator.GetBoneTransform(HumanBodyBones.RightHand);
        if (wrist != null)
        {
            if (IsChildOf(transform, wrist))
                return true;
        }

        bool IsChildOf(Transform source, Transform target)
        {
            if (source == target)
                return true;
            if (source.parent != null)
                return IsChildOf(source.parent, target);
            return false;
        }

        return false;
    }
    static List<Transform> FindNonExcludedChildren(Transform root, List<Transform> exclusions)
    {
        List<Transform> newList = new List<Transform>();

        if (!exclusions.Contains(root))
        {
            newList.Append(root);
            if (root.childCount > 0)
            {
                foreach (Transform child in root)
                {
                    newList.Concat(FindNonExcludedChildren(child, exclusions));
                }
            }
        }

        return newList;
    }
}
