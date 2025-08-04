using System;

namespace TaskManagerConvertor.Models;

public static class Constants
{
    public static class TaskManagerApi
    {
        public static class Ticket
        {
            public const string GET_TICKET_BY_ID = "/api/task/{0}/details";
            public const string POST_CREATE_TICKET = "/api/task/create";
            public const string GET_TICKETS_IN_ORG = "/api/task/all/{organizationId}/organization";
            public const string GET_TICKETS_IN_PROJECT = "/api/task/all/{projectId}/project";
            public const string POST_CREATE_TICKETS_FOR_AI = "/api/task/create/ai/list";
            public const string POST_EDIT_TICKET = "/api/task/{taskId}/edit";
            public const string GET_TICKET_HISTORY = "/api/task/{taskId}/history";
            public const string DELETE_TICKET = "/api/task/{taskId}/delete";
        }

        public static class Project
        {
            public const string GET_PROJECT_BY_ID = "/api/project/{0}/details";
            public const string POST_CREATE_PROJECT = "/api/project/create";
            public const string GET_PROJECT_IN_ORG = "/api/project/all/{organizationId}/organization";
            public const string POST_EDIT_PROJECT = "/api/project/{taskId}/edit";
            public const string DELETE_PROJECT = "/api/project/{taskId}/delete";
        }

        public static class Organization
        {
            public const string GET_ORGANIZATION_BY_ID = "/api/organization/{0}/details";
            public const string GET_MY_ORGANIZATION = "/api/organization/details/me";
            public const string POST_CREATE_ORGANIZATION= "/api/organization/create";
            public const string POST_EDIT_ORGANIZATION = "/api/organization/{taskId}/edit";
            public const string DELETE_ORGANIZATION= "/api/organization/{taskId}/delete";
        }
    }

    public static class IdentityServer
    {
        public static class Api
        {
            public const string GET_OWN_DETAILS = "/api/auth/details";
            public const string GET_MULTIPLE_DETAILS = "/api/auth/details/accounts";
            public const string POST_OWN_DETAILS = "/api/auth/details/";
            public const string POST_INVITE_MEMBER = "/api/auth/invite/";
        }
    }

    public static class Settings
    {
        public static class HttpClientNaming
        {
            public const string TASK_HISTORY_CLIENT = "taskHistory";
            public const string AUTH_CLIENT = "identityServer";
            public const string TASK_MANAGER_CLIENT = "taskManager";
        }

        public static class Header
        {
            public const string ORGANIZATION = "organizationId";
            public const string AUTHORIZATION = "Authorization";
        }
    }
}
