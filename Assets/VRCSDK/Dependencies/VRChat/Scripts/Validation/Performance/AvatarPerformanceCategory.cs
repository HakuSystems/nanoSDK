namespace VRC.SDKBase.Validation.Performance
{
    public enum AvatarPerformanceCategory
    {
        None,

        Overall,

        DownloadSize,
        PolyCount,
        AABB,
        SkinnedMeshCount,
        MeshCount,
        MaterialCount,
        DynamicBoneComponentCount,
        DynamicBoneSimulatedBoneCount,
        DynamicBoneColliderCount,
        DynamicBoneCollisionCheckCount,
        PhysBoneComponentCount,
        PhysBoneTransformCount,
        PhysBoneColliderCount,
        PhysBoneCollisionCheckCount,
        ContactCount,
        AnimatorCount,
        BoneCount,
        LightCount,
        ParticleSystemCount,
        ParticleTotalCount,
        ParticleMaxMeshPolyCount,
        ParticleTrailsEnabled,
        ParticleCollisionEnabled,
        TrailRendererCount,
        LineRendererCount,
        ClothCount,
        ClothMaxVertices,
        PhysicsColliderCount,
        PhysicsRigidbodyCount,
        AudioSourceCount,

        AvatarPerformanceCategoryCount
    }
}
