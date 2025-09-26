// Add this to your C# backend project
using Microsoft.EntityFrameworkCore;
using BarberShopAPI.Server.Data;
using BarberShopAPI.Server.Models;

namespace BarberShopAPI.Server.Repositories
{
    public abstract class BaseRepository<T> where T : class, ITenantEntity
    {
        protected readonly BarberShopContext _context;
        protected readonly int _tenantId;

        public BaseRepository(BarberShopContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _tenantId = GetTenantId(httpContextAccessor);
        }

        protected IQueryable<T> GetTenantQuery()
        {
            return _context.Set<T>().Where(e => e.TenantId == _tenantId);
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await GetTenantQuery().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await GetTenantQuery().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            // Ensure tenant ID is set
            entity.TenantId = _tenantId;
            
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            // Ensure tenant ID is set
            entity.TenantId = _tenantId;
            
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await GetTenantQuery().AnyAsync(e => EF.Property<int>(e, "Id") == id);
        }

        private int GetTenantId(IHttpContextAccessor httpContextAccessor)
        {
            var tenantId = httpContextAccessor.HttpContext?.Items["TenantId"];
            if (tenantId == null)
            {
                throw new InvalidOperationException("TenantId not found in HTTP context. Ensure TenantMiddleware is registered.");
            }
            return (int)tenantId;
        }
    }

    // Specific repositories
    public class ServiceRepository : BaseRepository<Service>
    {
        public ServiceRepository(BarberShopContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor) { }

        public async Task<List<Service>> GetActiveServicesAsync()
        {
            return await GetTenantQuery()
                .Where(s => s.IsActive)
                .ToListAsync();
        }
    }

    public class AppointmentRepository : BaseRepository<Appointment>
    {
        public AppointmentRepository(BarberShopContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor) { }

        public async Task<List<Appointment>> GetAppointmentsByDateAsync(DateTime date)
        {
            return await GetTenantQuery()
                .Where(a => a.AppointmentDate.Date == date.Date)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetUpcomingAppointmentsAsync()
        {
            var today = DateTime.Today;
            return await GetTenantQuery()
                .Where(a => a.AppointmentDate >= today)
                .Include(a => a.Service)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync(DateTime date, TimeSpan time)
        {
            return !await GetTenantQuery()
                .AnyAsync(a => a.AppointmentDate.Date == date.Date && 
                              a.AppointmentTime == time && 
                              a.Status != "Cancelled");
        }
    }

    public class AdminRepository : BaseRepository<Admin>
    {
        public AdminRepository(BarberShopContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor) { }

        public async Task<Admin?> GetByUsernameAsync(string username)
        {
            return await GetTenantQuery()
                .FirstOrDefaultAsync(a => a.Username.ToLower() == username.ToLower());
        }
    }

    public class AvailabilityScheduleRepository : BaseRepository<AvailabilitySchedule>
    {
        public AvailabilityScheduleRepository(BarberShopContext context, IHttpContextAccessor httpContextAccessor) 
            : base(context, httpContextAccessor) { }

        public async Task<List<AvailabilitySchedule>> GetByDayOfWeekAsync(string dayOfWeek)
        {
            return await GetTenantQuery()
                .Where(a => a.DayOfWeek == dayOfWeek && a.IsAvailable)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<List<AvailabilitySchedule>> GetAllSchedulesAsync()
        {
            return await GetTenantQuery()
                .OrderBy(a => a.DayOfWeek)
                .ThenBy(a => a.StartTime)
                .ToListAsync();
        }

        public async Task<AvailabilitySchedule?> GetByDayAndTimeAsync(string dayOfWeek, TimeSpan startTime)
        {
            return await GetTenantQuery()
                .FirstOrDefaultAsync(a => a.DayOfWeek == dayOfWeek && a.StartTime == startTime);
        }

        public async Task<bool> IsTimeSlotAvailableAsync(DateTime date, TimeSpan time)
        {
            var dayOfWeek = date.DayOfWeek.ToString();
            return await GetTenantQuery()
                .AnyAsync(a => a.DayOfWeek == dayOfWeek && 
                              a.IsAvailable &&
                              time >= a.StartTime && 
                              time < a.EndTime);
        }

        public async Task<List<string>> GetAvailableTimeSlotsAsync(DateTime date)
        {
            var dayOfWeek = date.DayOfWeek.ToString();
            var schedules = await GetTenantQuery()
                .Where(a => a.DayOfWeek == dayOfWeek && a.IsAvailable)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            var timeSlots = new List<string>();
            
            foreach (var schedule in schedules)
            {
                var currentTime = schedule.StartTime;
                while (currentTime < schedule.EndTime)
                {
                    timeSlots.Add(currentTime.ToString(@"hh\:mm\:ss"));
                    currentTime = currentTime.Add(TimeSpan.FromHours(1)); // 1-hour slots
                }
            }

            return timeSlots;
        }
    }
}
