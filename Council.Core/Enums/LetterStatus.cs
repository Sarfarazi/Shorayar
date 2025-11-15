using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Council.Core.Enums
{
    public enum LetterStatus
    {
        NormalLetter = 0,
        SendForBoss = 1, //ارسال برای رئیس   شورا شده  
        SendInvite=13 , //صدور دعوتنامه شده باشد      
        Votting = 3, // تنظیم صورت جلسه شده
        ReadyForVoting = 4, // صدور اجازه رای گیری
        AllowVotting = 5, // رای گیری صورت گرفته
        End = 6, // اتمام رای گیری
        OutOfVotting=11, //خروج از دستور جلسه

        SentToCommision = 2,   // ارسال به رئیس   کمیسیون شده
        CommissionVoting = 7,  // تنظیم صورت جلسه شده 
        CommissionReadyForVoting = 8, // صدور اجازه رای گیری
        CommissionAllowVotting = 9, // رای گیری صورت گرفته
        CommissionEnd = 10,// اتمام رای گیری
        OutOfCommisionVotting = 12, //خروج از دستور جلسه کمیسیون

        SendToMember = 20,
       
    }
    public enum OutLetterStatus
    { 
        Sended=0,              
        Plan=2,    
        Recieved=1,        
        Force1=4, //تک فوریتی
        Force2= 5, //دو فوریتی
        Force3= 6,  //سه فوریتی 
        WaitForComfirm = 7, //  انتظار برای تایید نامه ارسالی
        ForEdit = 8, // بازگشت به دبیرخانه برای اصلاح
        Comfirmed = 9, // تایید شده توسط امضا کننده
        NoData = 200 ,  //مقدار null    
    }
    public enum SessionStatus
    {
        Created=0,
        SendInviteLetter=1,
        Dispose=2
    }    
}
