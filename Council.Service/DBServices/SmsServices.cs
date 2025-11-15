using Council.Core.Entities;
using Council.Core.Enums;
using Council.Core.Interfaces;
using Council.Core.Statics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Service.DBServices
{
    public class SmsServices : CRUDServices<SMSDeliversLog>, ISMSDeliversLog
    {

        int val = 1;
        public async Task SendSMS()
        {
            SmsErrorCodeServices smsErrorCodeService = new SmsErrorCodeServices();
            SmsServices smsDeliverService = new SmsServices();
            SessionServices sessionService = new SessionServices();
            UserServices userService = new UserServices();
            SmsTokenService smsTokenService = new SmsTokenService();
            SMSService.services_sms sendSmsService = new SMSService.services_sms();
            //SMSService.SendSMS sendSmsService = new SMSService.SendSMS();

            SMSErrorCode smsErrorCode = new SMSErrorCode();
            SMSToken smsToken = new SMSToken();

            var session = sessionService.GetActiveSession();

            smsToken = smsTokenService.All().OrderByDescending(m => m.CreatedOn).FirstOrDefault();
            var inviteMessage = string.Format("با سلام ، از شما دعوت می شود در جلسه به شماره {0} که در تاریخ {1} و از ساعت {2}  تا ساعت {3} برگزار می شود شرکت کنید ، قبلا دستور جلسه در سیستم برای شما ارسال شده است . با تشکر شورای اسلامی شهر {4}  ", session.MeetingNumber, session.RegisterDate, session.StartTime, session.EndTime, Settings.CityName);
            var allUser = userService.GetActiveUsers().Where(m => m.IsCouncilMember && m.IsActive && !m.Deleted).ToList();
            //string[] mobiles = new string[allUser.Count];
            //string[] messages = new string[allUser.Count];
            //int count = allUser.Count();
            long DeliverCode;

            try
            {
                for (int i = 0; i <= allUser.Count - 1; i++)
                {
                    //mobiles[i] = allUser[i].Mobile;
                    //messages[i] = inviteMessage;

                    DeliverCode = await Task.Run(() => sendSmsService.singleSMS(smsToken.PortalCode, smsToken.UserName, smsToken.PassWord, allUser[i].Mobile , inviteMessage , smsToken.ServerType));

                    SMSDeliversLog smsDeliverLog = new SMSDeliversLog();
                    smsDeliverLog.SMSErrorCode = new SMSErrorCode();
                    smsDeliverLog.User = new User();

                    SMSErrorCode error = DeliverCode > 200 ? smsErrorCodeService.All().Where(m => m.ErrorCode == 200).FirstOrDefault() : smsErrorCodeService.All().Where(m => m.ErrorCode == DeliverCode).FirstOrDefault();
                    smsDeliverLog.SMSErrorCode = error;
                    smsDeliverLog.User = allUser[i];
                    smsDeliverLog.Content = inviteMessage;
                    smsDeliverLog.Title = "";
                    smsDeliverLog.SendDate = Convert.ToString(DateTime.Today);
                    smsDeliverService.Create(smsDeliverLog);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
