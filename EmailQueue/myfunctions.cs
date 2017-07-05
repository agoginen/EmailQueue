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
 
            var data = db.emailQueue.Where(x => x.EStatus == "Not Posted").ToList();
            //var data = db.emailQueues.Where(x => x.EStatus == "Not Posted").Take(30).ToList();

            data.Sort();

            Queue<emailQueue> orderedRequests = new Queue<emailQueue>();

            for (int i = 0; i < data.Count; i++)
            { 
                orderedRequests.Enqueue(data[i]);
            }

            return orderedRequests;
        }

        public int[] dequeue(Queue<emailQueue> data)
        {
            var bodySearch = "<div id=\"EBody\">";

            var subjectSearch = "<div id=\"ESubject\">";

            var reasonSearch = "<div id=\"EReason\">";

            var clientName = "<div id=\"EClientName\">";

            db = new DiversityTraxEntities();

            Queue<emailQueue> failedMails = new Queue<emailQueue>();

            Queue<emailQueue> sentMails = new Queue<emailQueue>();

            int[] MessageCount = { 0,0 };
            
            int totalRequests = data.Count;

            emailQueueSuccessfulLogs updateSuccessLog = new emailQueueSuccessfulLogs();

            emailQueueFailedLog updatefailedLog = new emailQueueFailedLog();

            for (int i = 0; i < totalRequests; i++)
            {
                emailQueue emailDetails = (emailQueue)data.Dequeue();

                Templates presentTemplate = new Templates();

                presentTemplate = db.Templates.Find(emailDetails.TableId);

                var modifiedBody = presentTemplate.Template_Content.Insert(presentTemplate.Template_Content.IndexOf(bodySearch) + bodySearch.Length, emailDetails.EBody);

                modifiedBody = modifiedBody.Insert(presentTemplate.Template_Content.IndexOf(subjectSearch) + subjectSearch.Length, emailDetails.ESubject);

                modifiedBody = modifiedBody.Insert(presentTemplate.Template_Content.IndexOf(reasonSearch) + reasonSearch.Length, emailDetails.EReason);

                modifiedBody = modifiedBody.Insert(presentTemplate.Template_Content.IndexOf(clientName) + clientName.Length, emailDetails.EName);

                MailMessage mail = new MailMessage();
                mail.To.Add(emailDetails.ETo);
                mail.Subject = emailDetails.ESubject;
                mail.Body = modifiedBody;
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

            MessageCount[0] = sentMails.Count;

            MessageCount[1] = failedMails.Count;

            for (int i = 0; i < MessageCount[0]; i++)
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

            for (int i = 0; i < MessageCount[1]; i++)
            {
                emailQueue currMail = (emailQueue)failedMails.Dequeue();

                updatefailedLog.ETo = currMail.ETo;

                updatefailedLog.EDate = currMail.EDate;

                updatefailedLog.ETime = currMail.ETime;

                updatefailedLog.Tries = currMail.Tries;

                db.emailQueueFailedLog.Add(updatefailedLog);

                db.SaveChanges();
            }

            return MessageCount;
        }
    }
}