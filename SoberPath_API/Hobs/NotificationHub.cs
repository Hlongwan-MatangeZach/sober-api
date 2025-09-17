using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using SoberPath_API.Controllers;

namespace SoberPath_API.Hobs
{
    public class NotificationHub(Sober_Context context):Hub
    {
        private readonly Sober_Context _context=context;

        public async Task Send_Application(Application app)
        {
            
            _context.Applications.Add(app);  //Add application to database
            await _context.SaveChangesAsync();
            

            await Clients.All.SendAsync("sw_application_verification", app);
        }


        public async Task Get_Unread()
        {
            var applications = await _context.Applications.Where(app => app.IsRead == false && app.ClientId != null && app.Social_WorkerId != null).Select(app => new
            {
                id = app.Id,
                name = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Name).FirstOrDefault(),
                surname = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Surname).FirstOrDefault(),
                status = app.Status,
                sw_name = _context.Social_Workers.Where(sw => sw.Id == app.Social_WorkerId).Select(sw => sw.Name).FirstOrDefault(),
                sw_suname = _context.Social_Workers.Where(sw => sw.Id == app.Social_WorkerId).Select(sw => sw.Surname).FirstOrDefault(),
                date = app.Date,
                client_id = app.ClientId,
            }).ToListAsync();   //get unread applications


            await Clients.All.SendAsync("rehab_application_notification", applications);

        }

        public async Task SetAs_Read(int id)
        {
            var obj_application = await _context.Applications.Where(app => app.Id ==id).FirstOrDefaultAsync();
            if(obj_application!=null)
            {
                obj_application.IsRead = true;
            }


            await Clients.Caller.SendAsync("success", "Success message");
        }

    }
}
