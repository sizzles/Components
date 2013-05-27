#region File Description
//-----------------------------------------------------------------------------
// AnimationState
//
// Copyright (c) 2013, Esoteric Software
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this
//    list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
#endregion

namespace WaveEngine.Components.Animation.Spine
{
    /// <summary>
    /// AnimationState class
    /// </summary>
    public class AnimationState
    {
        /// <summary>
        /// The previous
        /// </summary>
        private Animation previous;

        /// <summary>
        /// The previous time
        /// </summary>
        private float previousTime;

        /// <summary>
        /// The previous loop
        /// </summary>
        private bool previousLoop;

        /// <summary>
        /// mix and mixDuration time
        /// </summary>
        private float mixTime, mixDuration;

        /// <summary>
        /// The queue
        /// </summary>
        private List<QueueEntry> queue = new List<QueueEntry>();

        #region Properties
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public AnimationStateData Data { get; private set; }

        /// <summary>
        /// Gets the animation.
        /// </summary>
        /// <value>
        /// The animation.
        /// </value>
        public Animation Animation { get; private set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public float Time { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AnimationState" /> is loop.
        /// </summary>
        /// <value>
        ///   <c>true</c> if loop; otherwise, <c>false</c>.
        /// </value>
        public bool Loop { get; set; }
        #endregion

        #region Initialize
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationState" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="System.ArgumentNullException">data cannot be null.</exception>
        public AnimationState(AnimationStateData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data cannot be null.");
            }

            this.Data = data;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Updates the specified delta.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void Update(float delta)
        {
            this.Time += delta;
            this.previousTime += delta;
            this.mixTime += delta;

            if (this.queue.Count > 0)
            {
                QueueEntry entry = this.queue[0];
                if (this.Time >= entry.Delay)
                {
                    this.SetAnimationInternal(entry.Animation, entry.Loop);
                    this.queue.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Applies the specified skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        public void Apply(Skeleton skeleton)
        {
            if (Animation == null)
            {
                return;
            }

            if (this.previous != null)
            {
                this.previous.Apply(skeleton, this.previousTime, this.previousLoop);
                float alpha = this.mixTime / this.mixDuration;
                if (alpha >= 1)
                {
                    alpha = 1;
                    this.previous = null;
                }

                Animation.Mix(skeleton, this.Time, this.Loop, alpha);
            }
            else
            {
                Animation.Apply(skeleton, this.Time, this.Loop);
            }
        }

        /// <summary>
        /// Adds the animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void AddAnimation(string animationName, bool loop)
        {
            this.AddAnimation(animationName, loop, 0);
        }

        /// <summary>
        /// Adds the animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <param name="delay">The delay.</param>
        /// <exception cref="System.ArgumentException">Animation not found:  + animationName</exception>
        public void AddAnimation(string animationName, bool loop, float delay)
        {
            Animation animation = this.Data.SkeletonData.FindAnimation(animationName);

            if (animation == null)
            {
                throw new ArgumentException("Animation not found: " + animationName);
            }

            this.AddAnimation(animation, loop, delay);
        }

        /// <summary>
        /// Adds the animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void AddAnimation(Animation animation, bool loop)
        {
            this.AddAnimation(animation, loop, 0);
        }

        /// <summary>
        /// Adds an animation to be played delay seconds after the current or last queued animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <param name="delay">May be minor equal than 0 to use duration of previous animation minus any mix duration plus the negative delay.</param>
        public void AddAnimation(Animation animation, bool loop, float delay)
        {
            QueueEntry entry = new QueueEntry();
            entry.Animation = animation;
            entry.Loop = loop;

            if (delay <= 0)
            {
                Animation previousAnimation = this.queue.Count == 0 ? this.Animation : this.queue[this.queue.Count - 1].Animation;
                if (previousAnimation != null)
                {
                    delay = previousAnimation.Duration - this.Data.GetMix(previousAnimation, animation) + delay;
                }
                else
                {
                    delay = 0;
                }
            }

            entry.Delay = delay;
            this.queue.Add(entry);
        }

        /// <summary>
        /// Sets the animation.
        /// </summary>
        /// <param name="animationName">Name of the animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        /// <exception cref="System.ArgumentException">Animation not found:  + animationName</exception>
        public void SetAnimation(string animationName, bool loop)
        {
            Animation animation = this.Data.SkeletonData.FindAnimation(animationName);
            if (animation == null)
            {
                throw new ArgumentException("Animation not found: " + animationName);
            }

            this.SetAnimation(animation, loop);
        }

        /// <summary>
        /// Sets the animation.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        public void SetAnimation(Animation animation, bool loop)
        {
            this.queue.Clear();
            this.SetAnimationInternal(animation, loop);
        }

        /// <summary>
        /// Clears the animation.
        /// </summary>
        public void ClearAnimation()
        {
            this.previous = null;
            Animation = null;
            this.queue.Clear();
        }

        /// <summary>
        /// Determines whether this instance is complete.
        /// </summary>
        /// <returns>
        ///   Returns true if no animation is set or if the current time is greater than the animation duration, regardless of looping.
        /// </returns>
        public bool IsComplete()
        {
            return Animation == null || this.Time >= Animation.Duration;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return (Animation != null && Animation.Name != null) ? Animation.Name : base.ToString();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Sets the animation internal.
        /// </summary>
        /// <param name="animation">The animation.</param>
        /// <param name="loop">if set to <c>true</c> [loop].</param>
        private void SetAnimationInternal(Animation animation, bool loop)
        {
            this.previous = null;
            if (animation != null && Animation != null)
            {
                this.mixDuration = this.Data.GetMix(Animation, animation);

                if (this.mixDuration > 0)
                {
                    this.mixTime = 0;
                    this.previous = this.Animation;
                    this.previousTime = this.Time;
                    this.previousLoop = this.Loop;
                }
            }

            this.Animation = animation;
            this.Loop = loop;
            this.Time = 0;
        }
        #endregion
    }
}
