using System;
using System.Collections.Generic;

namespace SQLDM
{
    public class ValidationResult
    {
        public string Message { get; set; }
        public string AdditionalInfo { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }

    public class ValidationResults: List<ValidationResult>
    {
        public bool IsValid
        {
            get
            {
                return Count == 0;
            }
        }



        public void Add(string message)
        {
            Add(message, "");
        }

        public void Add(string message, string additionalInfo)
        {
            ValidationResult newResult = new ValidationResult();
            newResult.Message = message;
            newResult.AdditionalInfo = additionalInfo;

            Add(newResult);
        }

        public override string ToString()
        {
            return string.Join("\r\n", this);
        }

    }
}
