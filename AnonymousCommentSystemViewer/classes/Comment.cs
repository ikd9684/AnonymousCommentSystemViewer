using Codeplex.Data;
using System.Collections.Generic;
using System.Net;

namespace AnonymousCommentSystemViewer.classes
{
    public class Comment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentID"></param>
        /// <param name="threadID"></param>
        /// <param name="comment"></param>
        /// <param name="timestamp"></param>
        public Comment(string commentID, string threadID, string comment, string timestamp)
        {
            this.commentID = commentID;
            this.threadID = threadID;
            this.comment = comment;
            this.timestamp = timestamp;
        }
        /// <summary></summary>
        public string commentID { private set; get; }
        /// <summary></summary>
        public string threadID { private set; get; }
        /// <summary></summary>
        public string comment { private set; get; }
        /// <summary></summary>
        public string timestamp { private set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}: {2}; {3}]", commentID, threadID, comment, timestamp);
        }
    }

    public class CommentsRequest
    {
        /// <summary></summary>
        private static readonly string baseurl;
        /// <summary></summary>
        private static readonly string default_thread_id;

        /// <summary>
        /// 
        /// </summary>
        static CommentsRequest()
        {
            baseurl = Properties.Settings.Default.baseuri;
            default_thread_id = Properties.Settings.Default.default_thread_id;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Comment> getCommentList(string threadID, string last)
        {
            string url = string.Format("{0}{1}?last={2}", baseurl, threadID, last);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Headers["X-requested-with"] = "Application";
            req.Method = "GET";

            HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            dynamic jsonArray = DynamicJson.Parse(res.GetResponseStream());

            List<Comment> commentList = new List<Comment>();
            foreach (dynamic c in jsonArray)
            {
                Comment comment = new Comment(c.comment_id, c.thread_id, c.comment, c.timestamp);
                commentList.Add(comment);
            }
            return commentList;
        }
    }
}
