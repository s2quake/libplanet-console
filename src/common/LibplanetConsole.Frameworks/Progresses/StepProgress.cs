// <copyright file="StepProgress.cs" company="JSSoft">
//   Copyright (c) 2024 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Commands;

public sealed class StepProgress<T> : Progress<T>
    where T : notnull
{
    private static readonly object _obj = new();
    private int _step = -1;

    public StepProgress(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length), length, "The length must be greater than 0.");
        }

        Length = length;
    }

    public int Length { get; }

    public int Step => _step;

    public bool IsCompleted { get; private set; }

    public void Next(T value)
    {
        lock (_obj)
        {
            if (IsCompleted is true)
            {
                throw new InvalidOperationException("The progress has already been completed.");
            }

            if (_step + 1 >= Length)
            {
                throw new InvalidOperationException("The step limit has been exceeded.");
            }

            _step++;
            OnReport(value);
        }
    }

    public void Complete(T value)
    {
        lock (_obj)
        {
            if (IsCompleted is true)
            {
                throw new InvalidOperationException("The progress has already been completed.");
            }

            if (_step + 1 != Length)
            {
                throw new InvalidOperationException("There is a step that has not been completed.");
            }

            _step = Length;
            IsCompleted = true;
            OnReport(value);
        }
    }
}
