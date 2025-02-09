﻿using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SWB_Base.UI
{
    public class RenderScopeRT : Panel
    {
        private Texture colorTexture;
        private Texture depthTexture;

        private Rect viewport;
        private float fieldOfView = 25f;
        const float baseFov = 50f;

        private RenderAttributes renderAttributes;
        private SceneObject sceneObject;

        public RenderScopeRT(SceneObject sceneObject)
        {
            if (!sceneObject.IsValid()) return;

            this.sceneObject = sceneObject;

            renderAttributes = new();
            viewport = new Rect(Vector2.Zero, Screen.Size / 2f);

            colorTexture = Texture.CreateRenderTarget()
                         .WithSize((int)viewport.width, (int)viewport.height)
                         .WithScreenFormat()
                         .WithScreenMultiSample()
                         .Create();

            depthTexture = Texture.CreateRenderTarget()
                         .WithSize((int)viewport.width, (int)viewport.height)
                         .WithDepthFormat()
                         .WithScreenMultiSample()
                         .Create();
        }

        public override void OnDeleted()
        {
            colorTexture.Dispose();
            depthTexture.Dispose();

            base.OnDeleted();
        }

        public override void DrawBackground(ref RenderState state)
        {
            Log.Info("Drawing");

            var player = Local.Pawn as PlayerBase;
            if (player == null || sceneObject == null) return;
            if (player.ActiveChild is not WeaponBase weapon) return;

            sceneObject.Flags.ViewModelLayer = true;

            if (sceneObject == null)
                return;

            if (!weapon.IsZooming)
                return;

            Render.Draw.DrawScene(colorTexture,
                    depthTexture,
                    Map.Scene,
                    renderAttributes,
                    viewport,
                    CurrentView.Position,
                    CurrentView.Rotation,
                    fieldOfView,
                    zNear: 32,
                    zFar: 25000);

            Render.Attributes.Set("ScopeRT", colorTexture);
            sceneObject.Attributes.Set("ScopeRT", colorTexture);
        }
    }
}
