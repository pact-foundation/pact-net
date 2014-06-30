impromptu-interface http://code.google.com/p/impromptu-interface/

C# 4.0 framework to allow you to wrap any object with a static Duck Typing Interface, emits cached dynamic binding code inside a proxy.

Copyright 2010-2012 Ekon Benefits
Apache Licensed: http://www.apache.org/licenses/LICENSE-2.0

Author:
Jay Tuley jay+code@tuley.name


Usage:

    public interface ISimpeleClassProps
    {
        string Prop1 { get;  }

        long Prop2 { get; }

        Guid Prop3 { get; }
    }
    var tAnon = new {Prop1 = "Test", Prop2 = 42L, Prop3 = Guid.NewGuid()};

    ISimpeleClassProps tActsLike = tAnon.ActLike<ISimpeleClassProps>();
Or

    dynamic tNew = new ExpandoObject();
    tNew.Prop1 = "Test";
    tNew.Prop2 = 42L;
    tNew.Prop3 = Guid.NewGuid();

    ISimpeleClassProps tActsLike = Impromptu.ActLike(tNew);

Also Contains may primitive base classes such as:

ImpromptuObject --Similar to DynamicObject but the expected static return type from the wrapped interface can be queried.
ImpromptuFactory -- Functional base class, used to create fluent factories with less boilerplate
ImpromptuDictionary -- Similar to ExpandoObject but returns default of the static return type if the property has never been set.
Mimic -- Accepts any call you make
ImpromptuLateLibraryType -- Allows you to use a type loaded at runtime using the dynamic keyword.

Includes Fluent syntaxes for buiding object graphs & Regexes & Currying

And has a full suite of helper invocation methods:

    dynamic InvokeConvert(object target, Type type, bool explict =false)
    dynamic InvokeConstructor(Type type, params object[] args)
    dynamic Impromptu.InvokeGet(object target, String_Or_InvokeMemberName name)
    dynamic Impromptu.InvokeSet(object target, String_Or_InvokeMemberName name, object value)
    dynamic InvokeGetIndex(object target, params object[] indexes)
    dynamic InvokeSetIndex(object target, params object[] indexesThenValue)
    dynamic Impromptu.InvokeMember(object target, String_Or_InvokeMemberName name, params object[] args)
    dynamic Impromptu.InvokeMemberAction(object target, String_Or_InvokeMemberName name, params object[] args)
    dynamic Impromptu.Invoke(object target, params object[] args)
    dynamic Impromptu.InvokeAction(object target, params object[] args)
    dynamic Impromptu.InvokeGetChain(object target, string propertyChain)
    dynamic Impromptu.InvokeSetChain(object target, string propertyChain)
    dynamic Impromputu.InvokeSetAll(object target, ...)
    IEnumerable<string> GetMemberNames(object target, bool dynamicOnly = false)
    void Impromputu.InvokeAddAssign(object target, string name, object value)
    void Impromputu.InvokeSubtractAssign(object target, string name, object value)
    bool Impromputu.InvokeIsEvent(object target, string name)
    dynamic Impromputu.InvokeBinaryOperator(dynamic leftArg, ExpressionType op, dynamic rightArg)
    dynamic Impromputu.InvokeUnaryOpartor(ExpressionType op, dynamic arg)
    dynamic Impromputu.CoerceConvert(object target, Type type)  //uses every runtime conversion available to convert.

