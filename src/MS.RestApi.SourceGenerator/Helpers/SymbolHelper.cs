using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace MS.RestApi.SourceGenerator.Tests.Helpers;

public class SymbolHelper
{
    public static PropertyDeclarationSyntax CreatePropertyDeclaration(IPropertySymbol propertySymbol)
    {
        // Create the type syntax for the property
        var typeSyntax = SyntaxFactory.ParseTypeName(propertySymbol.Type.ToDisplayString());
        
        // Create the property declaration with type and name
        var propertyDeclaration = SyntaxFactory.PropertyDeclaration(typeSyntax, propertySymbol.Name);
        
        // Handle accessibility
        var modifiers = GetModifiers(propertySymbol.DeclaredAccessibility);
        if (propertySymbol.IsStatic)
        {
            modifiers.Add(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
        }

        if (propertySymbol.IsRequired)
        {
            modifiers.Add(SyntaxFactory.Token(SyntaxKind.RequiredKeyword));
        }
        
        propertyDeclaration = propertyDeclaration.WithModifiers(SyntaxFactory.TokenList(modifiers));
        
        // Create accessors (get and/or set)
        var accessors = SyntaxFactory.AccessorList();
        if (propertySymbol.GetMethod is { } gm)
        {
            var getAccessor = SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration);

            if (gm.DeclaredAccessibility != propertySymbol.DeclaredAccessibility)
            {
                getAccessor = getAccessor.AddModifiers(GetModifiers(gm.DeclaredAccessibility).ToArray());
            }
            
            getAccessor = getAccessor.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            accessors = accessors.AddAccessors(getAccessor);
        }

        if (propertySymbol.SetMethod is { } sm)
        {
            var setKind = sm.IsInitOnly ? SyntaxKind.InitAccessorDeclaration : SyntaxKind.SetAccessorDeclaration;
            var setAccessor = SyntaxFactory.AccessorDeclaration(setKind);
            
            if (sm.DeclaredAccessibility != propertySymbol.DeclaredAccessibility)
            {
                setAccessor = setAccessor.AddModifiers(GetModifiers(sm.DeclaredAccessibility).ToArray());
            }
            
            setAccessor = setAccessor.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            accessors = accessors.AddAccessors(setAccessor);
        }

        return propertyDeclaration.WithAccessorList(accessors);
    }

    private static HashSet<SyntaxToken> GetModifiers(Accessibility accessibility)
    {
        var modifiers = new HashSet<SyntaxToken>();
        
        switch (accessibility)
        {
            case Accessibility.Public:
                modifiers.Add(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                break;
            case Accessibility.Private:
                modifiers.Add(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
                break;
            case Accessibility.Protected:
                modifiers.Add(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword));
                break;
            case Accessibility.Internal:
                modifiers.Add(SyntaxFactory.Token(SyntaxKind.InternalKeyword));
                break;
            case Accessibility.NotApplicable:
                break;
            case Accessibility.ProtectedAndInternal:
                break;
            case Accessibility.ProtectedOrInternal:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return modifiers;
    }
}