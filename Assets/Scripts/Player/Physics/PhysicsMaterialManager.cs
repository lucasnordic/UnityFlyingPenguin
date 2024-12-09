using UnityEngine;

public class PhysicsMaterialManager : MonoBehaviour
{
    private PhysicsMaterial2D flyingMaterial;
    private PhysicsMaterial2D groundMaterial;
    private Collider2D playerCollider;

    void Awake()
    {
        SetupMaterials();
        playerCollider = GetComponent<Collider2D>();
    }

    private void SetupMaterials()
    {
        flyingMaterial = new PhysicsMaterial2D("FlyingMaterial")
        {
            bounciness = 0.5f,
            friction = 0.4f
        };

        groundMaterial = new PhysicsMaterial2D("groundMaterial")
        {
            bounciness = 0f,
            friction = 0.6f
        };
    }

    public void SetFlyingMaterial()
    {
        playerCollider.sharedMaterial = flyingMaterial;
    }

    public void SetGroundMaterial()
    {
        playerCollider.sharedMaterial = groundMaterial;
    }
}