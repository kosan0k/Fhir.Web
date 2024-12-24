using CSharpFunctionalExtensions;
using Fhir.Patients.Domain.Models;
using Fhir.Patients.Web.Features.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Linq.Expressions;

namespace Fhir.Patients.Web.Models;

[ModelBinder(typeof(SearchParametersModelBinder))]
public class SearchParameters
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

    public required ILookup<string, string> Parameters { get; init; }

    internal Result<Expression<Func<Patient, bool>>, Exception> BuildExpression()
    {
        var parameter = Expression.Parameter(typeof(Patient), "x");

        var andExpressions = new List<Expression>();

        foreach (var group in Parameters)
        {
            var property = Expression.Property(expression: parameter, propertyName: group.Key);

            if (property is null)
                continue;

            if (property.Type.Equals(typeof(DateTime)) && group.Any())
            {
                //https://www.hl7.org/fhir/search.html#date

                var values = group.ToList();

                foreach (var value in values)
                {
                    if (value.Length > 2)
                    {
                        Prefix prefix = Prefix.eq;
                        string dateValue = value;

                        if (Enum.TryParse<Prefix>(value[..2], out var prefixValue) && Enum.GetValues<Prefix>().Contains(prefixValue))
                        {
                            prefix = prefixValue;
                            dateValue = value[2..];
                        }

                        DateTime parsedDate = default;
                        DateTimeFormatType parsedDateFomatType = DateTimeFormatType.Instant;

                        foreach (var format in _dateFormats)
                        {
                            var dateParsed = DateTimeOffset.TryParseExact(
                                dateValue,
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

                        if (parsedDate == default)
                            return new BadHttpRequestException($"Invalid date value [{value}]");

                        var expressionResult = GetDateTimeExpression(property, prefix, parsedDate, parsedDateFomatType);

                        if (expressionResult.IsFailure)
                            return expressionResult.Error;

                        andExpressions.Add(expressionResult.Value);
                    }
                }
            }
        }

        var finalExpression = andExpressions.Aggregate(Expression.AndAlso);

        return Expression.Lambda<Func<Patient, bool>>(finalExpression, parameter);
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
