using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RaaiVan.Modules.FormGenerator;
using RaaiVan.Modules.GlobalUtilities;

namespace RaaiVan.Web.API
{
    public class APOMaturityAssessment
    {
        private static string FormName = "apo_maturity_assessment";

        private static Guid get_poll_app_id(Guid applicationId) {
            Guid baseId = Guid.Parse("2358C791-CC7E-402B-AF31-AF26E84500F8");
            return PublicMethods.guid_xor(baseId, applicationId);
        }

        private static string PollName
        {
            get { return FormName.Replace("_", " "); }
        }

        private static Poll get_poll(Guid applicationId, Guid currentUserId)
        {
            FormType form = FGController.get_form(applicationId, FormName);
            
            if (form == null)
            {
                //create form ******************************************************************************
            }

            if (form == null || !form.FormID.HasValue) return null;

            List<Poll> pollsList = FGController.get_polls_by_form_id(applicationId, formId: form.FormID.Value, archive: null);

            if (pollsList != null && pollsList.Count > 1)
                return null; //there must be exactly one poll using the form
            else if (pollsList != null && pollsList.Count == 1)
            {
                Poll poll = pollsList.FirstOrDefault();

                if (poll.Archived.HasValue && poll.Archived.Value)
                    FGController.recycle_poll(applicationId, poll.PollID.Value, currentUserId);

                return poll;
            }
            else
            {
                Poll poll = new Poll()
                {
                    PollID = Guid.NewGuid(),
                    OwnerID = get_poll_app_id(applicationId),
                    Name = PollName
                };

                bool result = FGController.add_poll(applicationId, pollId: poll.PollID.Value, copyFromPollId: null,
                    ownerId: poll.OwnerID, name: poll.Name, currentUserId: currentUserId);

                return result ? poll : null;
            }
        }

        private static List<Poll> get_poll_instances(Guid applicationId, Guid pollId, bool? archive)
        {
            Guid pollAppId = get_poll_app_id(applicationId);

            long totalCount = 0;

            return FGController.get_polls(applicationId, isCopyOfPollId: pollId, ownerId: pollAppId, 
                archive: archive, searchText: null, count: 1000, lowerBoundary: 1000, totalCount: ref totalCount);
        }

        public static string get_statistics(Guid? applicationId, Guid? currentUserId)
        {
            if (!applicationId.HasValue || !currentUserId.HasValue)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            Poll pollTemplate = get_poll(applicationId.Value, currentUserId.Value);

            if (pollTemplate == null || !pollTemplate.PollID.HasValue) return "{}";

            List<Poll> polls = get_poll_instances(applicationId.Value, pollTemplate.PollID.Value, archive: false);
            
            return "{\"Polls\":[" + string.Join(",", polls.Select(p => {
                FormStatistics stats = FGController.get_form_statistics(applicationId.Value, ownerId: p.PollID, instanceId: null);

                return "{\"Poll\":" + p.toJson() + ",\"Statistics\":" + (stats == null ? "{}" : stats.toJson()) + "}";
            })) + "]}";
        }

