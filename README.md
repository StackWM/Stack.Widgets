# Stack.Widgets
A collection of widgets for Stack

- [Widgets](#widgets)
  - [Usage](#usage)
  - [WebView](#webview)
  - [Underlay](#underlay)
- [Data Sources](#data-sources)
  - [Usage](#usage)
  - [WebDataSource](#webdatasource)
  - [RefreshableDataSource](#refreshabledatasource)
  - [Refreshable data sources](#refreshable-data-sources)
- [Parsers and data binding](docs/DataBinding.md)
- [Samples](https://stackwm.github.io/Inline-Widgets/)

# Widgets
## Usage
Requires Stack v2.1+ (release planned for the end of June 2018).

Reference widgets library in your layout like this:
```xml
xmlns:widgets="clr-namespace:LostTech.Stack.Widgets;assembly=LostTech.Stack.Widgets"
```

Insert your widget(s):
```xml
<widgets:__SOME__WIDGET__ ...></widgets:__SOME__WIDGET__>
```

## WebView
Example:
```xml
<widgets:WebView URL="https://detectmybrowser.com/"/>
```

**URL** (bindable, *OneWay*) - get or set the URL to display

Current version uses IE engine.

## Underlay

There's an easy way to hide widgets from layout grid, when you are dragging a window:
```xml
<widgets:__WIDGET__ ... zones:Layout.IsUnderlay="True" .../>
```

Setting this will automatically hide your widget from the dragging overlay. Note, this setting is an opposite of **IsHint**, which shows element only in the overlay.

# Data Sources
These are helpful if you want to create custom inline widgets inside your layout. Together with parsers (see [data binding](docs/DataBinding.md)), they allow to build complex data display for virtually any information source you could imagine.

## Usage
Requires Stack v2.1+

Add a reference to out of box data sources in your layout:
```xml
xmlns:sources="clr-namespace:LostTech.Stack.Widgets.DataSources;assembly=LostTech.Stack.Widgets"
```

If its not already there, add ```<*.Resources>``` section to your layout's root.
Add your data source to ```Resouces``` section.

For example:
```xml
<Grid
  ...
  xmlns:...
  xmlns:...
  ...
>
  <Grid.Resources>
    <sources:__SOME_DATA_SOURCE__ x:Key="MySource" 
              ...
              />  
  </Grid.Resources>
  ...
</Grid>
```

## WebDataSource
```WebDataSource``` allows you to fetch data from the Internet. Can refresh data automatically, if the remote source provides expiration information (see ```Expiration``` below). This data source is *Refreshable* (see below).
### Settable properties
**Url** (bindable, **required**) - sets the URL to fetch data from.

Example: ```Url="https://www.hanselman.com/blog/SyndicationService.asmx/GetAtom"```

Notes: ```WebDataSource``` does not currently provide any authentication mechanisms. So either the ```Url``` must be unauthenticated, or the authentication information must be passed in the ```Url``` itself.
### Provided data
**Content**: ```string``` (bindable, optional) - provides the text, retrieved from the data request. Usually, it will be JSON, XML or HTML. Returns ```null``` if the last request was not successful.

**ContentTimestamp**: ```DateTime``` (bindable) - provides the time, when the ```Content``` property was last updated.

**Error**: ```Exception``` (bindable, optional) - a network error, if any. Set to ```null``` if data source received valid, but possibly unsuccessful response from the server. Most useful is its ```Message``` property.

**Expiration**: ```DateTime?``` (bindable, optional) - if a successful response was received, and server provided expiration time in it, it will be reflected in this property. By default, when this time will come, ```WebDataSource``` will refresh itself.

**Response***: [HttpResponseMessage](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage) (bindable, optional, *OneWay*) - if any response was received (see ```Error``` above otherwise), this will contain .NET standard response object. 

Its most common properties are [IsSuccessStatusCode](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage.issuccessstatuscode), that indicates, if request was succesfully processed, and [StatusCode](https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpresponsemessage.statuscode), that returns corresponding HTTP status code.

## RefreshableDataSource
```RefreshableDataSource``` is actually just a wrapper for other data sources, or .NET properties. It enables periodic refresh of data from the source, even if the source does not update automatically. Hense, this data source is *Refreshable*.

### Example
In this example we create a data source from a static .NET [DateTime.Now](https://docs.microsoft.com/en-us/dotnet/api/system.datetime.now) property. ```RefreshableDataSource``` wrapping is required, because ```DateTime.Now``` does not raise any change events, so UI does not know when to update it. We set the refresh interval to 1 second.
```xml
<sources:RefreshableDataSource x:Key="TimeSource" 
         Source="{c:Binding sys:DateTime.Now}"
         sources:DataSource.RefreshInterval="0:0:1"
         />
...
<TextBlock Text="{Binding Source, Source={StaticResource TimeSource}, Mode=OneWay}"/>
```
### Settable properties
**Source** (bindable, **required**) - sets the source property to get the data from. The same property is used to retrieve the data.
**DataSource.RefreshInterval** (bindable, **required**) - sets the refresh interval. This property can actually be set on any *Refreshable* data source, see below.
### Provided data
**Source** (bindable) - gets the data from the wrapped data source.

## Refreshable data sources
Some data sources (currently - all out of box ones) are *refreshable*. They implement ```IRefreshable``` interface, which provides a ```RefreshCommand```, that can be executed to refresh data. For example:
```xml
<Button Command="{Binding RefreshCommand, Source={StaticResource SOME_REFRESHABLE_DATASOURCE}}">Refresh!</Button>
```
The following property can be set to update them at regular intervals:

**DataSource.RefreshInterval** (bindable) - sets the refresh interval.

## Samples
Sample inline widgets are demostrated in the [blog post](https://stackwm.github.io/Inline-Widgets/)
