using SummitGrid.Core.Entities;
using SummitGrid.Core.Models;
using SummitGrid.Core.Results;

namespace SummitGrid.Infrastructure.Services;

public class IncidentService
{
    private readonly SummitGridDbContext _context;

    public IncidentService(SummitGridDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateIncidentAsync(CreateIncidentCommand incident)
    {
        Incident _incident = new Incident
        {
            Title = incident.Title,
            Description = incident.Description,
            IncidentType = incident.IncidentType,
            IncidentStatus = IncidentStatus.Reported,
            ClimbingAreaId = incident.ClimbingAreaId,
            RouteId = incident.RouteId,
            ReportedBy = incident.UserName,
            AssignedTo = incident.AssignedTo
        };

        _context.Incidents.Add(_incident);
        await _context.SaveChangesAsync();

        AuditLog Log = new AuditLog
        {
            UserName = incident.UserName,
            AuditAction = AuditAction.Created,
            IncidentId = _incident.Id,
            OldValue = 0,
            NewValue = 0
        };
        _context.AuditLogs.Add(Log);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<UpdateIncidentResult> UpdateIncidentStatusAsync(int incidentId, IncidentStatus status, string UserName)
    {
        var incident = await _context.Incidents.FindAsync(incidentId);
        if (incident == null) return UpdateIncidentResult.IncidentNotFound;

        if(!IsValidTransition(incident.IncidentStatus, status))
            return UpdateIncidentResult.InvalidTransition;
        
        AuditLog Log = new AuditLog
        {
            UserName = UserName,
            AuditAction = AuditAction.StatusUpdated,
            IncidentId = incidentId,
            OldValue = (int)incident.IncidentStatus,
            NewValue = (int)status
        };
        _context.AuditLogs.Add(Log);

        incident.IncidentStatus = status;

        var climbingArea = await _context.ClimbingAreas.FindAsync(incident.ClimbingAreaId);
        if (climbingArea == null) return UpdateIncidentResult.IncidentNotFound;
        
        if(status == IncidentStatus.Acknowledged)
            climbingArea.Status = AreaStatus.Restricted;

        else if(status == IncidentStatus.InProgress)
            climbingArea.Status = AreaStatus.Closed;

        else if(status == IncidentStatus.Resolved)
            climbingArea.Status = AreaStatus.Open;

        var route = await _context.Routes.FindAsync(incident.RouteId);
        if(route != null)
        {
            if(status == IncidentStatus.Acknowledged)
                route.Status = RouteStatus.Restricted;

            else if(status == IncidentStatus.InProgress)
                route.Status = RouteStatus.Closed;

            else if(status == IncidentStatus.Resolved)
                route.Status = RouteStatus.Open;
        }

        incident.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return UpdateIncidentResult.Success;
    }

    public bool IsValidTransition(IncidentStatus s1, IncidentStatus s2)
    {
        return (int)s2 == (int)s1 + 1;
    }

}