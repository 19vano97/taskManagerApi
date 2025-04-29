using System;
using System.Collections.Immutable;
using TaskManagerApi.Models.TaskItemStatuses;

namespace TaskManagerApi.Models;

public class Constants
{
    public static class IdentityCustomOpenId
    {
        public static class DetailsFromToken
        {
            public const string ACCOUNT_ID = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        }
    }

    public static class Routes
    {
        public static class ProjectRoutes
        {
            public const string PROJECT_API = "project";
            public const string PROJECT_GET_ALL_PROJECTS_BY_ORG = "/all";
            
        }
    }

    public static class StatusesConstants
    {
        public static class DefaultStatuses
        {
            public const int TO_DO = 1;
            public const int IN_PROGRESS = 2;
            public const int DONE = 4;
            public const string TO_DO_NAME = "To Do";
            public const string IN_PROGRESS_NAME = "In Progress";
            public const string DONE_NAME = "Done";
        }
        public static class DefaultStatusTypes
        {
            public const int TO_DO = 2;
            public const int IN_PROGRESS = 3;
            public const int DONE = 4;
            public const string TO_DO_NAME = "To Do";
            public const string IN_PROGRESS_NAME = "In Progress";
            public const string DONE_NAME = "Done";
        }
        public static class DefaultStatusOrder
        {
            public const int TO_DO = 1;
            public const int IN_PROGRESS = 2;
            public const int DONE = 3;
        }
        public static ImmutableList<TaskItemStatusDto> DEFAULT_LIST = ImmutableList.Create(
            new TaskItemStatusDto
            {
                TypeId = DefaultStatusTypes.TO_DO,
                TypeName = DefaultStatusTypes.TO_DO_NAME,
                StatusId = DefaultStatuses.TO_DO,
                StatusName = DefaultStatuses.IN_PROGRESS_NAME,
                Order = DefaultStatusOrder.TO_DO
            },
            new TaskItemStatusDto 
            {
                TypeId = DefaultStatusTypes.IN_PROGRESS,
                TypeName = DefaultStatusTypes.IN_PROGRESS_NAME,
                StatusId = DefaultStatuses.IN_PROGRESS,
                StatusName = DefaultStatuses.IN_PROGRESS_NAME,
                Order = DefaultStatusOrder.IN_PROGRESS
            },
            new TaskItemStatusDto 
            {
                TypeId = DefaultStatusTypes.DONE,
                TypeName = DefaultStatusTypes.DONE_NAME,
                StatusId = DefaultStatuses.DONE,
                StatusName = DefaultStatuses.DONE_NAME,
                Order = DefaultStatusOrder.DONE
            }
        );
    }
}
