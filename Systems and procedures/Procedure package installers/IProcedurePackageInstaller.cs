using System.Collections.Generic;

using HereticalSolutions.Systems.Builders;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageInstaller<
		TContext,
		TSystem,
		TProcedure>
		where TContext : ASystemBuilderContext<TSystem, TProcedure>
	{
		string PackageName { get; }

		IEnumerable<string> PackageDependencies { get; }

		bool CanInstall(
			IProcedurePackageInstallerContext<TContext, TSystem, TProcedure> context);

		void Install(
			IProcedurePackageInstallerContext<TContext, TSystem, TProcedure> context);
	}
}