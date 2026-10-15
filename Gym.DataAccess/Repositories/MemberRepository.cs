using Gym.DataAccess.Data.Contexts;
using Gym.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gym.DataAccess.Repositories
{
    public class MemberRepository(GymDbContext dbContext) : Repository<Member>(dbContext), IMemberRepository
    {
    }
}
