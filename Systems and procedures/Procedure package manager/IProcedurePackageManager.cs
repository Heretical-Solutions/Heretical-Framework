using HereticalSolutions.Systems.Builders;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageManager<TContext, TSystem, TProcedure>
		where TContext : ASystemBuilderContext<TSystem, TProcedure>
	{
		#region Has

		bool HasPackageInstaller(
			string installerName);

		#endregion

		#region Add

		bool TryAddPackageInstaller(
			IProcedurePackageInstaller<TContext, TSystem, TProcedure> installer);

		#endregion

		#region Remove

		bool TryRemovePackageInstaller(
			string installerName);

		#endregion

		//Sort by dependencies, validate then run the ones that have reported CanInstall == true
		bool RunAllInstallers();
	}
}