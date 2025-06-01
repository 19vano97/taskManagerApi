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

    public static class DefaultParametersForUsers
    {
        public static class ProjectLimitations
        {
            public const int MAX_STATUSES_VALUE = 10;
        }
    }

    public static class LogPhrases
    {
        public static class PositiveActions
        {
            public const string TASK_CREATED_LOG = "Task created: {object}";
            public const string TASKS_SHOWN_LOG = "Tasks have shown: {object}";
            public const string TASK_UPDATED_LOG = "Task updated: {object}";
            public const string TASK_DELETED_LOG = "Task deleted: {object}";
            public const string TASK_ASSIGNED_LOG = "Task assigned to user: {object}";
            public const string TASK_STATUS_CHANGED_LOG = "Task status changed: {object}";
            public const string TASK_COMMENT_ADDED_LOG = "Comment added to task: {object}";

            public const string PROJECT_CREATED_LOG = "Project created: {object}";
            public const string PROJECT_UPDATED_LOG = "Project updated: {object}";
            public const string PROJECT_DELETED_LOG = "Project deleted: {object}";
            public const string PROJECT_ARCHIVED_LOG = "Project archived: {object}";
            public const string PROJECT_MEMBER_ADDED_LOG = "User added to project: {object}";
            public const string PROJECT_MEMBER_REMOVED_LOG = "User removed from project: {object}";

            public const string ORGANIZATION_CREATED_LOG = "Organization created: {object}";
            public const string ORGANIZATION_UPDATED_LOG = "Organization updated: {object}";
            public const string ORGANIZATION_DELETED_LOG = "Organization deleted: {object}";
            public const string ORGANIZATION_MEMBER_ADDED_LOG = "User added to organization: {object}";
            public const string ORGANIZATION_MEMBER_REMOVED_LOG = "User removed from organization: {object}";

            public const string USER_ROLE_CHANGED_LOG = "User role changed: {object}";
            public const string USER_INVITED_LOG = "User invited: {object}";
            public const string USER_PROFILE_UPDATED_LOG = "User profile updated: {object}";

            public const string FILE_UPLOADED_LOG = "File uploaded: {object}";
            public const string FILE_DELETED_LOG = "File deleted: {object}";
            public const string FILE_DOWNLOADED_LOG = "File downloaded: {object}";

            public const string NOTIFICATION_SENT_LOG = "Notification sent: {object}";
            public const string COMMENT_EDITED_LOG = "Comment edited: {object}";
            public const string COMMENT_DELETED_LOG = "Comment deleted: {object}";
        }
        public static class NegativeActions
        {
            public const string TASK_CREATION_FAILED_LOG = "Task creation failed: {object}";
            public const string TASK_UPDATE_FAILED_LOG = "Task update failed: {object}";
            public const string TASK_DELETION_FAILED_LOG = "Task deletion failed: {object}";
            public const string TASK_ASSIGNMENT_FAILED_LOG = "Task assignment failed: {object}";
            public const string TASK_STATUS_CHANGE_FAILED_LOG = "Failed to change task status: {object}";
            public const string TASK_NOT_FOUND_LOG = "Task is not found: {object}";

            public const string PROJECT_CREATION_FAILED_LOG = "Project creation failed: {object}";
            public const string PROJECT_UPDATE_FAILED_LOG = "Project update failed: {object}";
            public const string PROJECT_DELETION_FAILED_LOG = "Project deletion failed: {object}";
            public const string PROJECT_MEMBER_ADDITION_FAILED_LOG = "Failed to add user to project: {object}";
            public const string PROJECT_MEMBER_REMOVAL_FAILED_LOG = "Failed to remove user from project: {object}";

            public const string ORGANIZATION_CREATION_FAILED_LOG = "Organization creation failed: {object}";
            public const string ORGANIZATION_UPDATE_FAILED_LOG = "Organization update failed: {object}";
            public const string ORGANIZATION_DELETION_FAILED_LOG = "Organization deletion failed: {object}";
            public const string ORGANIZATION_MEMBER_ADDITION_FAILED_LOG = "Failed to add user to organization: {object}";
            public const string ORGANIZATION_MEMBER_REMOVAL_FAILED_LOG = "Failed to remove user from organization: {object}";

            public const string USER_LOGIN_FAILED_LOG = "User login failed: {object}";
            public const string USER_LOGOUT_FAILED_LOG = "User logout failed: {object}";
            public const string USER_ROLE_CHANGE_FAILED_LOG = "User role change failed: {object}";
            public const string USER_INVITATION_FAILED_LOG = "User invitation failed: {object}";
            public const string USER_PROFILE_UPDATE_FAILED_LOG = "User profile update failed: {object}";

            public const string FILE_UPLOAD_FAILED_LOG = "File upload failed: {object}";
            public const string FILE_DELETE_FAILED_LOG = "File deletion failed: {object}";
            public const string FILE_DOWNLOAD_FAILED_LOG = "File download failed: {object}";

            public const string COMMENT_EDIT_FAILED_LOG = "Comment edit failed: {object}";
            public const string COMMENT_DELETION_FAILED_LOG = "Comment deletion failed: {object}";

            public const string NOTIFICATION_FAILED_LOG = "Notification sending failed: {object}";
        }
        public static class ApiLogs
        {
            public const string API_REQUEST_RECEIVED_LOG = "API request received: {object}";
            public const string API_RESPONSE_SENT_LOG = "API response sent: {object}";
            public const string API_VALIDATION_FAILED_LOG = "API validation failed: {object}";
            public const string API_AUTHENTICATION_FAILED_LOG = "API authentication failed: {object}";
            public const string API_AUTHORIZATION_FAILED_LOG = "API authorization failed: {object}";
            public const string API_RESOURCE_NOT_FOUND_LOG = "API resource not found: {object}";
            public const string API_INTERNAL_SERVER_ERROR_LOG = "API internal server error: {object}";
            public const string API_TIMEOUT_LOG = "API request timed out: {object}";
            public const string API_RATE_LIMIT_EXCEEDED_LOG = "API rate limit exceeded: {object}";
        }
    }

    public static class TaskHistoryTypes
    {
        public static class TaskCreate
        {
            public const string TASK_CREATED = "TASK_CREATED";
        }
        public static class TaskEdit
        {
            public const string TASK_EDITED_TITLE = "TASK_EDITED_TITLE";
            public const string TASK_EDITED_DESCRIPTION = "TASK_EDITED_DESCRIPTION";
            public const string TASK_EDITED_REPORTEDID = "TASK_EDITED_REPORTEDID";
            public const string TASK_EDITED_ASSIGNEEID = "TASK_EDITED_ASSIGNEEID";
            public const string TASK_EDITED_PARENTTASK = "TASK_EDITED_PARENTTASK";
            public const string TASK_EDITED_TASKTYPE = "TASK_EDITED_TASKTYPE";
            public const string TASK_EDITED_STATUS = "TASK_EDITED_STATUS";
            public const string TASK_EDITED_PROJECT = "TASK_EDITED_PROJECT";
        }
        public static class TaskDelete
        {
            public const string TASK_DELETED = "TASK_DELETED";
        }
    }

    public static class ServerSettingsConstants
    {
        public static class ApiServicesConstants
        {
            public const string TASK_HISTORY = "TaskHistory";
            
        }
    }
}
