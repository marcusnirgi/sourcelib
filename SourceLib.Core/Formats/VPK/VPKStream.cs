using System.IO.Hashing;

namespace SourceLib.Core.Formats.VPK;

public sealed class VPKStream : Stream
{
    private readonly MemoryStream _inner;
    private readonly VPKFile _file;
    private readonly IList<Stream> _chunks;
    private readonly bool _writable;

    internal VPKStream(MemoryStream inner, VPKFile file, IList<Stream> chunks, bool writable)
    {
        _inner = inner;
        _file = file;
        _chunks = chunks;
        _writable = writable;
    }

    public override bool CanRead => _inner.CanRead;

    public override bool CanSeek => _inner.CanSeek;

    public override bool CanWrite => _writable && _inner.CanWrite;

    public override long Length => _inner.Length;

    public override long Position
    {
        get => _inner.Position;
        set => _inner.Position = value;
    }

    public override void Flush()
    {
        _inner.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return _inner.FlushAsync(cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _inner.Read(buffer, offset, count);
    }

    public override int Read(Span<byte> buffer)
    {
        return _inner.Read(buffer);
    }

    public override ValueTask<int> ReadAsync(
        Memory<byte> buffer,
        CancellationToken cancellationToken = default
    )
    {
        return _inner.ReadAsync(buffer, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _inner.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        EnsureWritable();
        _inner.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        EnsureWritable();
        _inner.Write(buffer, offset, count);
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        EnsureWritable();
        _inner.Write(buffer);
    }

    public override Task WriteAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken
    )
    {
        EnsureWritable();
        return _inner.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override ValueTask WriteAsync(
        ReadOnlyMemory<byte> buffer,
        CancellationToken cancellationToken = default
    )
    {
        EnsureWritable();
        return _inner.WriteAsync(buffer, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_writable)
                Commit();

            _inner.Dispose();
        }

        base.Dispose(disposing);
    }

    private void EnsureWritable()
    {
        if (!_writable)
            throw new NotSupportedException("The VPK stream is read-only.");
    }

    private void Commit()
    {
        _inner.Position = 0;

        var data = _inner.ReadAllBytes();

        if (_chunks.Count == 0)
            throw new InvalidOperationException("The VPK has no archive chunks.");

        var preloadSize = Math.Min(
            _file.PreloadSize,
            (ushort)Math.Min(data.Length, ushort.MaxValue)
        );

        var preloadData = data.AsSpan(0, preloadSize).ToArray();
        var chunkData = data.AsSpan(preloadSize).ToArray();

        var parts = new List<VPKFilePart>();

        if (chunkData.Length > 0)
        {
            var chunk = _chunks[0];

            if (!chunk.CanWrite)
                throw new InvalidOperationException("The VPK chunk is not writable.");

            if (chunk.Length > uint.MaxValue)
                throw new InvalidOperationException(
                    "VPK chunk exceeds the maximum representable offset."
                );

            if (chunk.Length + chunkData.Length > uint.MaxValue)
                throw new InvalidOperationException(
                    "VPK chunk exceeds the maximum representable size."
                );

            var offset = chunk.Length;

            chunk.Position = offset;
            chunk.Write(chunkData);

            parts.Add(
                new VPKFilePart
                {
                    FileNumber = 0,
                    Offset = (uint)offset,
                    Size = (uint)chunkData.Length,
                }
            );
        }

        _file.Crc = Crc32.HashToUInt32(data);
        _file.PreloadData = preloadData.ToList();
        _file.PreloadSize = (ushort)preloadData.Length;
        _file.Parts = parts;
    }
}

file static class VPKStreamExtensions
{
    public static byte[] ReadAllBytes(this Stream stream)
    {
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}
