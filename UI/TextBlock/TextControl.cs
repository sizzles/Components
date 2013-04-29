﻿#region File Description
//-----------------------------------------------------------------------------
// TextControl
// Copyright © 2010 - 2013 Wave Coorporation. All rights reserved.
// Use is subject to license terms.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Resources;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.Framework.UI;

#endregion

namespace WaveEngine.Components.UI
{
    /// <summary>
    ///     The text block.
    /// </summary>
    public class TextControl : Control
    {
        #region Static Fields

        /// <summary>
        ///     Total number of instances.
        /// </summary>
        private static int instances;

        #endregion

        #region Fields

        /// <summary>
        /// The disposed.
        /// </summary>
        protected bool disposed;

        /// <summary>
        /// The is global asset.
        /// </summary>
        protected bool isGlobalAsset;

        /// <summary>
        /// The font path.
        /// </summary>
        private readonly string fontPath;

        /// <summary>
        /// The line spacing.
        /// </summary>
        private float lineSpacing;

        /// <summary>
        /// The text.
        /// </summary>
        private string text;

        /// <summary>
        /// The line width
        /// </summary>
        private int lineWidth;

        /// <summary>
        /// The text wrapping.
        /// </summary>
        private bool textWrapping;

        /// <summary>
        /// The text alignment
        /// </summary>
        private TextAlignment textAlignment;

        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TextControl" /> class.
        /// </summary>
        public TextControl()
            : base("TextControl" + instances++)
        {
            this.LinesInfo = new List<LineInfo>();
            this.Foreground = Color.White;
            this.text = string.Empty;
            this.lineWidth = -1;
            this.Width = 1;
            this.Height = 1;
            this.textWrapping = false;
            this.lineSpacing = 0;
            this.Transform2D = null;
            this.textAlignment = TextAlignment.Left;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControl"/> class.
        /// </summary>
        /// <param name="fontPath">
        /// The font path.
        /// </param>
        public TextControl(string fontPath)
            : this(fontPath, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextControl"/> class. 
        /// </summary>
        /// <param name="fontPath">
        /// The font Path.
        /// </param>
        /// <param name="isGlobalAsset">
        /// The is Global Asset.
        /// </param>
        public TextControl(string fontPath, bool isGlobalAsset)
            : base("TextControl" + instances++)
        {
            if (string.IsNullOrEmpty(fontPath))
            {
                throw new ArgumentNullException("fontPath cannot be null.");
            }

            this.fontPath = fontPath;
            this.isGlobalAsset = isGlobalAsset;
            this.LinesInfo = new List<LineInfo>();
            this.Foreground = Color.White;
            this.text = string.Empty;
            this.lineWidth = -1;
            this.Width = 1;
            this.Height = 1;
            this.textWrapping = false;
            this.lineSpacing = 0;
            this.Transform2D = null;
            this.textAlignment = TextAlignment.Left;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the color of the fore.
        /// </summary>
        /// <value>
        ///     The color of the fore.
        /// </value>
        public Color Foreground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [perssistent asset].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [perssistent asset]; otherwise, <c>false</c>.
        /// </value>
        public bool IsGlobalAsset
        {
            get
            {
                return this.isGlobalAsset;
            }

            set
            {
                if (this.isInitialized)
                {
                    throw new InvalidOperationException("Asset has already initialized.");
                }

                this.isGlobalAsset = value;
            }
        }

        /// <summary>
        /// Gets or sets the line spacing.
        /// </summary>
        /// <value>
        ///     The line spacing.
        /// </value>
        public float LineSpacing
        {
            get
            {
                return this.lineSpacing;
            }

            set
            {
                this.lineSpacing = value;
            }
        }

        /// <summary>
        /// Gets the spritefont.
        /// </summary>
        public SpriteFont SpriteFont { get; private set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        ///     The text.
        /// </value>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value;
                this.UpdateSize();
            }
        }

        /// <summary>
        /// Gets or sets the width of the line.
        /// </summary>
        /// <value>
        /// The width of the line.
        /// </value>
        public int LineWidth
        {
            get
            {
                return this.lineWidth;
            }

            set
            {
                this.lineWidth = value;
                this.width = this.lineWidth;
                this.UpdateSize();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [text wrapping].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [text wrapping]; otherwise, <c>false</c>.
        /// </value>
        public bool TextWrapping
        {
            get
            {
                return this.textWrapping;
            }

            set
            {
                this.textWrapping = value;
                this.UpdateSize();
            }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public new float Width
        {
            get
            {
                return base.Width;
            }

            protected set
            {
                base.Width = value;
            }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>
        /// The height.
        /// </value>
        public new float Height
        {
            get
            {
                return base.Height;
            }

            protected set
            {
                base.Height = value;
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>
        /// The text alignment.
        /// </value>
        public TextAlignment TextAlignment
        {
            get
            {
                return this.textAlignment;
            }

            set
            {
                if (this.textAlignment != value)
                {
                    this.textAlignment = value;
                    this.UpdateAlignment();
                }
            }
        }
        #endregion

        #region Private Properties

        /// <summary>
        /// Gets the height of the font.
        /// </summary>
        internal float FontHeight { get; private set; }

        /// <summary>
        /// Gets the text lines.
        /// </summary>
        internal List<LineInfo> LinesInfo { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (!this.IsGlobalAsset && !string.IsNullOrEmpty(this.fontPath))
                    {
                        Assets.UnloadAsset(this.fontPath);
                    }

                    this.disposed = true;
                }
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            if (!string.IsNullOrEmpty(this.fontPath))
            {
                if (this.isGlobalAsset)
                {
                    this.SpriteFont = WaveServices.Assets.Global.LoadAsset<SpriteFont>(this.fontPath);
                }
                else
                {
                    this.SpriteFont = Assets.LoadAsset<SpriteFont>(this.fontPath);
                }
            }
            else
            {
                this.SpriteFont = StaticResources.DefaultSpriteFont;
            }

            this.isInitialized = true;
            this.UpdateSize();
        }

        /// <summary>
        /// Updates the size.
        /// </summary>
        private void UpdateSize()
        {
            if (!this.isInitialized)
            {
                return;
            }

            this.LinesInfo.Clear();

            if (!string.IsNullOrEmpty(this.text) && this.SpriteFont != null)
            {
                this.FontHeight = this.SpriteFont.MeasureString("A").Y;

                if (this.textWrapping)
                {
                    // Filters
                    this.text = this.text.Replace("\r\n", " /n ");
                    this.text = this.text.Replace("\n", " /n ");
                    this.text = this.text.Replace("/n", " /n ");
                    
                    string[] words = this.text.Split(' ');
                    var stringBuilder = new StringBuilder();

                    int i = 0;
                    while (i < words.Length)
                    {
                        do
                        {
                            if (words[i].Equals("/n"))
                            {
                                i++;
                                break;
                            }

                            string nextString = string.Format("{0}{1} ", stringBuilder, words[i]);
                            float lineSize = this.SpriteFont.MeasureString(nextString).X;
                            if (stringBuilder.Length != 0 && lineSize > this.Width)
                            {
                                break;
                            }

                            stringBuilder.Append(words[i]);
                            stringBuilder.Append(" ");
                            i++;
                        }
                        while (i < words.Length);

                        string text = stringBuilder.ToString();
                        Vector2 size = this.SpriteFont.MeasureString(text);
                        float offsetX = this.CalculateAlignmentOffset(size);
                        this.LinesInfo.Add(new LineInfo(text, size, offsetX));
                        stringBuilder.Length = 0;
                    }

                    this.Height = this.LinesInfo.Count * (this.FontHeight + this.lineSpacing);
                }
                else
                {
                    Vector2 size = this.SpriteFont.MeasureString(this.text);
                    float offsetX = this.CalculateAlignmentOffset(size);
                    this.LinesInfo.Add(new LineInfo(this.text, size, offsetX));

                    if (this.lineWidth != -1)
                    {
                        this.Width = this.lineWidth;
                    }
                    else
                    {
                        this.Width = size.X;
                    }

                    this.Height = size.Y;
                }
            }
        }

        /// <summary>
        /// Calculates the alignment offset.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>Offset X</returns>
        public float CalculateAlignmentOffset(Vector2 size)
        {
            float offsetX = 0;

            switch (this.textAlignment)
            {
                case TextAlignment.Left:
                    break;
                case TextAlignment.Right:

                    if (size.X < this.width)
                    {
                        offsetX = this.width - size.X;
                    }

                    break;
                case TextAlignment.Center:

                    if (size.X < this.width)
                    {
                        offsetX = (this.width - size.X) / 2;
                    }

                    break;
            }

            return offsetX;
        }

        /// <summary>
        /// Updates the alignment.
        /// </summary>
        private void UpdateAlignment()
        {
            if (this.LinesInfo == null)
            {
                return;
            }

            for (int i = 0; i < this.LinesInfo.Count; i++)
            {
                LineInfo lineInfo = this.LinesInfo[i];
                lineInfo.AlignmentOffsetX = this.CalculateAlignmentOffset(this.LinesInfo[i].Size);
            }
        }

        #endregion
    }
}