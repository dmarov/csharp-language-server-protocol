﻿using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class ContentModified : RpcError
    {
        internal ContentModified() : base(null, new ErrorMessage(ErrorCodes.ContentModified, "Content Modified")) { }
        internal ContentModified(object id ) : base(id, new ErrorMessage(ErrorCodes.ContentModified, "Content Modified")) { }
    }
}