        public static string add(Guid? applicationId, int period, Guid? currentUserId)
        {
            if (!applicationId.HasValue || !currentUserId.HasValue || period < 1000)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            Poll poll = get_poll(applicationId.Value, currentUserId.Value);

            if (poll == null || !poll.PollID.HasValue)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            List<Poll> currentInstances = get_poll_instances(applicationId.Value, poll.PollID.Value, archive: null);

            Poll existingInstance = currentInstances == null ? null : currentInstances
                .Where(i => !string.IsNullOrEmpty(i.Description) && i.Description.Trim() == period.ToString()).FirstOrDefault();

            bool result = false;
            bool alreadyExists = false;
            Guid newPollId = Guid.NewGuid();
            FormStatistics stats = null;

            if (existingInstance != null && existingInstance.PollID.HasValue)
            {
                newPollId = existingInstance.PollID.Value;

                alreadyExists = true;

                result = existingInstance.Archived.HasValue && existingInstance.Archived.Value ?
                    FGController.recycle_poll(applicationId.Value, newPollId, currentUserId.Value) : true;

                if (result) stats = FGController.get_form_statistics(applicationId.Value, ownerId: newPollId, instanceId: null);
            }
            else result = FGController.add_poll(applicationId.Value, pollId: newPollId, copyFromPollId: poll.PollID,
                ownerId: get_poll_app_id(applicationId.Value), name: PollName, currentUserId: currentUserId.Value);

            if (result) {
                FGController.set_poll_description(applicationId.Value, newPollId, description: period.ToString(), currentUserId.Value);
                FGController.set_poll_begin_date(applicationId.Value, newPollId, new DateTime(2000, 1, 1), currentUserId.Value);
                FGController.set_poll_hide_contributors(applicationId.Value, newPollId, hideContributors: true, currentUserId.Value);
                FGController.set_poll_show_summary(applicationId.Value, newPollId, showSummary: false, currentUserId.Value);
            }

            return !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"" +
                ",\"PollID\":\"" + newPollId.ToString() + "\"" +
                (alreadyExists ? ",\"AlreadyExists\":" + true.ToString().ToLower() : string.Empty) +
                (stats != null ? ",\"Statistics\":" + stats.toJson() : string.Empty) +
                "}";
        }

        public static string edit(Guid? applicationId, Guid? pollId, int period, Guid? currentUserId)
        {
            if (!applicationId.HasValue || !currentUserId.HasValue || !pollId.HasValue || period < 1000)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            bool result = FGController.set_poll_description(applicationId.Value, 
                pollId.Value, description: period.ToString(), currentUserId.Value);

            return result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";
        }

        public static string remove(Guid? applicationId, Guid? pollId, Guid? currentUserId)
        {
            if (!applicationId.HasValue || !currentUserId.HasValue || !pollId.HasValue)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            bool result = FGController.remove_poll(applicationId.Value, pollId.Value, currentUserId.Value);

            return result ? "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"}" :
                "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";
        }

        public static string get_form(Guid? applicationId, Guid? pollId, Guid? currentUserId)
        {
            if (!applicationId.HasValue || !currentUserId.HasValue || !pollId.HasValue)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            Poll poll = get_poll(applicationId.Value, currentUserId.Value);

            if (poll == null || !poll.PollID.HasValue)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            Guid? instanceId = FGController.get_poll_instance(applicationId.Value, pollId.Value,
                copyFromPollId: poll.PollID.Value, ownerId: get_poll_app_id(applicationId.Value), currentUserId.Value);

            List<FormElement> elements = !instanceId.HasValue ? new List<FormElement>() :
                FGController.get_form_instance_elements(applicationId.Value, instanceId.Value);

            return "{\"InstanceID\":\"" + (!instanceId.HasValue ? string.Empty : instanceId.ToString()) + "\"" + 
                ",\"Elements\":[" + string.Join(",", elements.Select(e => e.toJson(applicationId.Value))) + "]}";
        }

        public static string save_form(Guid? applicationId, Guid? pollId, List<FormElement> elements, Guid? currentUserId)
        {
            if (!applicationId.HasValue || !currentUserId.HasValue || !pollId.HasValue || 
                elements == null || elements.Count == 0)
                return "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}";

            string errorMessage = string.Empty;

            bool result = FGController.save_form_instance_elements(applicationId.Value,
                elements, elementsToClear: new List<Guid>(), currentUserId.Value, ref errorMessage);

            FormStatistics stats = !result ? null :
                FGController.get_form_statistics(applicationId.Value, ownerId: pollId.Value, instanceId: null);

            return !result ? "{\"ErrorText\":\"" + Messages.OperationFailed.ToString() + "\"}" :
                "{\"Succeed\":\"" + Messages.OperationCompletedSuccessfully.ToString() + "\"" +
                (stats != null ? ",\"Statistics\":" + stats.toJson() : string.Empty) +
                "}";
        }
    }
}