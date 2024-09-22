// <copyright file="ProgressExtensions.cs" company="JSSoft">
//   Copyright (c) 2024 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Commands.Extensions;

public static class ProgressExtensions
{
    public static void Report(this IProgress<ProgressInfo> @this, double value, string text)
        => @this.Report(new ProgressInfo() { Value = value, Text = text });

    public static void Complete(this IProgress<ProgressInfo> @this, string text)
        => @this.Report(new ProgressInfo() { Value = double.MaxValue, Text = text });

    public static StepProgress<string> PreProgress(
        this IProgress<ProgressInfo> @this, double begin, double end, int length)
    {
        if (begin < 0)
        {
            throw new ArgumentException(
                $"{nameof(begin)} must be greater than or equal to 0.", nameof(begin));
        }

        if (begin > end)
        {
            throw new ArgumentException(
                $"{nameof(begin)} must be less than or equal to {nameof(end)}.", nameof(begin));
        }

        if (end > 1)
        {
            throw new ArgumentException(
                $"{nameof(end)} must be less than or equal to 1.", nameof(end));
        }

        var preProgress = new StepProgress<string>(length);
        preProgress.ProgressChanged += (s, e) =>
        {
            var step = preProgress.Step;
            var gap = (end - begin) / length;
            var progress = (step * gap) + begin;
            @this.Report(new ProgressInfo(progress, e));
        };

        return preProgress;
    }

    public static StepProgress<string> PostProgress(
        this IProgress<ProgressInfo> @this, double begin, double end, int length)
    {
        if (begin < 0)
        {
            throw new ArgumentException(
                $"{nameof(begin)} must be greater than or equal to 0.", nameof(begin));
        }

        if (begin > end)
        {
            throw new ArgumentException(
                $"{nameof(begin)} must be less than or equal to {nameof(end)}.", nameof(begin));
        }

        if (end > 1)
        {
            throw new ArgumentException(
                $"{nameof(end)} must be less than or equal to 1.", nameof(end));
        }

        var postProgress = new StepProgress<string>(length);
        postProgress.ProgressChanged += (s, e) =>
        {
            var step = postProgress.Step + 1;
            var gap = (end - begin) / length;
            var progress = Math.Min((step * gap) + begin, end);
            @this.Report(new ProgressInfo(progress, e));
        };

        return postProgress;
    }
}
