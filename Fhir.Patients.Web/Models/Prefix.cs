namespace Fhir.Patients.Web.Models;

/// <summary>
/// https://www.hl7.org/fhir/search.html#prefix
/// </summary>
public enum Prefix
{
    eq, //the resource value is equal to or fully contained by the parameter value
    ne, //the resource value is not equal to the parameter value
    gt, //the resource value is greater than the parameter value
    lt, //the resource value is less than the parameter value
    ge, //the resource value is greater or equal to the parameter value
    le, //the resource value is less or equal to the parameter value
    sa, //the resource value starts after the parameter value
    eb, //the resource value ends before the parameter value
    ap  //the resource value is approximately the same to the parameter value. Note that the recommended value for the approximation is 10% of the stated value (or for a date, 10% of the gap between now and the date), but systems may choose other values where appropriate
}
