using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Jumpy
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(JumpyPackage.PackageGuidString)]
    public sealed class JumpyPackage : AsyncPackage
    {
        public const string PackageGuidString = "bd1c14e2-a4d1-4483-990f-97766a593a5b";

        protected override async System.Threading.Tasks.Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }
    }
}
