using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Input;
using TableCloth2.Shared;

namespace TableCloth2;

public static class Helpers
{
    public static Binding Bind<TListControl, TSource, TSourceProperty>(
        this TListControl listControl,
        TSource dataSource,
        Expression<Func<TSource, TSourceProperty>> dataSourcePropertyExpression,
        bool formattingEnabled = true,
        DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged,
        string displayMemberName = "",
        string valueMemberName = "")
        where TListControl : ListControl
        where TSource : notnull
    {
        var binding = listControl.Bind(
            c => c.DataSource,
            dataSource, dataSourcePropertyExpression,
            formattingEnabled, updateMode);

        listControl.DisplayMember = displayMemberName;
        listControl.ValueMember = valueMemberName;
        return binding;
    }

    public static Binding Bind<TBindableComponent, TSource, TBindableComponentProperty, TSourceProperty>(
        this TBindableComponent bindableComponent,
        Expression<Func<TBindableComponent, TBindableComponentProperty>> controlPropertyExpression,
        TSource dataSource,
        Expression<Func<TSource, TSourceProperty>> dataSourcePropertyExpression,
        bool formattingEnabled = true,
        DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged)
        where TBindableComponent : IBindableComponent
        where TSource : notnull
    {
        return bindableComponent.DataBindings.Add(
            GetPropertyName(controlPropertyExpression), dataSource,
            GetPropertyName(dataSourcePropertyExpression),
            formattingEnabled, updateMode);
    }

    public static Binding Bind<TBindableComponent, TBindableComponentProperty>(
        this TBindableComponent bindableComponent,
        Expression<Func<TBindableComponent, TBindableComponentProperty>> controlPropertyExpression,
        object dataSource,
        string dataSourcePropertyName,
        bool formattingEnabled = true,
        DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged)
        where TBindableComponent : IBindableComponent
    {
        return bindableComponent.DataBindings.Add(
            GetPropertyName(controlPropertyExpression), dataSource,
            dataSourcePropertyName,
            formattingEnabled, updateMode);
    }

    public static Binding Bind<TBindableComponent, TSource, TSourceProperty>(
        this TBindableComponent bindableComponent,
        string controlPropertyName,
        TSource dataSource,
        Expression<Func<TSource, TSourceProperty>> dataSourcePropertyExpression,
        bool formattingEnabled = true,
        DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged)
        where TBindableComponent : IBindableComponent
        where TSource : notnull
    {
        return bindableComponent.DataBindings.Add(
            controlPropertyName, dataSource,
            GetPropertyName(dataSourcePropertyExpression),
            formattingEnabled, updateMode);
    }

    private static string GetPropertyName<TSource, TProperty>(
        Expression<Func<TSource, TProperty>> propertyExpression)
        where TSource : notnull
    {
        if (propertyExpression.Body is MemberExpression memberExpr)
            return memberExpr.Member.Name;
        else if (propertyExpression.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression operandExpr)
            return operandExpr.Member.Name;

        throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
    }

    public static EventHandler<TEventArgs> ToEventHandler<TEventArgs>(this ICommand command)
        where TEventArgs : EventArgs
    {
        return new EventHandler<TEventArgs>((sender, e) =>
        {
            var args = new RelayCommandArgument(sender, e);

            if (!command.CanExecute(args))
                return;

            command.Execute(args);
        });
    }

    public static EventHandler ToEventHandler(this ICommand command)
    {
        return new EventHandler((sender, e) =>
        {
            var args = new RelayCommandArgument(sender, e);

            if (!command.CanExecute(args))
                return;

            command.Execute(args);
        });
    }

    internal static readonly string TableClothHttpClient = nameof(TableClothHttpClient);

    public static IServiceCollection AddTableClothHttpClient(
        this IServiceCollection collection,
        int retryCount = 5)
    {
        var policyHandler = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        collection
            .AddHttpClient(TableClothHttpClient, c =>
            {
                c.BaseAddress = new Uri("https://yourtablecloth.app", UriKind.Absolute);
                c.DefaultRequestHeaders.UserAgent.ParseAdd($"TableCloth/2.0 ({RuntimeInformation.FrameworkDescription}; {RuntimeInformation.RuntimeIdentifier}) TableClothHttpClient/2.0");
            })
            .AddPolicyHandler(policyHandler);
        return collection;
    }

    public static HttpClient GetTableClothHttpClient(this IHttpClientFactory factory)
        => factory.CreateClient(TableClothHttpClient);

    public static string Combine<TFileSystemInfo>(this TFileSystemInfo fileInfo, params string[] paths)
        where TFileSystemInfo : FileSystemInfo
        => Path.Combine(new string[] { fileInfo.FullName, }.Concat(paths).ToArray());

    public static Binding ApplyValueConverter(this Binding binding,
        ConvertEventHandler convert,
        ConvertEventHandler? convertBack = default)
    {
        binding.Format += convert;

        if (convertBack != null)
            binding.Parse += convertBack;

        return binding;
    }
}
