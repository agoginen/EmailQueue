using EmailQueue.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace EmailQueue
{
    public class myfunctions : Ienqueue
    {
        protected DiversityTraxEntities db;

        public   Queue<emailQueue> enqueue()
        {
            db = new DiversityTraxEntities();
 
            var data = db.emailQueues.Where(x => x.EStatus == "Not Posted").ToList();
            //var data = db.emailQueues.Where(x => x.EStatus == "Not Posted").Take(30).ToList();

            data.Sort();

            Queue<emailQueue> orderedRequests = new Queue<emailQueue>();

            for (int i = 0; i < data.Count; i++)
            { 
                orderedRequests.Enqueue(data[i]);
            }

            return orderedRequests;
        }

        public int dequeue(Queue<emailQueue> data)
        {
            db = new DiversityTraxEntities();

            Queue<emailQueue> failedMails = new Queue<emailQueue>();

            Queue<emailQueue> sentMails = new Queue<emailQueue>();

            int successMessageCount = 0;

            int failMessageCount = 0;

            int totalRequests = data.Count;

            emailQueueSuccessfulLogs updateSuccessLog = new emailQueueSuccessfulLogs();

            emailQueueFailedLog updatefailedLog = new emailQueueFailedLog();

            for (int i = 0; i < totalRequests; i++)
            {
                emailQueue emailDetails = (emailQueue)data.Dequeue();

                MailMessage mail = new MailMessage();
                mail.To.Add(emailDetails.ETo);
                mail.Subject = emailDetails.ESubject;
                mail.Body = emailDetails.EBody;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.EnableSsl = true;

                try
                {
                    smtp.Send(mail);

                    emailDetails.Tries = (int)emailDetails.Tries + 1;

                    db.Entry(emailDetails).State = EntityState.Modified;

                    db.SaveChanges();

                    sentMails.Enqueue(emailDetails);
                }
                catch
                {
                    emailDetails.Tries =  (int)emailDetails.Tries + 1;

                    db.Entry(emailDetails).State = EntityState.Modified;

                    db.SaveChanges();
                    
                    failedMails.Enqueue(emailDetails);
                }
            }

            successMessageCount = sentMails.Count;

            failMessageCount = failedMails.Count;

            for (int i = 0; i < successMessageCount; i++)
            {
                emailQueue currMail = (emailQueue)sentMails.Dequeue();

                currMail.EStatus = "Posted";

                updateSuccessLog.ETo = currMail.ETo;

                updateSuccessLog.EDate = currMail.EDate;

                updateSuccessLog.ETime = currMail.ETime;

                updateSuccessLog.Tries = currMail.Tries;

                db.Entry(currMail).State = EntityState.Modified;

                db.SaveChanges();

                db.emailQueueSuccessfulLogs.Add(updateSuccessLog);

                db.SaveChanges();

            }

            for (int i = 0; i < failMessageCount; i++)
            {
                emailQueue currMail = (emailQueue)failedMails.Dequeue();

                updatefailedLog.ETo = currMail.ETo;

                updatefailedLog.EDate = currMail.EDate;

                updatefailedLog.ETime = currMail.ETime;

                updatefailedLog.Tries = currMail.Tries;

                db.emailQueueFailedLog.Add(updatefailedLog);

                db.SaveChanges();
            }

            return successMessageCount;
        }
    }
}