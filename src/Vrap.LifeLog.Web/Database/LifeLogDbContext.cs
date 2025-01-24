using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Vrap.LifeLog.Web.Database;

public sealed class LifeLogDbContext(DbContextOptions<LifeLogDbContext> options) : IdentityDbContext(options);
