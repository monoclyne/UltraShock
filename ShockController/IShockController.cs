// This file was originally part of PeakShock, Copyright (c) 2025 Addzeey.
// Available at https://github.com/addzeey/PeakShock
// Licensed under the MIT License.
// See https://opensource.org/licenses/MIT for details.

#nullable enable
namespace ShockController;

public interface IShockController
{
    void EnqueueShock(int intensity, int duration_ms, string? code = null);
}
