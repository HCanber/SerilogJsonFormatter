#BETA: No packages have been published yet!

## A CustomizableJsonFormatter for Serilog

This repo contains a json formatter, `CustomizableJsonFormatter`, for [Serilog](http://serilog.net/) that by default writes messages in the logstash format.

Example:
``` json
{
  "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Information",
  "message": "The time now is 2015-04-16T12:35:36.0900887+02:00",
  "template": "The time now is {TimeNow}",
  "TimeNow": "2015-04-16T12:35:36.0900887+02:00",
}
```
_Note that the object has been indented to make it more readable. Normally this will be written on one line with no whitespaces._

As the name suggests how the json is formatted is customizable.

## Installation
Install the nuget package `Hcanber.Serilog.JsonFormatters`
```
PM> Install-Package Hcanber.Serilog.JsonFormatters
```

## Usage
### Writing logs using RollingFileSink
Use the `RollingFileWithJson()` overload:
``` C#
var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt")
  .CreateLogger();
```

### Usage with other sinks
To use with other sinks, that

``` C#
var log = new LoggerConfiguration()
  .WriteTo.Sink(new FileSink("log.txt", new CustomizableJsonFormatter(), null))
  .CreateLogger();
```


### Change property names<a name="ChangePropertyNames"></a>
If you want to change the names of the properties (`@timestamp`, `level`, `message`, `template` and so on) you do this using `PropertyNames`.

``` C#
var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt",
    jsonPropertyNames: new PropertyNames { Timestamp = "date", Message = "m" })
  .CreateLogger();
```
The code above will produce messages like this:
``` json
{
  "date": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Information",
  "m": "The time now is 2015-04-16T12:35:36.0900887+02:00",
  "template": "The time now is {TimeNow}",
  "TimeNow": "2015-04-16T12:35:36.0900887+02:00",
}
```

### Suppress the rendered message from being written
By setting `shouldRenderMessage` to `false` you can suppress the message property.
``` C#
var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt",
    shouldRenderMessage: false)
  .CreateLogger();
```
The code above will produce messages like this:
``` json
  {
    "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
    "level": "Information",
    "template": "The time now is {TimeNow}",
    "TimeNow": "2015-04-16T12:35:36.0900887+02:00",
  }
```

### All-integer property names are collected under properties
By default properties – like `TimeNow` in the previous examples – are written directly under the root object. There are a few exceptions though. Properties with names consisting of only digits, for example `{0}` and `{1}` will not be written at all.
``` C#
log.Information("The time now is {0} and the user is {1}", DateTime.Now, userId);
```

``` json
{
  "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Information",
  "template": "The time now is {0} and the user is {1}",
}
```

### Property Filters – Defining how property values should be serialized
All properties passes through a chain of filters that define what to do with every property.
The default property handling behavior can be changed by specifying other filters.

__Example:__ To include properties for  `{0}` and `{1}` under a `properties` property:

``` C#
var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt",
    propertyFilter: PropertyFilters.IntPropertiesUnderSharedProperty + PropertyFilters.Default)
  .CreateLogger();

log.Information("The time now is {0} and the user is {1}", DateTime.Now, userId);  
```

``` json
{
  "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Information",
  "template": "The time now is {0} and the user is {1}",
  "properties":
  {
    "0": "2015-04-16T12:35:36.0900887+02:00",  
    "1": "HCanber",  
  }  
}
```
The `properties` property can be renamed. See, [Change property names](#ChangePropertyNames) above.

### Existing property filters
The class `PropertyFilters` contains a few filters.

| Filter Name | Description |
| ----------- | ----------- |
| `AllPropertiesInlined` | Unless a previous filter has specified an action, this filter makes all properties to be written on the root object |
| `AllPropertiesUnderSharedProperty` | Unless a previous filter has specified an action, this filter makes all properties to be written under the shared `properties` property |
| `IntPropertiesUnderSharedProperty` | Unless a previous filter has specified an action, this filter makes all properties with a name consisting of only digits, like `{0}` or `{1}`, to be written under the shared `properties` property |
| `IntPropertiesExcluded` | Unless a previous filter has specified an action, this filter makes all properties with a name consisting of only digits, like `{0}` or `{1}`, to be excluded from the json object |
| `ExcludeProperty(name)` |  Unless a previous filter has specified an action, this filter makes all properties with the specified a name, to be excluded from the json object. By default it ignores casing and will also replace the property in the message template with the rendered value. The default behavior can be changed via named parameters. |
| `ExcludePropertiesStartingWithUnderscore()` |  Unless a previous filter has specified an action, this filter makes all properties with names that starts with an underscore, to be excluded from the json object. By default it will also replace the property in the message template with the rendered value. The default behavior can be changed via named parameters. |
| `Default` | The default filter. Chains the filters `IntPropertiesExcluded`  `AllPropertiesInlined`

### Chaining filters
Filters can be chained using `+` or `|`. These two lines are equivalent:

``` C#
PropertyFilters.IntPropertiesUnderSharedProperty + PropertyFilters.Default
PropertyFilters.IntPropertiesUnderSharedProperty | PropertyFilters.Default
```

The filters are called in the order, so in the example above, first `IntPropertiesUnderSharedProperty` is called, and then `Default`. All filters in `PropertyFilters` have been coded to not overwrite what previous filters have decided, so you can override the default behavior as in the example with the two lines above.

If no filter applies to a property the default behavior is to inline properties and let the message template be as it is.

### Creating filters
You can easily create your own. Below is an example that, for all properties with names starting with an lower case letter, excludes the values them from being written and replace them with their values in the message template.
<a name="removePropertiesStartingWithUnderscores"></a>
``` C#
var removePropertiesStartingWithUnderscores = new DelegatePropertyFilter(action =>
  {
    if(char.IsLower(action.PropertyName[0]))
    {
      if(!action.JsonPropertyAction.HasValue)
      {
        action.JsonPropertyAction = JsonPropertyAction.Exclude;
        action.MessageTemplateAction = MessageTemplateAction.RenderValue;
      }
    }
    return action;
  });

var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt",
    propertyFilter: removePropertiesStartingWithUnderscores | PropertyFilters.Default)
  .CreateLogger();

log.Information("The time now is {TimeNow} and the user is {userName}", DateTime.Now, "hcanber");  
```

``` json
{
  "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Information",
  "message": "The time now is 2015-04-16T12:35:36.0900887+02:00 and the user is hcanber",
  "template": "The time now is {0} and the user is hcanber",
  "TimeNow": "2015-04-16T12:35:36.0900887+02:00",
}
```

How the value of a property is handled is specified on the `JsonPropertyAction` property, and you may choose from:
- Excluded from being written
- Inlined, i.e. a property directly under the root json object
- Put under the shared property `properties` (which can be renamed)
- `null` to let a filter later in the chain decide

How the property is handled on the message template is specified on the `MessageTemplateAction` property, and you may choose from:
- As-is, i.e. do not modify the message template
- Remove it from being written
- Replaced by its value
- `null` to let a filter later in the chain decide

## Tips: Do not concatenate – Remove unnecessary properties
You should __NEVER__ concatenate strings to form the message template. Say you want to log when a user logged on, but for some reason, you don't want to capture the user name in a property.

__DO NOT DO:__
``` C#
log.Debug("The user "+ userId +" logged on"); //NEVER DO THIS
```
Concatenating strings in the message template will fill up Serilog's internal MessageTemplateCache, which is bad.

Instead, add the filter `ExcludeProperty("Ignored")` to and use `{Ignored}` in the template

``` C#
var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt",
    propertyFilter: ExcludeProperty("Ignored") + PropertyFilters.Default)
  .CreateLogger();

log.Debug("The user {Ignored} logged on", userId);
```

``` json
{
  "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Debug",
  "message": "The user hcanber logged on",
  "template": "The user hcanber logged on",
  "TimeNow": "2015-04-16T12:35:36.0900887+02:00",
}
```

Or use `{_UserName}` in the template and the `ExcludePropertiesStartingWithUnderscore()` filter:
``` C#
var log = new LoggerConfiguration()
  .WriteTo.RollingFileWithJson("log-{Date}.txt",
    propertyFilter: ExcludePropertiesStartingWithUnderscore() + PropertyFilters.Default)
  .CreateLogger();

  log.Debug("The user {_UserName} logged on", userId);
  ```

``` json
{
  "@timestamp": "2015-04-16T12:35:36.0900887+02:00",
  "level": "Debug",
  "message": "The user hcanber logged on",
  "template": "The user hcanber logged on",
  "TimeNow": "2015-04-16T12:35:36.0900887+02:00",
}
```

_Note_: If you specify `ExcludePropertiesStartingWithUnderscore(MessageTemplateAction.AsIs)` the `template` property will become: `"template": "The user {UserName} logged on"` (the value is still excluded from being written).

## Tips: Add a hash of message template and exception
If you add a property containing a hash of the message template, then similar messages can easily be found in the log (instead of searching after the template text itself).
By adding a hash for exceptions you can find other message when the same error occurred.

This package contains implementations options for these two, so just add these Enrich-lines to your config:
``` C#
var log = new LoggerConfiguration()
  .Enrich.WithMessageTemplateHash()
  .Enrich.WithExceptionHash()
  .WriteTo.RollingFileWithJson("log-{Date}.txt")
  .CreateLogger();
```
