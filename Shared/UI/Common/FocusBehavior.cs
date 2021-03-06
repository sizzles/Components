﻿#region File Description
//-----------------------------------------------------------------------------
// FocusBehavior
//
// Copyright © 2016 Wave Engine S.L. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WaveEngine.Framework;
#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    /// Focused behavior
    /// </summary>
    public class FocusBehavior : Behavior, IDisposable
    {
        /// <summary>
        /// The current focus behavior
        /// </summary>
        public static FocusBehavior CurrentFocus;

        #region Events

        /// <summary>
        /// Occurs when [got focus].
        /// </summary>
        public event EventHandler GotFocus;

        /// <summary>
        /// Occurs when [lost focus].
        /// </summary>
        public event EventHandler LostFocus;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is focus.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is focus; otherwise, <c>false</c>.
        /// </value>
        public bool IsFocus
        {
            get
            {
                bool result = false;
                if (CurrentFocus == this)
                {
                    result = true;
                }

                return result;
            }

            set
            {
                if (value)
                {
                    if (CurrentFocus != null)
                    {
                        if (CurrentFocus.LostFocus != null)
                        {
                            CurrentFocus.LostFocus(this, new EventArgs());
                        }
                    }

                    CurrentFocus = this;

                    if (this.GotFocus != null)
                    {
                        this.GotFocus(this, new EventArgs());
                    }
                }
                else if (CurrentFocus == this)
                {
                    if (this.LostFocus != null)
                    {
                        this.LostFocus(this, new EventArgs());
                    }

                    CurrentFocus = null;
                }
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusBehavior" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public FocusBehavior(string name)
            : base(name)
        {
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // If this component was disposed. Remove the static reference to avoid unnecessary memory withholding 
            if (CurrentFocus == this)
            {
                CurrentFocus = null;
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Allows this instance to execute custom logic during its <c>Update</c>.
        /// </summary>
        /// <param name="gameTime">The game time.</param>
        /// <remarks>
        /// This method will not be executed if the <see cref="Component" />, or the <see cref="Entity" />
        /// owning it are not <c>Active</c>.
        /// </remarks>
        protected override void Update(TimeSpan gameTime)
        {
        }

        #endregion
    }
}
