using FluentValidation;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddScoped<ILeaveRequestService, LeaveRequestService>();
        services.AddScoped<IApprovalService, ApprovalService>();

        return services;
    }
}
