using CSharpFunctionalExtensions;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Models;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Fhir.Patients.Web.Features.Search;

internal static class FhirSearchExpressions
{
    private enum DateTimeFormatType
    {
        Instant,
        Date,
        YearMonth,
        YearOnly
    }

    private static readonly (DateTimeFormatType Type, string Format)[] _dateFormats =
    {
        (DateTimeFormatType.Instant, "yyyy-MM-ddTHH:mm:ss.ffffK"),
        (DateTimeFormatType.Instant, "yyyy-MM-ddTHH:mm:ssK"),
        (DateTimeFormatType.Instant, "yyyy-MM-ddTHH:mmK"),
        (DateTimeFormatType.Instant, "yyyy-MM-ddTHHK"),
        (DateTimeFormatType.Date, "yyyy-MM-dd"),
        (DateTimeFormatType.YearMonth, "yyyy-MM"),
        (DateTimeFormatType.YearOnly, "yyyy")
    };

    internal static Result<Expression<Func<TResource, bool>>, Exception> BuildExpression<TResource>(SearchParameters? searchParameters)
        where TResource : IResource
    {
        if (searchParameters is null)
            return Result.Success<Expression<Func<TResource, bool>>, Exception>(_ => true); //find all expression

        var parameter = Expression.Parameter(typeof(Patient), "x");
        var andExpressions = new List<Expression>();

        foreach (var group in searchParameters.Parameters)
        {
            var resourceProperty = typeof(TResource)
                .GetProperty(
                    group.Key,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (resourceProperty is null)
                return new BadHttpRequestException($"Can not search with key {group.Key}");

            var memberExpression = Expression.Property(expression: parameter, resourceProperty);
            var values = group.ToList();

            if (memberExpression.Type.Equals(typeof(DateTime)) && group.Any())
            {
                foreach (var dateTimeStringValue in values)
                {
                    var expressionResult = GetDateTimeExpression(memberExpression, dateTimeStringValue);

                    if (expressionResult.IsFailure)
                        return expressionResult.Error;

                    andExpressions.Add(expressionResult.Value);
                }
            }
            else if (memberExpression.Type.Equals(typeof(string)))
            {
                foreach (var stringValue in values)
                {
                    var expressionResult = GetStringExpression(memberExpression, stringValue);

                    if (expressionResult.IsFailure)
                        return expressionResult.Error;

                    andExpressions.Add(expressionResult.Value);
                }
            }
        }

        if (andExpressions.Count == 0)
            return new BadHttpRequestException("Invalid search parameters");

        return Expression.Lambda<Func<TResource, bool>>(
            andExpressions.Aggregate(Expression.AndAlso), 
            parameter);
    }

    private static Result<BinaryExpression, Exception> GetStringExpression(
        MemberExpression memberExpression,
        string stringValue)
        //TODO: add wildcard support
        //For now check only equality
        => Expression.Equal(memberExpression, Expression.Constant(stringValue, typeof(string)));

    //https://www.hl7.org/fhir/search.html#date
    private static Result<BinaryExpression, Exception> GetDateTimeExpression(
        MemberExpression memberExpression,
        string dateTimeString)
    {
        if (dateTimeString.Length > 2)
        {
            Prefix prefix = Prefix.eq;

            if (Enum.TryParse<Prefix>(dateTimeString[..2], out var prefixValue) && Enum.GetValues<Prefix>().Contains(prefixValue))
            {
                prefix = prefixValue;
                dateTimeString = dateTimeString[2..];
            }

            DateTime parsedDate = default;
            DateTimeFormatType parsedDateFomatType = DateTimeFormatType.Instant;

            foreach (var format in _dateFormats)
            {
                var dateParsed = DateTimeOffset.TryParseExact(
                    dateTimeString,
                    format.Format,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind,
                    out DateTimeOffset result);

                if (dateParsed)
                {
                    parsedDate = result.DateTime;
                    parsedDateFomatType = format.Type;
                    break;
                }
            }

            return parsedDate == default
                ? new BadHttpRequestException($"Invalid date time value [{dateTimeString}]")
                : GetDateTimeExpression(memberExpression, prefix, parsedDate, parsedDateFomatType);
        }
        else
            return new BadHttpRequestException($"Invalid date time value [{dateTimeString}]");

    }

    private static Result<BinaryExpression, Exception> GetDateTimeExpression(
        MemberExpression memberExpression,
        Prefix comparer,
        DateTime dateTime,
        DateTimeFormatType dateTimeType)
    {
        var dateTimeExpression = Expression.Constant(dateTime, typeof(DateTime));

        if (dateTimeType == DateTimeFormatType.Instant)
        {
            return comparer switch
            {
                Prefix.eq => Expression.Equal(memberExpression, dateTimeExpression),
                Prefix.ne => Expression.NotEqual(memberExpression, dateTimeExpression),
                Prefix.gt => Expression.GreaterThan(memberExpression, dateTimeExpression),
                Prefix.lt => Expression.LessThan(memberExpression, dateTimeExpression),
                Prefix.ge => Expression.GreaterThanOrEqual(memberExpression, dateTimeExpression),
                Prefix.le => Expression.LessThanOrEqual(memberExpression, dateTimeExpression),
                _ => new BadHttpRequestException($"Unsupported prefix [{comparer}].")
            };
        }
        else
        {
            var upperDate = dateTimeType switch
            {
                DateTimeFormatType.Date => dateTime.AddDays(1),
                DateTimeFormatType.YearMonth => new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddMilliseconds(-1),
                DateTimeFormatType.YearOnly => new DateTime(dateTime.Year + 1, 1, 1).AddMilliseconds(-1),
                _ => dateTime
            };

            var upperDateExpression = Expression.Constant(upperDate, typeof(DateTime));

            return comparer switch
            {
                Prefix.eq =>
                    Expression.AndAlso(
                        Expression.GreaterThan(memberExpression, dateTimeExpression),
                        Expression.LessThan(memberExpression, upperDateExpression)
                    ),
                Prefix.ne =>
                    Expression.OrElse(
                        Expression.GreaterThan(memberExpression, upperDateExpression),
                        Expression.LessThan(memberExpression, dateTimeExpression)
                    ),
                Prefix.gt => Expression.GreaterThan(memberExpression, dateTimeExpression),
                Prefix.lt => Expression.LessThan(memberExpression, dateTimeExpression),
                Prefix.ge =>
                    Expression.OrElse(
                        Expression.GreaterThanOrEqual(memberExpression, dateTimeExpression),
                        Expression.AndAlso( //check equality with respect to date dateTimeType
                            Expression.GreaterThan(memberExpression, dateTimeExpression),
                            Expression.LessThan(memberExpression, upperDateExpression))
                    ),
                Prefix.le =>
                    Expression.OrElse(
                        Expression.AndAlso( //check equality with respect to date dateTimeType
                            Expression.GreaterThan(memberExpression, dateTimeExpression),
                            Expression.LessThan(memberExpression, upperDateExpression)),
                        Expression.LessThanOrEqual(memberExpression, dateTimeExpression)
                    ),
                _ => new BadHttpRequestException($"Unsupported prefix [{comparer}]")
            };
        }
    }
}

