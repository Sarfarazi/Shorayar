using Council.Core.Entities;
using Council.Core.Enums;
using Council.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Council.Service.DBServices
{
    public class SessionServices : CRUDServices<MeetingHeader>, IMeetingHeader
    {
        LetterServices letterService = new LetterServices();
        MeetingServices meetingService = new MeetingServices();
        UniqueNumberServices UniqueNumberService = new UniqueNumberServices();

        public void DeActiveAllSession()
        {
            var sessions = All().Where(m => m.IsActive).ToList();
            foreach(var item in sessions)
            {
                if (item.IsActive)
                {
                    RemoveLettersToVotingStatus(item.MeetingNumber);
                    item.IsActive = false;
                    item.SessionStatus = Core.Enums.SessionStatus.Dispose;
                }
            }
            Save();
        }
        private void RemoveLettersToVotingStatus(int sessionNumber) {
            var meetings = meetingService.All().Where(m => m.MeetingNumber == sessionNumber.ToString()).Where(m => !m.MeetingUsers.Any()).ToList();
            foreach(var meeting in meetings)
            {
                var letter = letterService.All().Where(m => m.Meeting.ID == meeting.ID).FirstOrDefault();
                letterService.RemoveMeetingFromLetter(letter.ID);
                letterService.ChangLetterStatus(letter.ID, LetterStatus.OutOfVotting);

            }
            //letter.Meeting = null;
            //Save();
            //RemoveMeetingFromLetter(letterID);
            //ChangLetterStatus(letterID, LetterStatus.OutOfCommisionVotting);
        }
        public MeetingHeader GetActiveSession()
        {
            return All().Where(m => m.IsActive).OrderByDescending(m=>m.CreatedOn).FirstOrDefault(); 
        }

        public string GetUniqueNumberForSession(string CouncilPeriodsID)
        {

             string lastTodayUniqueCode = GetLastUniqueCode(CouncilPeriodsID);
            string code = UniqueNumberService.SetCodeFourDigit(int.Parse(lastTodayUniqueCode), 4);
            return  code;
        }

        private string GetLastUniqueCode(string CouncilPeriodsID)
        {
            MeetingHeader uniqueNumber = this.All().Where(u => u.CouncilPeriodsID == CouncilPeriodsID ).OrderByDescending(u => u.CreatedOn).FirstOrDefault();
            if (uniqueNumber == null)
                return "0";

            return uniqueNumber.Code;
        }
    }
}
