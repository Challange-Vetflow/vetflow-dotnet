using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VetFlow.Infrastructure.Persistence;

public class VetFlowContextFactory : IDesignTimeDbContextFactory<VetFlowContext>
{
    public VetFlowContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<VetFlowContext>();
        optionsBuilder.UseOracle("Data Source=oracle.fiap.com.br:1521/orcl;User ID=RM562310;Password=270905;");
        return new VetFlowContext(optionsBuilder.Options);
    }
}