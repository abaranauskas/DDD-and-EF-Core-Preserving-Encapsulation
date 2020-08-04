using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace App
{
    public class Name : ValueObject
    {
        protected Name()
        {
        }

        private Name(string first, string last, Suffix suffix)
        {
            First = first;
            Last = last;
            Suffix = suffix;
        }

        public string First { get; }
        public string Last { get; }

        public virtual Suffix Suffix { get; }

        public static Result<Name> Create(string firstName, string lastName, Suffix suffix)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                return Result.Failure<Name>("firstName should not be empty");
            if (string.IsNullOrWhiteSpace(lastName))
                return Result.Failure<Name>("lastName should not be empty");

            firstName = firstName.Trim();
            lastName = lastName.Trim();

            if (firstName.Length > 200)
                return Result.Failure<Name>("firstName is too long");
            if (lastName.Length > 200)
                return Result.Failure<Name>("lastName is too long");

            return Result.Success(new Name(firstName, lastName, suffix));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return First;
            yield return Last;
            yield return Suffix;
        }
    }
}
