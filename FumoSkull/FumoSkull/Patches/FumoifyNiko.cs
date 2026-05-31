using FumoSkull;
using HarmonyLib;
using System;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;
namespace FumoSkull.Patches;
[HarmonyPatch(typeof(Readable), "Awake")]
class NikoBook
{
    static void Prefix(Readable __instance)
    {
        if (FumoSkulls.Config.IsNikoDisabled)
        {
            return;
        }


        var identifier = __instance.gameObject.GetComponentInChildren<ItemIdentifier>();

        if (identifier.name == "Book")
        {
            ReplaceBook(__instance);
        }
        else if (identifier.name == "BookTablet")
        {
            ReplaceTablet(__instance);
        }
    }

    static void ReplaceBook(Readable readable)
    {
        Renderer meshRenderer = readable.gameObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer)
        {
            meshRenderer.enabled = false;
            FumoSkulls.CreateFumo(
                Fumo.Niko,
                meshRenderer.transform,
                position: new Vector3(1.75f, -0.1f, -1.5f),
                rotation: Quaternion.Euler(0, 180, 0),
                scale: new Vector3(1, 1, 1) * 2.5f,
                meshRenderer.material.shader
            );
        }
    }

    static void ReplaceTablet(Readable readable)
    {
        var cube = readable.gameObject.transform.GetChild(0);

        Renderer meshRenderer = cube.gameObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer)
        {
            meshRenderer.enabled = false;
            FumoSkulls.CreateFumo(
                Fumo.Niko,
                meshRenderer.transform,
                position: new Vector3(1f, -0.1f, -1f),
                rotation: Quaternion.Euler(0, 180, 0),
                scale: new Vector3(1, 1, 1) * 2.5f,
                meshRenderer.material.shader
            );
        }
    }
}
[HarmonyPatch(typeof(Torch), "Start")]
static class NikoTorch
{
    static void Prefix(Torch __instance)
    {
        if (FumoSkulls.Config.IsNikoDisabled)
        {
            return;
        }

        Renderer meshRenderer = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer)
        {
            meshRenderer.enabled = false;
            FumoSkulls.CreateFumo(
                Fumo.Niko,
                meshRenderer.transform.parent.transform,
                position: new Vector3(0, 0.1f, 0),
                rotation: Quaternion.Euler(270, 270, 0),
                scale: new Vector3(1, 1, 1) * 2.75f,
                meshRenderer.material.shader
            );
        }
    }
}
[HarmonyPatch(typeof(Soap), "Start")]
static class NikoSoap
{
    static void Prefix(Soap __instance)
    {
        if (FumoSkulls.Config.IsNikoDisabled)
        {
            return;
        }

        Renderer masterSkull = __instance.gameObject.GetComponentInChildren<MeshRenderer>();
        if (masterSkull)
        {
            masterSkull.enabled = false;
            FumoSkulls.CreateFumo(
                Fumo.Niko,
                masterSkull.transform.parent.transform,
                position: new Vector3(0, 0.1f, 0),
                rotation: Quaternion.Euler(270, 270, 0),
                scale: new Vector3(1, 1, 1) * 2.75f,
                masterSkull.material.shader
            );
        }
    }
}
[HarmonyPatch(typeof(Skull), "Awake")]
static class NikoSkull
{
    static void Postfix(Skull __instance)
    {
        var skullType = __instance.GetComponent<ItemIdentifier>().itemType;

        switch (skullType)
        {
            case ItemType.SkullBlue:
                if (FumoSkulls.Config.IsNikoDisabled)
                {
                    return;
                }

                break;
            case ItemType.SkullRed:
                if (FumoSkulls.Config.IsNikoDisabled)
                {
                    return;
                }

                break;

            default:
                return;
        }

        ModifyMaterial modifyMaterial;
        try
        {
            modifyMaterial = Traverse.Create(__instance).Field<ModifyMaterial>("mod").Value;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get `mod` field of skull: {e.GetType()} {e.Message}");
            return;
        }


        Renderer renderer;
        try
        {
            var traverse = Traverse.Create(modifyMaterial);
            traverse.Method("SetValues").GetValue();
            renderer = traverse.Field<Renderer>("rend").Value;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to get `rend` field of modifyMaterial: {e.GetType()} {e.Message}");
            return;
        }

        if (renderer)
        {
            Fumo type;
            Vector3 position;
            switch (skullType)
            {
                case ItemType.SkullBlue:
                    type = Fumo.Niko;
                    position = new Vector3(0.05f, 0.03f, 0.1f);
                    break;

                case ItemType.SkullRed:
                    type = Fumo.Niko;
                    position = new Vector3(-0.015f, 0, 0.15f);
                    break;

                default:
                    return;
            }

            renderer.enabled = false;

            FumoSkulls.CreateFumo(
                type,
                renderer.transform,
                position: position,
                rotation: Quaternion.Euler(15, 0, 270),
                scale: new Vector3(0.8f, 0.8f, 0.8f),
                renderer.material.shader
            );
        }
        else
        {
            Debug.LogWarning("renderer was null");
        }
    }
}
[HarmonyPatch(typeof(Grenade), "Awake")]
static class NikoRocket
{
    static void Postfix(Grenade __instance)
    {
        if (__instance.rocket)
        {
            PatchRocket(__instance);
        }
        else
        {
            PatchCoreEject(__instance);
        }
    }

