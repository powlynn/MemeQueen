using EmailEngine;
using Repository.DataModels;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace MemeWorker
{
    public class ImgurWorker
    {
        private EmailService emailService;

        private IEnumerable<User> queuedUsers;

        private UserService userService;

        public ImgurWorker()
        {
            userService = new UserService();
            emailService = new EmailService();

            queuedUsers = new List<User>();

            //queuedUsers = userService.GetAllUsers();
        }

        public void GetMemes()
        {
            while (true)
            {
                // if any users are queued, send their memes
                foreach (var user in queuedUsers)
                {
                    if (user.UseEmailAddress)
                    {
                        //get memes
                        var memes = GetImageUrls("https://imgur.com/r/memes/top/day", 15);

                        //get customMessage
                        var messages = userService.GetCustomMessageForUser(user.Id, DateTime.Now).Select(x => x.MessageText).ToList();

                        //send memes
                        emailService.SendEmailWithImagesInBody(
                            user.EmailAddress,
                            string.Format("{0}'s Memes - {1}", user.FirstName, DateTime.Now.DayOfWeek),
                            memes,
                            messages
                        );
                    }
                }

                // clear queue
                queuedUsers = new List<User>();

                // get all users with MemeTime 1 minute from now, store in queuedUsers
                queuedUsers = this.userService.GetUsersWithMemeTimeOfNextMinute();

                // wait until next minute
                var now = DateTime.Now;
                var nextMinute = now.AddMinutes(1);
                var durationUntilNextMinute = nextMinute.AddSeconds(nextMinute.Second * -1) - now;

                System.Threading.Thread.Sleep(Convert.ToInt32(durationUntilNextMinute.TotalMilliseconds));
            }
        }

        public List<string> GetImageUrls(string baseUrl, int amount)
        {
            var memeUrls = new List<string>();

            string html = "";

            // download HTML
            using (WebClient client = new WebClient())
            {
                html = client.DownloadString(baseUrl);
            }

            var imgTag = "<a class=\"image-list-link\"";

            var i = 0;

            while (i < amount)
            {
                // go to img
                var img = html.Substring(html.IndexOf(imgTag));

                // go to img src
                var src = img.Substring(img.IndexOf(" href=\"") + 7);

                // go to url
                var url = src.Substring(0, src.IndexOf('\"'));

                // get suffix of link
                var suffix = url.Substring(url.IndexOf("memes/") + 5);

                // point to actual photo
                var imgUrl = "https://i.imgur.com/" + suffix + ".jpg";

                memeUrls.Add(imgUrl);

                html = html.Substring(html.IndexOf(url));
                
                i++;
            }

            return memeUrls;
        }
    }
}
