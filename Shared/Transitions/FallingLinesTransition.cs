﻿#region File Description
//-----------------------------------------------------------------------------
// FallingLinesTransition
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
#endregion

namespace WaveEngine.Components.Transitions
{
    /// <summary>
    /// Transition effect where each square of the image appears at a different time.
    /// </summary>
    public class FallingLinesTransition : ScreenTransition
    {
        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <value>
        /// The segments.
        /// </value>
        private int segments;

        /// <summary>
        /// The sprite batch
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Source transition renderTarget
        /// </summary>
        private RenderTarget sourceRenderTarget;

        /// <summary>
        /// Target transition renderTarget
        /// </summary>
        private RenderTarget targetRenderTarget;

        /// <summary>
        /// Gets or sets the segments.
        /// </summary>
        /// <exception cref="System.ArgumentException">Out of range, segments >= 3</exception>
        public int Segments
        {
            get
            {
                return this.segments;
            }

            set
            {
                if (value < 3)
                {
                    throw new ArgumentException("Out of range, segments >= 3");
                }

                this.segments = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FallingLinesTransition" /> class.
        /// </summary>
        /// <param name="duration">The duration.</param>
        public FallingLinesTransition(TimeSpan duration)
            : base(duration)
        {
            this.segments = 60;
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
        }

        /// <summary>
        /// Updates the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Update(TimeSpan gameTime)
        {
            this.UpdateSources(gameTime);
            this.UpdateTarget(gameTime);
        }

        /// <summary>
        /// Draws the specified game time.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        protected override void Draw(TimeSpan gameTime)
        {
            this.sourceRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(
                this.platform.ScreenWidth,
                this.platform.ScreenHeight);
            this.targetRenderTarget = this.graphicsDevice.RenderTargets.GetTemporalRenderTarget(
                this.platform.ScreenWidth,
                this.platform.ScreenHeight);

            System.Random random = new System.Random(23);

            this.DrawSources(gameTime, this.sourceRenderTarget);
            this.DrawTarget(gameTime, this.targetRenderTarget);

            this.SetRenderState();
            this.graphicsDevice.RenderTargets.SetRenderTarget(null);
            this.graphicsDevice.Clear(ref this.BackgroundColor, ClearFlags.Target | ClearFlags.DepthAndStencil, 1);

            this.spriteBatch.Draw(this.targetRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.5f);

            int width = this.sourceRenderTarget.Width;
            int height = this.sourceRenderTarget.Height;

            for (int x = 0; x < this.segments; x++)
            {
                int wsize = (width / this.segments) + 1;
                Rectangle rect = new Rectangle(wsize * x, 0, wsize, height);

                Vector2 pos = new Vector2(rect.X, 0);

                pos.Y += height * (float)Math.Pow(this.Lerp, random.NextDouble() * 10);

                this.spriteBatch.Draw(this.sourceRenderTarget, pos, rect, Color.White, 0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0f);
            }

            this.spriteBatch.Render();

            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.sourceRenderTarget);
            this.graphicsDevice.RenderTargets.ReleaseTemporalRenderTarget(this.targetRenderTarget);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.spriteBatch.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
