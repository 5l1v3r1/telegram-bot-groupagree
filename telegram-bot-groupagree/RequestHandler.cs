﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WJClubBotFrame.Types;

/*
    polls to update
    instance

    user (pointer)
    ..`.?
     */

namespace telegrambotgroupagree {
	public class RequestHandler {
		/*
			Limits for the telegram api, -1 means message won't be accepted because it is too long 
		*/
		public static int[] requestsPerMinute = { 20, 4, -1};
		public static int[] requestThresholds = { 512, 4096};
		
		//instance, user, poll
		private List<Poll> pollsToUpdateWhenBored;
        private List<Poll> pollsToUpdate; //TODO refine

		/*public bool getRestricted(Instance currentInstance, Poll currentPoll) {
			//TODO Make this work
			return false;
		}

		public bool getRestricted(Instance currentInstance, User currentUser) {
			return false;
		}

		public bool getToThrottle(Instance currentInstance, Poll currentPoll) {
			//TODO Make this work
			return false;
		}

		public bool getToThrottle(Instance currentInstance, User currentUser) {
			return false;
		}*/

		//If user pressed more than 3 inline buttons in the last minute
        public bool getUserRestricted(Pointer pointer) {
			//TODO Handle empty lastrequests
            if (pointer.LastRequests[2] - pointer.LastRequests[0] > TimeSpan.FromMinutes(1)) {
				return true;
			}
			return false;
        }

        public UpdateAvailabilityList GetInstanceAvailableUpdates(Instance instance) {
			if (instance.retryAt >= DateTime.Now) {
				return UpdateAvailabilityList.GetNoUpdatesLeft();
			}
            return GetListFromLastUpdatesList(datesList:instance.last30Updates, max: 30, recommended: 25, cooldown:TimeSpan.FromSeconds(1));
        }

		public UpdateAvailabilityList GetInlineMessageAvailableUpdates(string inlineMessageID, Poll poll) 
			=> GetListFromLastUpdatesList(
				poll.MessageIds.Find(x => x.inlineMessageId == inlineMessageID).last30Updates,
				max: 20,
				recommended: 15,
				cooldown:TimeSpan.FromMinutes(1));

		public UpdateAvailabilityList GetListFromLastUpdatesList(List<DateTime> datesList, int max, int recommended, TimeSpan cooldown) {
            DateTime startingNow = DateTime.Now;
			UpdateAvailabilityList result = UpdateAvailabilityList.GetNoUpdatesLeft();
            for (int i = 0; i < datesList.Count; i++) {
                if (startingNow - datesList[i] > cooldown) {
                    result.maxUpdates = max - (Math.Min(max, i));
                    result.recommendedUpdates = recommended - Math.Min(recommended,i);
                    break;
                }
            }
            return result;
        }
	}
}

public class UpdateAvailabilityList {
    public int maxUpdates;
    public int recommendedUpdates;

	public static UpdateAvailabilityList GetNoUpdatesLeft() => new UpdateAvailabilityList {
		maxUpdates = 0,
		recommendedUpdates = 0,
	};
}
