﻿using System;
using System.Collections.Generic;

namespace Spectre.CommandLine.Internal.Rendering.Elements
{
    internal sealed class BlockElement : IRenderable
    {
        private readonly List<IRenderable> _elements;

        public int Length { get; private set; }

        public BlockElement()
        {
            _elements = new List<IRenderable>();
        }

        public BlockElement Append(IRenderable element)
        {
            _elements.Add(element);
            Length += element.Length;
            return this;
        }

        public BlockElement Append(Action<BlockElement> config)
        {
            config(this);
            return this;
        }

        public void Render(IRenderer renderer)
        {
            foreach (var element in _elements)
            {
                element.Render(renderer);
            }
        }
    }
}