using DG.Tweening;
using UnityEngine;

public static class CollisionEffect
{
    private static Material material;

    public static void Play(Vector3 position)
    {
        SpawnFlash(position);
        SpawnBurst(position);
    }

    private static void SpawnFlash(Vector3 position)
    {
        var go = new GameObject("CollisionFlash");
        go.transform.position = position;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Circle;
        sr.color = new Color(1f, 1f, 1f, 0.9f);
        sr.sortingOrder = 25;
        go.transform.localScale = Vector3.zero;
        go.transform.DOScale(1.6f, 0.25f).SetEase(Ease.OutQuad);
        sr.DOFade(0f, 0.3f).SetEase(Ease.OutQuad).OnComplete(() => Object.Destroy(go));
    }

    private static void SpawnBurst(Vector3 position)
    {
        var go = new GameObject("CollisionBurst");
        go.transform.position = position;
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 0.4f;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.55f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 4.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.2f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color32(231, 76, 60, 255),
            new Color32(255, 255, 255, 230));
        main.gravityModifier = 0.8f;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 22) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = GetMaterial();
        renderer.sortingOrder = 26;

        ps.Play();
        Object.Destroy(go, 1.2f);
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
