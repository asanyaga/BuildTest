using System;
using System.Text;
using Distributr.Mobile.Envelopes;

namespace Distributr.Mobile.Core.ErrorLog
{
    public class ErrorLogEntry
    {
        public static string ErrorLogEntriesForDirection(RoutingDirection direction)
        {
            return string.Format(@" SELECT 
                                    ParentDoucmentGuid, DateLastUpdated, RoutingDirection, ErrorMessage
                                FROM 
                                    LocalCommandEnvelope                  
                                WHERE 
                                    RoutingDirection = {0} 
                                AND 
                                    RoutingStatus = {1}
                                GROUP BY
                                    ParentDoucmentGuid, ErrorMessage, RoutingDirection
                                ORDER BY 
                                    DateLastUpdated DESC", (int)direction, (int)RoutingStatus.Error);
        }

        public Guid ParentDoucmentGuid { get; set; }
        public RoutingDirection RoutingDirection { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateLastUpdated { get; set; }

        public override string ToString()
        {
            return new StringBuilder()
                .Append("Error Date")
                .Append("\n")
                .Append(DateLastUpdated)
                .Append("\n\n")
                .Append("Parent Document ID")
                .Append("\n")
                .Append(ParentDoucmentGuid)
                .Append("\n\n")
                .Append("Error Details")
                .Append("\n")
                .Append(ErrorMessage)
                .ToString();
        }
    }
}
