//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;

//namespace FujiyBlog.Web.Extensions
//{
//    public static class ModelStateDictionaryExtensions
//    {
//        public static void Merge(this ModelStateDictionary instance, IEnumerable<ValidationResult> ruleViolations)
//        {
//            if (instance == null)
//            {
//                throw new ArgumentNullException("instance");
//            }
//            if (ruleViolations == null)
//            {
//                throw new ArgumentNullException("ruleViolations");
//            }

//            foreach (ValidationResult validationResult in ruleViolations)
//            {
//                foreach (string memberName in validationResult.MemberNames)
//                {
//                    instance.AddModelError(memberName, validationResult.ErrorMessage);
//                }
//            }
//        }
//    }
//}