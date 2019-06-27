## Chef.Extensions.XElement

### TryGetAttribute(XName name, out XAttribute attribute)

Try get attribute on XElement.

Example:

    var result = element.TryGetAttribute("Id", out var attr);
    
    // result is true if element has "Id" attribute.