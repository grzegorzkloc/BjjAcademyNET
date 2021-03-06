﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BjjAcademy.GlobalMethods
{
    public static class MessagingCenterMessage
    {
        public static readonly string PersonUpdated = "PersonUpdated";
        public static readonly string TrainingPlanAdded = "TrainingPlanAdded";
        public static readonly string FinishedEditingTrainingPlan = "FinishedEditingTrainingPlan";
        public static readonly string AddedBjjEvent = "AddedBjjEvent";
        public static readonly string SingleEventPageCreated = "SingleEventPageCreated";
        public static readonly string PromotionPageCreated = "PromotionPageCreated";
        public static readonly string SentToSingleEventPage = "SentToSingleEventPage";
        public static readonly string SentToPromotionPage = "SentToPromotionPage";
        public static readonly string MultiselectPersonsSent = "MultiselectPersonsSent";
        public static readonly string PromotionListEmpty = "PromotionListEmpty";
        public static readonly string DeletePromotionEvent = "DeletePromotionEvent";
    }
}
