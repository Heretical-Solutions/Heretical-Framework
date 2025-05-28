using HereticalSolutions.Repositories;

using HereticalSolutions.Systems.Builders;

namespace HereticalSolutions.Systems
{
	public interface IProcedurePackageInstallerContext<TContext, TSystem, TProcedure>
		where TContext : ASystemBuilderContext<TSystem, TProcedure>
	{
		IRepository<string, ISystemBuilder<TContext, TSystem, TProcedure>>
			Builders { get; }
	}
}