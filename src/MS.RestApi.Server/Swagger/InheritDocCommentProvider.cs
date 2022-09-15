using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MS.RestApi.Server.Swagger;

public static class InheritedCommentDocumentBuilder
{
    public static IEnumerable<XDocument> ReplaceInheritedComments(this XDocument[] documents)
    {
        var members = new Dictionary<string, XElement>();
        
        // map identifier to element
        foreach (var document in documents)
        {
            var memberElementList = document.XPathSelectElements("/doc/members/member[@name and not(inheritdoc)]");
            
            foreach (var memberElement in memberElementList)
            {
                var nameAttribute = memberElement.Attribute("name");
                if (nameAttribute == null)
                {
                    continue;
                }
                
                members.Add(nameAttribute.Value, memberElement);
            }
        }
        
        foreach (var document in documents)
        {
            var memberElementList = document.XPathSelectElements("/doc/members/member[inheritdoc[@cref]]");
            
            foreach (var memberElement in memberElementList)
            {
                var inheritDocElement = memberElement.Element("inheritdoc");
                
                if (inheritDocElement is { Parent: {} parent} && inheritDocElement.Attribute("cref") is {} attributeElement)
                {
                    if (members.TryGetValue(attributeElement.Value, out var realDocMember))
                    {
                        parent.ReplaceNodes(realDocMember.Nodes());
                    }
                }
            }

            yield return document;
        }
    }
}