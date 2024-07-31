﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Core.Assets;
using Squidex.Domain.Apps.Core.Rules.EnrichedEvents;
using Squidex.Infrastructure;

namespace Squidex.Domain.Apps.Entities.Contents.GraphQL.Types.Assets;

internal sealed class EnrichedAssetEventGraphType : SharedObjectGraphType<EnrichedAssetEvent>
{
    public EnrichedAssetEventGraphType()
    {
        // The name is used for equal comparison. Therefore it is important to treat it as readonly.
        Name = "EnrichedAssetEvent";

        AddField(new FieldType
        {
            Name = "type",
            ResolvedType = Scalars.EnrichedAssetEventType,
            Resolver = Resolve(x => x.Type),
            Description = FieldDescriptions.EventType
        });

        AddField(new FieldType
        {
            Name = "id",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.Id.ToString()),
            Description = FieldDescriptions.EntityId
        });

        AddField(new FieldType
        {
            Name = "version",
            ResolvedType = Scalars.NonNullInt,
            Resolver = Resolve(x => x.Version),
            Description = FieldDescriptions.EntityVersion
        });

        AddField(new FieldType
        {
            Name = "created",
            ResolvedType = Scalars.NonNullDateTime,
            Resolver = Resolve(x => x.Created.ToDateTimeUtc()),
            Description = FieldDescriptions.EntityCreated
        });

        AddField(new FieldType
        {
            Name = "createdBy",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.CreatedBy.ToString()),
            Description = FieldDescriptions.EntityCreatedBy
        });

        AddField(new FieldType
        {
            Name = "createdByUser",
            ResolvedType = UserGraphType.NonNull,
            Resolver = Resolve(x => x.CreatedBy),
            Description = FieldDescriptions.EntityCreatedBy
        });

        AddField(new FieldType
        {
            Name = "lastModified",
            ResolvedType = Scalars.NonNullDateTime,
            Resolver = Resolve(x => x.LastModified.ToDateTimeUtc()),
            Description = FieldDescriptions.EntityLastModified
        });

        AddField(new FieldType
        {
            Name = "lastModifiedBy",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.LastModifiedBy.ToString()),
            Description = FieldDescriptions.EntityLastModifiedBy
        });

        AddField(new FieldType
        {
            Name = "lastModifiedByUser",
            ResolvedType = UserGraphType.NonNull,
            Resolver = Resolve(x => x.LastModifiedBy),
            Description = FieldDescriptions.EntityLastModifiedBy
        });

        AddField(new FieldType
        {
            Name = "mimeType",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.MimeType),
            Description = FieldDescriptions.AssetMimeType
        });

        AddField(new FieldType
        {
            Name = "url",
            ResolvedType = Scalars.NonNullString,
            Resolver = Url,
            Description = FieldDescriptions.AssetUrl
        });

        AddField(new FieldType
        {
            Name = "thumbnailUrl",
            ResolvedType = Scalars.String,
            Resolver = ThumbnailUrl,
            Description = FieldDescriptions.AssetThumbnailUrl
        });

        AddField(new FieldType
        {
            Name = "fileName",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.FileName),
            Description = FieldDescriptions.AssetFileName
        });

        AddField(new FieldType
        {
            Name = "fileHash",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.FileHash),
            Description = FieldDescriptions.AssetFileHash
        });

        AddField(new FieldType
        {
            Name = "fileType",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.FileName.FileType()),
            Description = FieldDescriptions.AssetFileType
        });

        AddField(new FieldType
        {
            Name = "fileSize",
            ResolvedType = Scalars.NonNullInt,
            Resolver = Resolve(x => x.FileSize),
            Description = FieldDescriptions.AssetFileSize
        });

        AddField(new FieldType
        {
            Name = "fileVersion",
            ResolvedType = Scalars.NonNullInt,
            Resolver = Resolve(x => x.FileVersion),
            Description = FieldDescriptions.AssetFileVersion
        });

        AddField(new FieldType
        {
            Name = "slug",
            ResolvedType = Scalars.NonNullString,
            Resolver = Resolve(x => x.Slug),
            Description = FieldDescriptions.AssetSlug
        });

        AddField(new FieldType
        {
            Name = "isProtected",
            ResolvedType = Scalars.NonNullBoolean,
            Resolver = Resolve(x => x.IsProtected),
            Description = FieldDescriptions.AssetIsProtected
        });

        AddField(new FieldType
        {
            Name = "isImage",
            ResolvedType = Scalars.NonNullBoolean,
            Resolver = Resolve(x => x.AssetType == AssetType.Image),
            Description = FieldDescriptions.AssetIsImage,
            DeprecationReason = "Use 'type' field instead."
        });

        AddField(new FieldType
        {
            Name = "assetType",
            ResolvedType = Scalars.NonNullAssetType,
            Resolver = Resolve(x => x.AssetType),
            Description = FieldDescriptions.AssetType
        });

        AddField(new FieldType
        {
            Name = "pixelWidth",
            ResolvedType = Scalars.Int,
            Resolver = Resolve(x => x.Metadata.GetPixelWidth()),
            Description = FieldDescriptions.AssetPixelWidth,
            DeprecationReason = "Use 'metadata' field instead."
        });

        AddField(new FieldType
        {
            Name = "pixelHeight",
            ResolvedType = Scalars.Int,
            Resolver = Resolve(x => x.Metadata.GetPixelHeight()),
            Description = FieldDescriptions.AssetPixelHeight,
            DeprecationReason = "Use 'metadata' field instead."
        });

        AddField(new FieldType
        {
            Name = "metadata",
            Arguments = AssetActions.Metadata.Arguments,
            ResolvedType = Scalars.JsonNoop,
            Resolver = AssetActions.Metadata.Resolver,
            Description = FieldDescriptions.AssetMetadata
        });

        AddField(new FieldType
        {
            Name = "sourceUrl",
            ResolvedType = Scalars.NonNullString,
            Resolver = SourceUrl,
            Description = FieldDescriptions.AssetSourceUrl
        });

        Description = "An asset event";
    }

    private static readonly IFieldResolver Url = Resolve((asset, _, context) =>
    {
        var urlGenerator = context.Resolve<IUrlGenerator>();

        return urlGenerator.AssetContent(asset.AppId, asset.Id.ToString());
    });

    private static readonly IFieldResolver SourceUrl = Resolve((asset, _, context) =>
    {
        var urlGenerator = context.Resolve<IUrlGenerator>();

        return urlGenerator.AssetSource(asset.AppId, asset.Id, asset.FileVersion);
    });

    private static readonly IFieldResolver ThumbnailUrl = Resolve((asset, _, context) =>
    {
        var urlGenerator = context.Resolve<IUrlGenerator>();

        return urlGenerator.AssetThumbnail(asset.AppId, asset.Id.ToString(), asset.AssetType);
    });

    private static IFieldResolver Resolve<T>(Func<EnrichedAssetEvent, IResolveFieldContext, GraphQLExecutionContext, T> resolver)
    {
        return Resolvers.Sync(resolver);
    }

    private static IFieldResolver Resolve<T>(Func<EnrichedAssetEvent, T> resolver)
    {
        return Resolvers.Sync(resolver);
    }
}
