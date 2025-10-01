using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SoberPath_API.Context;
using SoberPath_API.Models;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SoberPath_API.Hobs
{
    public class SW_NotificationHub(Sober_Context context) : Hub
    {
        private readonly Sober_Context _context = context;


        public async Task Get_StatusUpdates(int id)
        {

            var statusupdates = await _context.Applications
                .Where(app => app.Social_WorkerId == id && app.Status_Update_Date != null && app.IsRead == false && app.Status != "Pending")
                .Select(app => new
                {
                    Id = app.Id,
                    type = "status_change",
                    title = "Application Status Updated",
                    message = "Application for the client has changed, see new status update",
                    clientName = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Name).FirstOrDefault(),
                    clientSurname = _context.Clients.Where(cl => cl.Id == app.ClientId).Select(cl => cl.Surname).FirstOrDefault(),
                    clientId = app.ClientId,
                    applicationId = app.Id,
                    priority = "urgent",
                    timestamp = app.Status_Update_Date,
                    isRead = app.IsRead,
                }).ToListAsync();


            var assigedclients = await _context.Clients
                .Where(cl => cl.Social_WorkerId == id && cl.IsRead == false)
                .Select(client => new
                {
                    Id = client.Id,
                    type = "assigned_client",
                    title = "New Client Assigned",
                    message = "You have a new client assigned to you",
                    clientName = client.Name,
                    clientSurname = client.Surname,
                    clientId = client.Id,
                    applicationId = (int?)null,
                    priority = "high",
                    timestamp = (string?)null,
                    isRead = client.IsRead,
                }).ToListAsync();

            // Get recent events for the social worker
            var eventNotifications = await _context.Events
                .Where(e => e.Social_Id == id && e.Date != null && e.IsRead == false) // Filter by social worker ID
                .OrderByDescending(e => e.Date)
                .Take(20) // Limit to recent 20 events
                .Select(e => new
                {
                    Id = e.Id, // Offset to avoid ID conflicts with applications
                    type = "new_event",
                    title = "Calendar Event",
                    message = $"Event '{e.Title}' - {e.Description}",
                    clientName = e.Client_Id != null ?
                        _context.Clients.Where(c => c.Id == e.Client_Id).Select(c => c.Name).FirstOrDefault() : null,
                    clientSurname = e.Client_Id != null ?
                        _context.Clients.Where(c => c.Id == e.Client_Id).Select(c => c.Surname).FirstOrDefault() : null,
                    clientId = e.Client_Id,
                    applicationId = (int?)null,
                    eventId = e.Id,
                    priority = "medium",
                    timestamp = e.Date, // Use the event date as timestamp
                    isRead = e.IsRead,
                    metadata = new
                    {
                        eventType = "calendar_event",
                        startTime = e.StartTime,
                        endTime = e.EndTime,
                        description = e.Description
                    }
                }).ToListAsync();

            var allNotifications = eventNotifications.Cast<object>().Concat(statusupdates.Cast<object>()).Concat(assigedclients.Cast<object>()).OrderByDescending(n => ((dynamic)n).timestamp).ToList();


            await Clients.Caller.SendAsync("StatusUpdates", allNotifications);
        }


        public async Task Add_Event(Event new_event)
        {
            _context.Events.Add(new_event);
            await _context.SaveChangesAsync();

            // Get client name if event is associated with a client
            string clientName = null;
            if (new_event.Client_Id.HasValue)
            {
                clientName = await _context.Clients
                    .Where(c => c.Id == new_event.Client_Id.Value)
                    .Select(c => c.Name)
                    .FirstOrDefaultAsync();
            }

            // Create notification for the new event
            var notification = new
            {
                Id = new_event.Id,
                type = "new_event",
                title = "New Calendar Event",
                message = $"Event '{new_event.Title}' - {new_event.Description}",
                clientName = clientName,
                clientId = new_event.Client_Id,
                applicationId = (int?)null,
                eventId = new_event.Id,
                priority = "medium",
                timestamp = new_event.Date ?? DateTime.Now.ToString("yyyy-MM-dd"),
                isRead = false,
                metadata = new
                {
                    eventType = "calendar_event",
                    startTime = new_event.StartTime,
                    endTime = new_event.EndTime,
                    description = new_event.Description
                }
            };

            // Send to the specific social worker who owns the event
            if (new_event.Social_Id.HasValue)
            {
                await Clients.User(new_event.Social_Id.Value.ToString()).SendAsync("NewEventAdded", notification);
            }

            // Also send to all connected clients for real-time updates
            await Clients.All.SendAsync("NewEventAdded", notification);
        }

        public async Task MarkAsRead(int notificationId, string notificationType)
        {
            if (notificationType == "status_change")
            {
                // Handle application notification read status
                var application = await _context.Applications.FindAsync(notificationId);
                if (application != null)
                {
                    application.IsRead = true;
                    await _context.SaveChangesAsync();
                }
            }
            else if (notificationType == "new_event")
            {
                // Handle event notification read status
                var eventNotification = await _context.Events.FindAsync(notificationId);
                if (eventNotification != null)
                {
                    eventNotification.IsRead = true;
                    await _context.SaveChangesAsync();
                }
            }
            else if (notificationType == "assigned_client")
            {
                var client = await _context.Clients.FindAsync(notificationId);
                if (client != null)
                {
                    client.IsRead = true;
                    await _context.SaveChangesAsync();
                }
            }
            // Event notifications don't need persistent read status

            await Clients.Caller.SendAsync("MarkedAsRead", notificationId);
        }

        // Method to get only event notifications
        public async Task Get_EventNotifications(int socialWorkerId)
        {
            var eventNotifications = await _context.Events
                .Where(e => e.Social_Id == socialWorkerId && e.Date != null)
                .OrderByDescending(e => e.Date)
                .Select(e => new
                {
                    Id = e.Id + 1000000,
                    type = "new_event",
                    title = "Calendar Event",
                    message = $"Event '{e.Title}' - {e.Description}",
                    clientName = e.Client_Id != null ?
                        _context.Clients.Where(c => c.Id == e.Client_Id).Select(c => c.Name).FirstOrDefault() : null,
                    clientId = e.Client_Id,
                    applicationId = (int?)null,
                    eventId = e.Id,
                    priority = "medium",
                    timestamp = e.Date,
                    isRead = false,
                    metadata = new
                    {
                        eventType = "calendar_event",
                        startTime = e.StartTime,
                        endTime = e.EndTime
                    }
                }).ToListAsync();

            await Clients.Caller.SendAsync("EventNotifications", eventNotifications);
        }
    }
}