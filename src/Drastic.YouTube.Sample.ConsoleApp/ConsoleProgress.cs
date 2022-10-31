// <copyright file="ConsoleProgress.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;
using System.IO;

namespace Drastic.YouTube.Sample.ConsoleApp
{
    internal class ConsoleProgress : IProgress<double>, IDisposable
    {
        private readonly TextWriter writer;
        private readonly int posX;
        private readonly int posY;

        private int lastLength;

        public ConsoleProgress(TextWriter writer)
        {
            this.writer = writer;
            this.posX = Console.CursorLeft;
            this.posY = Console.CursorTop;
        }

        public ConsoleProgress()
            : this(Console.Out)
        {
        }

        public void Report(double progress) => this.Write($"{progress:P1}");

        public void Dispose() => this.EraseLast();

        private void EraseLast()
        {
            if (this.lastLength > 0)
            {
                Console.SetCursorPosition(this.posX, this.posY);
                this.writer.Write(new string(' ', this.lastLength));
                Console.SetCursorPosition(this.posX, this.posY);
            }
        }

        private void Write(string text)
        {
            this.EraseLast();
            this.writer.Write(text);
            this.lastLength = text.Length;
        }
    }
}