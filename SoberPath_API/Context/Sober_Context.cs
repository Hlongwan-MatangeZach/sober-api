using Microsoft.EntityFrameworkCore;
using SoberPath_API.Models;

namespace SoberPath_API.Context
{
    public class Sober_Context(DbContextOptions<Sober_Context>options):DbContext(options)
    {
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Social_Worker> Social_Workers => Set<Social_Worker>();
        public DbSet<NGO_Admin> NGO_Admins => Set<NGO_Admin>();
        public DbSet<Rehab_Admin> Rehab_Admins => Set<Rehab_Admin>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Substance> Substances => Set<Substance>();

        public DbSet<Session> Sessions => Set<Session>();

        public DbSet<Application> Applications => Set<Application>();


        public DbSet<Client_Assignment> ClientAssignments => Set<Client_Assignment>();


        public DbSet<Social_Worker_Schedule> Social_Worker_Schedules=>Set<Social_Worker_Schedule>();

        public DbSet<Next_of_Kin> Next_Of_Kins => Set<Next_of_Kin>();

        public DbSet<Rehab_Admission> Rehab_Admissions => Set<Rehab_Admission>();
        public DbSet<Room> rooms => Set<Room>();

        public DbSet<Rehabilitation_Progress> Rehabilitation_Progresses => Set<Rehabilitation_Progress>();

        public DbSet<Records> Records => Set<Records>();

        public DbSet<Event> Events => Set<Event>();

    }
}
