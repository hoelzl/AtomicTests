using System.IO;

using AtomicEngine;
using AtomicPlayer;

public class AtomicMain : AppDelegate
{
    // scene switch time in seconds
    float switchTime = 5.0f;

    int currentScene = 0;
    string[] scenes = { "", "Scene", "", "Scene2" };
    Scene scene;
    bool replaceTexture;

    public override void Start()
    {
        AddExternalResourcesPathToCache();

        var ui = GetSubsystem<UI>();

        // set up the DebugHud to show metrics and update at 10hz
        ui.SetDebugHudProfileMode(DebugHudProfileMode.DEBUG_HUD_PROFILE_METRICS);
        ui.ShowDebugHud(true);
        ui.DebugHudRefreshInterval = 0.1f;

        switchScene();

        SubscribeToEvent<UpdateEvent>(e =>
        {

            switchTime -= e.TimeStep;

            if (switchTime <= 0.0f)
            {
                switchTime = 10.0f;

                switchScene();
            }

        });
    }

    void switchScene()
    {
        var player = GetSubsystem<Player>();

        player.UnloadAllScenes();
        scene = null;

        // unload resources from cache
        GetSubsystem<ResourceCache>().ReleaseAllResources(false);

        string sceneName = scenes[currentScene++];

        if (!string.IsNullOrEmpty(sceneName))
        {
            scene = player.LoadScene("Scenes/" + sceneName + ".scene");
            ReplaceTexture();
        }

        currentScene %= scenes.Length;

    }

    void ReplaceTexture()
    {
        if (replaceTexture)
        {
            StaticModel model = scene.GetComponent<StaticModel>(true);
            if (model != null)
            {
                ResourceCache cache = GetSubsystem<ResourceCache>();
                Texture2D texture = cache.GetResource<Texture2D>("checkerboard.png");
                Material material = model.GetMaterial(0);
                if (texture != null && material != null)
                {
                    material.SetTexture(TextureUnit.TU_DIFFUSE, texture);
                }
            }
        }
        replaceTexture = !replaceTexture;
    }

    void AddExternalResourcesPathToCache()
    {
        ResourceCache cache = GetSubsystem<ResourceCache>();

        string mainScriptPath = cache.GetResourceFileName("Scripts/AtomicMain.cs");
        string externalResourcesDir = Path.GetDirectoryName(mainScriptPath);
        externalResourcesDir = Path.Combine(externalResourcesDir, "../../ExternalResources");

        cache.AddResourceDir(externalResourcesDir);
    }

}