    static void PatchRocket(Grenade grenade)
    {
        if (FumoSkulls.Config.IsNikoDisabled)
        {
            return;
        }

        Renderer[] meshRenderer = grenade.gameObject.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderer.Length > 0)
        {
            for (int i = 0; i < meshRenderer.Length; i++)
            {
                meshRenderer[i].enabled = false;
            }

            FumoSkulls.CreateFumo(Fumo.Niko, grenade.transform,
                position: new Vector3(0f, 0f, 2f),
                rotation: Quaternion.Euler(0, 0, 90),
                scale: new Vector3(10f, 10f, 10f),
                meshRenderer[0].material.shader
            );
        }
    }

    static void PatchCoreEject(Grenade grenade)
    {
        if (FumoSkulls.Config.IsNikoDisabled)
        {
            return;
        }

        Renderer[] meshRenderer = grenade.gameObject.GetComponentsInChildren<MeshRenderer>();
        if (meshRenderer.Length > 0)
        {
            for (int i = 0; i < meshRenderer.Length; i++)
            {
                meshRenderer[i].enabled = false;
            }

            FumoSkulls.CreateFumo(Fumo.Niko, grenade.transform,
                position: new Vector3(0f, -0.5f, 2f),
                rotation: Quaternion.Euler(0, 0, 90),
                scale: new Vector3(3.5f, 3.5f, 3.5f),
                meshRenderer[0].material.shader
            );
        }
    }
}
[HarmonyPatch(typeof(Landmine), "Start")]
static class NikoLandmine
{
    static void Postfix(Landmine __instance)
    {
        if (FumoSkulls.Config.IsNikoDisabled)
        {
            return;
        }

        var renderer = __instance.gameObject.GetComponentInChildren<MeshRenderer>();

        var lightCylinder = Traverse.Create(__instance).Field<GameObject>("lightCylinder").Value;
        var cylinderRenderer = lightCylinder.GetComponentInChildren<MeshRenderer>();
        cylinderRenderer.enabled = false;


        if (renderer)
        {
            renderer.enabled = false;
            FumoSkulls.CreateFumo(
                Fumo.Niko,
                lightCylinder.transform,
                position: new Vector3(0, 0, 0),
                rotation: Quaternion.Euler(0, 270, 0),
                scale: new Vector3(1, 1, 1) * 0.001f,
                renderer.material.shader
            );
        }
    }
}