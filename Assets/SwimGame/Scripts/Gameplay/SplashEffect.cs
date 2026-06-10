using UnityEngine;

public static class SplashEffect
{
    private static Material material;

    public static void Play(Vector3 position)
    {
        var go = new GameObject("Splash");
        go.transform.position = position;
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.3f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.25f, 0.4f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1f, 2.4f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.15f);
        main.startColor = new Color(0.8f, 0.93f, 1f, 0.9f);
        main.gravityModifier = 1.6f;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 12) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.15f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetMaterial();
        renderer.sortingOrder = 20;

        ps.Play();
        Object.Destroy(go, 1f);
    }

    private static Material GetMaterial()
    {
        if (material == null)
        {
            material = new Material(Shader.Find("Sprites/Default"));
            material.mainTexture = SpriteFactory.CircleTexture;
        }
        return material;
    }
}