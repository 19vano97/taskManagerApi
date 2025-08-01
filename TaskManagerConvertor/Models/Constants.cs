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
            public const string GET_TICKETS_IN_ORG = "all/{organizationId}/organization";
            public const string GET_TICKETS_IN_PROJECT = "all/{projectId}/project";
            public const string POST_CREATE_TICKETS_FOR_AI = "create/ai/list";
            public const string POST_EDIT_TICKET = "{taskId}/edit";
            public const string GET_TICKET_HISTORY = "{taskId}/history";
            public const string DELETE_TICKET = "{taskId}/delete";
        }

        public static class Project
        {
            public const string GET_TICKET_BY_ID = "/api/task/{0}/details";
            public const string POST_CREATE_TICKET = "/api/task/create";
            public const string GET_TICKETS_IN_ORG = "all/{organizationId}/organization";
            public const string GET_TICKETS_IN_PROJECT = "all/{projectId}/project";
            public const string POST_CREATE_TICKETS_FOR_AI = "create/ai/list";
            public const string POST_EDIT_TICKET = "{taskId}/edit";
            public const string GET_TICKET_HISTORY = "{taskId}/history";
            public const string DELETE_TICKET = "{taskId}/delete";
        }

        public static class Organization
        {
            public const string GET_TICKET_BY_ID = "/api/task/{0}/details";
            public const string POST_CREATE_TICKET = "/api/task/create";
            public const string GET_TICKETS_IN_ORG = "all/{organizationId}/organization";
            public const string GET_TICKETS_IN_PROJECT = "all/{projectId}/project";
            public const string POST_CREATE_TICKETS_FOR_AI = "create/ai/list";
            public const string POST_EDIT_TICKET = "{taskId}/edit";
            public const string GET_TICKET_HISTORY = "{taskId}/history";
            public const string DELETE_TICKET = "{taskId}/delete";
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
