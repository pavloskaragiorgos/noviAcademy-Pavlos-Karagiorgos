using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorldRank.Infrastructure.Persistence.Context;

public class NoviAcademyContextFactory : IDesignTimeDbContextFactory<WorldRankDbContext>
{
    public WorldRankDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorldRankDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=NoviAcademy;Integrated Security=true;TrustServerCertificate=true");

        return new WorldRankDbContext(optionsBuilder.Options);
    }
}
// Add-Migration Init -OutputDir "Persistence/Context/Migrations" -Project "WorldRank.Infrastructure" -StartupProject "WorldRank.Infrastructure"