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

		public bool getRestricted(Instance currentInstance, Poll currentPoll) {
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
		}

        public bool getUserRestricted(Pointer pointer) {
            if (pointer.LastRequests[2] - pointer.LastRequests[0] > TimeSpan.FromMinutes(1))
               return true;
            return false;
        }

        public UpdateAvailabilityList getInstanceAvailableUpdates(Instance instance) {
            return getListFromLastUpdatesList(instance.lastUpdates, 30, 25, TimeSpan.FromSeconds(1));
        }

        public UpdateAvailabilityList getInlineMessageAvailableUpdates(string inlineMessageID, Poll poll) {
            return getListFromLastUpdatesList(
                poll.MessageIds.Find(x => x.inlineMessageId == inlineMessageID).last30Updates,
                20,
                15, 
                TimeSpan.FromMinutes(1));
        }

        public UpdateAvailabilityList getListFromLastUpdatesList(List<DateTime> datesList, short max, short recommended, TimeSpan cooldown) {
            DateTime startingNow = DateTime.Now;
            UpdateAvailabilityList result = {
                maxUpdates = 0,
                recommendedUpdates = 0,
            };
            for (int i = 0; i < datesList; i++) {
                if (startingNow - datesList[i] > cooldown) {
                    result.maxUpdates = max - Math.Min(max, i);
                    result.recommendedUpdates = recommended - Math.Min(recommended,i);
                    break;
                }
            }
            return result;
        }
	}
}

public class UpdateAvailabilityList {
    short maxUpdates;
    short recommendedUpdates;
}
