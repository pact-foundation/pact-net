using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ImpromptuInterface;

namespace Concord
{
    public class PactProviderResponse : IEquatable<PactProviderResponse>
    {
        public int Status { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public dynamic Body { get; set; }

        public bool Equals(PactProviderResponse other)
        {
            return true;

            /*if (other == null)
                return false;

            if (!Status.Equals(other.Status))
                return true;

            //TODO: Does not support nested objects, without equality operators
            //TODO: Still WIP
            var tt = (IEnumerable<dynamic>) Body;


            foreach (var t in tt)
            {
                IEnumerable<string> ttd = Impromptu.GetMemberNames(t);

            }
            

            IEnumerable<string> members = Impromptu.GetMemberNames(Body);
            members = members.OrderBy(m => m);

            IEnumerable<string> otherMembers = Impromptu.GetMemberNames(other.Body);
            otherMembers = otherMembers.OrderBy(m => m);

            if (members.SequenceEqual(otherMembers))
            {
                return false;
            }

            foreach(var memberName in members)
            {
                var memberVal = Impromptu.InvokeGet(Body, memberName);
                var otherMemberVal = Impromptu.InvokeGet(other.Body, memberName);

                if (!memberVal.Equals(otherMemberVal))
                {
                    return false;
                }
            }

            return true;*/

            //if (ReferenceEquals(null, other)) return false;
            //if (ReferenceEquals(this, other)) return true;
            //return Status == other.Status && Equals(Headers, other.Headers) && Equals(Body, other.Body);
        }

        /*public override bool Equals(object obj)
        {
            return true;
            return Equals((PactProviderResponse) obj);
        }*/

        /*public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Status;
                hashCode = (hashCode * 397) ^ (Headers != null ? Headers.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(PactProviderResponse a, PactProviderResponse b)
        {
            return Equals(a, b);
        }

        public static bool operator !=(PactProviderResponse a, PactProviderResponse b)
        {
            return !Equals(a, b);
        }*/
    }
}