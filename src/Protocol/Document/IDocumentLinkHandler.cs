using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Document
{
    [Parallel, Method(TextDocumentNames.DocumentLink, Direction.ClientToServer)]
    [GenerateHandlerMethods, GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentLinkHandler : IJsonRpcRequestHandler<DocumentLinkParams, DocumentLinkContainer>,
        IRegistration<DocumentLinkRegistrationOptions>, ICapability<DocumentLinkCapability>
    {
    }

    [Parallel, Method(TextDocumentNames.DocumentLinkResolve, Direction.ClientToServer)]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    public interface IDocumentLinkResolveHandler : ICanBeResolvedHandler<DocumentLink>
    {
    }

    public abstract class DocumentLinkHandler : IDocumentLinkHandler, IDocumentLinkResolveHandler
    {
        private readonly DocumentLinkRegistrationOptions _options;

        public DocumentLinkHandler(DocumentLinkRegistrationOptions registrationOptions)
        {
            _options = registrationOptions;
        }

        public DocumentLinkRegistrationOptions GetRegistrationOptions() => _options;

        public abstract Task<DocumentLinkContainer> Handle(DocumentLinkParams request, CancellationToken cancellationToken);

        public abstract Task<DocumentLink> Handle(DocumentLink request, CancellationToken cancellationToken);
        public abstract bool CanResolve(DocumentLink value);
        public virtual void SetCapability(DocumentLinkCapability capability) => Capability = capability;
        protected DocumentLinkCapability Capability { get; private set; }
    }

    public static partial class DocumentLinkExtensions
    {
        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, DocumentLinkCapability, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, bool> canResolve,
            Func<DocumentLink, DocumentLinkCapability, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<DocumentLink>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.Request<DocumentLinkParams, DocumentLinkContainer, DocumentLinkCapability,
                            DocumentLinkRegistrationOptions>(
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkCapability, DocumentLinkRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, CancellationToken, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, bool> canResolve,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<DocumentLink>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                    new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams, DocumentLinkContainer,
                        DocumentLinkRegistrationOptions>(
                        handler,
                        registrationOptions)).AddHandler(TextDocumentNames.DocumentLinkResolve,
                    new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                        resolveHandler,
                        canResolve,
                        registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Func<DocumentLinkParams, Task<DocumentLinkContainer>> handler,
            Func<DocumentLink, bool> canResolve,
            Func<DocumentLink, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<DocumentLink>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        new LanguageProtocolDelegatingHandlers.RequestRegistration<DocumentLinkParams, DocumentLinkContainer,
                            DocumentLinkRegistrationOptions>(
                            handler,
                            registrationOptions))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, DocumentLinkCapability, CancellationToken> handler,
            Func<DocumentLink, bool> canResolve,
            Func<DocumentLink, DocumentLinkCapability, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, cap, token) => Task.FromException<DocumentLink>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink, DocumentLinkCapability,
                            DocumentLinkRegistrationOptions>(
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkCapability, DocumentLinkRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>, CancellationToken> handler,
            Func<DocumentLink, bool> canResolve,
            Func<DocumentLink, CancellationToken, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link, token) => Task.FromException<DocumentLink>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink,
                            DocumentLinkRegistrationOptions>(
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }

        public static ILanguageServerRegistry OnDocumentLink(this ILanguageServerRegistry registry,
            Action<DocumentLinkParams, IObserver<IEnumerable<DocumentLink>>> handler,
            Func<DocumentLink, bool> canResolve,
            Func<DocumentLink, Task<DocumentLink>> resolveHandler,
            DocumentLinkRegistrationOptions registrationOptions)
        {
            registrationOptions ??= new DocumentLinkRegistrationOptions();
            registrationOptions.ResolveProvider = canResolve != null && resolveHandler != null;
            canResolve ??= item => registrationOptions.ResolveProvider;
            resolveHandler ??= (link) => Task.FromException<DocumentLink>(new NotImplementedException());

            return registry.AddHandler(TextDocumentNames.DocumentLink,
                        _ => new LanguageProtocolDelegatingHandlers.PartialResults<DocumentLinkParams, DocumentLinkContainer, DocumentLink,
                            DocumentLinkRegistrationOptions>(
                            handler,
                            registrationOptions,
                            _.GetRequiredService<IProgressManager>(),
                            x => new DocumentLinkContainer(x)))
                    .AddHandler(TextDocumentNames.DocumentLinkResolve,
                        new LanguageProtocolDelegatingHandlers.CanBeResolved<DocumentLink, DocumentLinkRegistrationOptions>(
                            resolveHandler,
                            canResolve,
                            registrationOptions))
                ;
        }
    }
}
