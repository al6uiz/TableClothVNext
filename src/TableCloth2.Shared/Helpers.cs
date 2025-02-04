using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System.Linq.Expressions;
using System.Net;
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

public static class HttpClientHelpers
{
    internal static IServiceCollection AddCustomHttpClient(
        this IServiceCollection collection,
        string name,
        Action<HttpClient>? configureClient = null,
        Func<HttpMessageHandler>? configureMessageHandler = null,
        int retryCount = 5)
    {
        var policyHandler = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        var clientBuilder = collection.AddHttpClient(name);
        if (configureClient != null)
            clientBuilder.ConfigureHttpClient(configureClient);
        if (configureMessageHandler != null)
            clientBuilder.ConfigurePrimaryHttpMessageHandler(configureMessageHandler);
        clientBuilder.AddPolicyHandler(policyHandler);

        return collection;
    }

    internal static readonly string TableClothHttpClient = nameof(TableClothHttpClient);

    public static IServiceCollection AddTableClothHttpClient(this IServiceCollection collections, int retryCount = 5)
    {
        return collections.AddCustomHttpClient(
            TableClothHttpClient,
            (client) =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    $"TableCloth/2.0 ({RuntimeInformation.FrameworkDescription}; {RuntimeInformation.RuntimeIdentifier}) TableClothHttpClient/2.0");
                client.BaseAddress = new Uri("https://yourtablecloth.app", UriKind.Absolute);
            },
            default,
            retryCount);
    }

    public static HttpClient GetTableClothHttpClient(this IHttpClientFactory factory)
        => factory.CreateClient(TableClothHttpClient);

    internal static readonly string ChromeLikeHttpClient = nameof(ChromeLikeHttpClient);

    public static IServiceCollection AddChromeLikeHttpClient(this IServiceCollection collections, int retryCount = 5)
    {
        return collections.AddCustomHttpClient(
            ChromeLikeHttpClient,
            (client) =>
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.9,en-US;q=0.8,en;q=0.7");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            },
            () => new HttpClientHandler
            {
                UseCookies = true,
                AllowAutoRedirect = true,
                CookieContainer = new CookieContainer(),
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
            },
            retryCount);
    }

    public static HttpClient GetChromeLikeHttpClient(this IHttpClientFactory factory)
        => factory.CreateClient(ChromeLikeHttpClient);
}
