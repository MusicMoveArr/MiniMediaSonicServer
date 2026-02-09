

using Newtonsoft.Json;

namespace MiniMediaSonicServer.WebJob.Playlists.Application.Models.Navidrome.SmartPlaylist;

public class Operator
{
    public List<Operator>? Any { get; set; }
    public OperatorField? Is { get; set; }
    public OperatorField? IsNot { get; set; }
    
    [JsonProperty("gt")]
    public OperatorField? GreaterThan { get; set; }
    
    [JsonProperty("ls")]
    public OperatorField? LessThan { get; set; }
    
    public OperatorField? Contains { get; set; }
    public OperatorField? NotContains { get; set; }
    public OperatorField? StartsWith { get; set; }
    public OperatorField? EndsWith { get; set; }
    public OperatorField? InTheRange { get; set; }
    public OperatorField? Before { get; set; }
    public OperatorField? After { get; set; }
    public OperatorField? InTheLast { get; set; }
    public OperatorField? NotInTheLastIs { get; set; }
    public OperatorField? InPlaylist { get; set; }
    public OperatorField? NotInPlaylist { get; set; }
    
    
    private OperatorType? _operatorType;
    public OperatorType OperatorType
    {
        get
        {
            if (_operatorType.HasValue)
            {
                return _operatorType.Value;
            }

            var operators = this.GetType()
                .GetProperties()
                .Where(prop => prop.Name != "OperatorType")
                .Where(prop => prop.PropertyType == typeof(OperatorField))
                .Where(prop => (prop.GetValue(this) as OperatorField) != null)
                .Select(prop => prop.Name)
                .ToList()!;
            
            if (operators.Count > 1)
            {
                _operatorType = OperatorType.Unknown;
            }
            else if (operators.Count == 1)
            {
                _operatorType = Enum.Parse<OperatorType>(operators.First());
            }
            return _operatorType.HasValue == true ? _operatorType.Value : OperatorType.Unknown;
        }
    }
    
    private List<OperatorField> _activeOperatorFields;
    public List<OperatorField> ActiveOperatorFields
    {
        get
        {
            if (_activeOperatorFields != null)
            {
                return _activeOperatorFields;
            }

            _activeOperatorFields = this.GetType()
                .GetProperties()
                .Where(prop => prop.Name != "Any")
                .Where(prop => prop.Name != "ActiveOperatorFields")
                .Select(prop => prop.GetValue(this))
                .Select(val => val as OperatorField)
                .Where(val => val != null)
                .ToList()!;
            return _activeOperatorFields;
        }
    }
}