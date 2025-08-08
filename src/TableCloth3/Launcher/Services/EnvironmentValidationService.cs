using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableCloth3.Launcher.Services;

public sealed class EnvironmentValidationService
{
    public async Task ValidateEnvironmentAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO: Check WindowsSandbox executable
        // TODO: Check current directory name
        await Task.CompletedTask.ConfigureAwait(false);
    }
}
