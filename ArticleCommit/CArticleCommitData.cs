using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SManApi.ArticleCommit
{
    public class CArticleCommitData
    {
        public string orderNumber
        { get; set; }

        public Decimal quantity
        { get; set; }
        public string articleNumber
        { get; set; }

        public int orderArtID // Identity of the order art checkout
        { get; set; }

    }
}