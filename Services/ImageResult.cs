using System;
using Microsoft.AspNetCore.Mvc;

namespace OOL_API.Services
{
#nullable enable
    public struct ImageResult
    {
        public byte[] Content { get; }

        public string ContentType { get; }

        public ImageResult(byte[] content, string contentType)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        }

        public FileContentResult ToFileResult()
        {
            return new FileContentResult(Content, ContentType);
        }
    }
}