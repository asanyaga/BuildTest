using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using Distributr.Core.Utility.Validation;

namespace Distributr.Core.Tests.Validation
{
    [TestFixture]
    public class ValidationFixture
    {
        [Test]
        public void BasicTest()
        {
            TestClass c = new TestClass();
            ValidationContext vt = new ValidationContext(c, null, null);

            List<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateObject(c, vt,   results);

            Assert.IsTrue(results.Count() > 0);
        }

        [Test]
        public void ValidatorClass()
        {
            V2 v = new V2();
            TestClass c = new TestClass();
            ValidationResultInfo result = v.Validate(c);
            Assert.IsTrue(result.Results.Count() > 0);
            Assert.IsFalse(result.IsValid);
        }

        
    }

    public class TestClass
    {
        [Required(ErrorMessage= "Error Message1")]
        public string Item1 { get; set; }
        [Required(ErrorMessage="Error Message2")]
        public string Item2 { get; set; }
    }

  

   

    public class V2 :  IValidation<TestClass>
    {
        public ValidationResultInfo Validate(TestClass itemToValidate)
        {
            return itemToValidate.BasicValidation();
        }

    }

}
