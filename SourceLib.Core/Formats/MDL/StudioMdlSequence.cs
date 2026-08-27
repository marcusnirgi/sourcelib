using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class StudioMdlSeqDesc
{
    public required int BasePtr { get; set; }
    public required int LabelIndex { get; set; }
    public required int ActivityNameIndex { get; set; }
    public required int Flags { get; set; }
    public required int Activity { get; set; }
    public required int ActWeight { get; set; }
    public required int NumEvents { get; set; }
    public required int EventIndex { get; set; }
    public required Vector3 BbMin { get; set; }
    public required Vector3 BbMax { get; set; }
    public required int NumBlends { get; set; }
    public required int AnimIndexIndex { get; set; }
    public required int MovementIndex { get; set; }
    public required int[] GroupSize { get; set; }
    public required int[] ParamIndex { get; set; }
    public required float[] ParamStart { get; set; }
    public required float[] ParamEnd { get; set; }
    public required int ParamParent { get; set; }
    public required float FadeInTime { get; set; }
    public required float FadeOutTime { get; set; }
    public required int LocalEntryNode { get; set; }
    public required int LocalExitNode { get; set; }
    public required int NodeFlags { get; set; }
    public required float EntryPhase { get; set; }
    public required float ExitPhase { get; set; }
    public required float LastFrame { get; set; }
    public required int NextSeq { get; set; }
    public required int Pose { get; set; }
    public required int NumIkRules { get; set; }
    public required int NumAutoLayers { get; set; }
    public required int AutoLayerIndex { get; set; }
    public required int WeightListIndex { get; set; }
    public required int PoseKeyIndex { get; set; }
    public required int NumIkLocks { get; set; }
    public required int IkLockIndex { get; set; }
    public required int KeyValueIndex { get; set; }
    public required int KeyValueSize { get; set; }
    public required int CyclePoseIndex { get; set; }
    public required int ActivityModifierIndex { get; set; }
    public required int NumActivityModifiers { get; set; }
    public required int[] Unused { get; set; }
    public required int Offset { get; set; }
    public required string Label { get; set; }
    public required string ActivityName { get; set; }
    public required IList<StudioMdlEvent> Events { get; set; }
    public required IList<StudioMdlAutoLayer> AutoLayers { get; set; }
    public required IList<StudioMdlIkLock> IkLocks { get; set; }
    public required IList<IList<int>> AnimIndices { get; set; }
    public IList<float> WeightList { get; set; } = [];
    public IList<float> PoseKeys { get; set; } = [];

    public static StudioMdlSeqDesc ReadBinary(BinaryReader reader, int boneCount = 0)
    {
        var baseOffset = reader.BaseStream.Position;
        var basePtr = reader.ReadInt32();
        var labelIndex = reader.ReadInt32();
        var activityNameIndex = reader.ReadInt32();
        var flags = reader.ReadInt32();
        var activity = reader.ReadInt32();
        var actWeight = reader.ReadInt32();
        var numEvents = reader.ReadInt32();
        var eventIndex = reader.ReadInt32();
        var bbMin = Vector3.ReadBinary(reader);
        var bbMax = Vector3.ReadBinary(reader);
        var numBlends = reader.ReadInt32();
        var animIndexIndex = reader.ReadInt32();
        var movementIndex = reader.ReadInt32();
        var groupSize = new int[2] { reader.ReadInt32(), reader.ReadInt32() };
        var paramIndex = new int[2] { reader.ReadInt32(), reader.ReadInt32() };
        var paramStart = new float[2] { reader.ReadSingle(), reader.ReadSingle() };
        var paramEnd = new float[2] { reader.ReadSingle(), reader.ReadSingle() };
        var paramParent = reader.ReadInt32();
        var fadeInTime = reader.ReadSingle();
        var fadeOutTime = reader.ReadSingle();
        var localEntryNode = reader.ReadInt32();
        var localExitNode = reader.ReadInt32();
        var nodeFlags = reader.ReadInt32();
        var entryPhase = reader.ReadSingle();
        var exitPhase = reader.ReadSingle();
        var lastFrame = reader.ReadSingle();
        var nextSeq = reader.ReadInt32();
        var pose = reader.ReadInt32();
        var numIkRules = reader.ReadInt32();
        var numAutoLayers = reader.ReadInt32();
        var autoLayerIndex = reader.ReadInt32();
        var weightListIndex = reader.ReadInt32();
        var poseKeyIndex = reader.ReadInt32();
        var numIkLocks = reader.ReadInt32();
        var ikLockIndex = reader.ReadInt32();
        var keyValueIndex = reader.ReadInt32();
        var keyValueSize = reader.ReadInt32();
        var cyclePoseIndex = reader.ReadInt32();
        var activityModifierIndex = reader.ReadInt32();
        var numActivityModifiers = reader.ReadInt32();
        var unused = new int[5]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };
        var returnPos = reader.BaseStream.Position;
        var label = BinaryReading.ReadStringUntilAt(reader, baseOffset + labelIndex, 0);
        var activityName = BinaryReading.ReadStringUntilAt(
            reader,
            baseOffset + activityNameIndex,
            0
        );

        IList<StudioMdlEvent> events = [];
        if (numEvents > 0 && eventIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + eventIndex;
            events = Enumerable
                .Range(0, numEvents)
                .Select(_ => StudioMdlEvent.ReadBinary(reader))
                .ToList();
        }

        IList<StudioMdlAutoLayer> autoLayers = [];
        if (numAutoLayers > 0 && autoLayerIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + autoLayerIndex;
            autoLayers = Enumerable
                .Range(0, numAutoLayers)
                .Select(_ => StudioMdlAutoLayer.ReadBinary(reader))
                .ToList();
        }

        IList<StudioMdlIkLock> ikLocks = [];
        if (numIkLocks > 0 && ikLockIndex != 0)
        {
            reader.BaseStream.Position = baseOffset + ikLockIndex;
            ikLocks = Enumerable
                .Range(0, numIkLocks)
                .Select(_ => StudioMdlIkLock.ReadBinary(reader))
                .ToList();
        }

        IList<IList<int>> animIndices = [];
        var width = groupSize[0];
        var height = groupSize[1];
        if (animIndexIndex != 0 && width > 0 && height > 0)
        {
            reader.BaseStream.Position = baseOffset + animIndexIndex;
            for (var y = 0; y < height; y++)
            {
                var row = new List<int>(width);
                for (var x = 0; x < width; x++)
                    row.Add(reader.ReadInt16());
                animIndices.Add(row);
            }
        }

        IList<float> weightList = [];
        if (weightListIndex > 0 && boneCount > 0)
        {
            reader.BaseStream.Position = baseOffset + weightListIndex;
            weightList = Enumerable.Range(0, boneCount).Select(_ => reader.ReadSingle()).ToList();
        }

        IList<float> poseKeys = [];
        var numPoseKeys = groupSize[0] + groupSize[1];
        if (poseKeyIndex > 0 && numPoseKeys > 0)
        {
            reader.BaseStream.Position = baseOffset + poseKeyIndex;
            poseKeys = Enumerable.Range(0, numPoseKeys).Select(_ => reader.ReadSingle()).ToList();
        }

        reader.BaseStream.Position = returnPos;

        return new StudioMdlSeqDesc
        {
            BasePtr = basePtr,
            LabelIndex = labelIndex,
            ActivityNameIndex = activityNameIndex,
            Flags = flags,
            Activity = activity,
            ActWeight = actWeight,
            NumEvents = numEvents,
            EventIndex = eventIndex,
            BbMin = bbMin,
            BbMax = bbMax,
            NumBlends = numBlends,
            AnimIndexIndex = animIndexIndex,
            MovementIndex = movementIndex,
            GroupSize = groupSize,
            ParamIndex = paramIndex,
            ParamStart = paramStart,
            ParamEnd = paramEnd,
            ParamParent = paramParent,
            FadeInTime = fadeInTime,
            FadeOutTime = fadeOutTime,
            LocalEntryNode = localEntryNode,
            LocalExitNode = localExitNode,
            NodeFlags = nodeFlags,
            EntryPhase = entryPhase,
            ExitPhase = exitPhase,
            LastFrame = lastFrame,
            NextSeq = nextSeq,
            Pose = pose,
            NumIkRules = numIkRules,
            NumAutoLayers = numAutoLayers,
            AutoLayerIndex = autoLayerIndex,
            WeightListIndex = weightListIndex,
            PoseKeyIndex = poseKeyIndex,
            NumIkLocks = numIkLocks,
            IkLockIndex = ikLockIndex,
            KeyValueIndex = keyValueIndex,
            KeyValueSize = keyValueSize,
            CyclePoseIndex = cyclePoseIndex,
            ActivityModifierIndex = activityModifierIndex,
            NumActivityModifiers = numActivityModifiers,
            Unused = unused,
            Offset = (int)baseOffset,
            Label = label,
            ActivityName = activityName,
            Events = events,
            AutoLayers = autoLayers,
            IkLocks = ikLocks,
            AnimIndices = animIndices,
            WeightList = weightList,
            PoseKeys = poseKeys,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(BasePtr);
        writer.Write(LabelIndex);
        writer.Write(ActivityNameIndex);
        writer.Write(Flags);
        writer.Write(Activity);
        writer.Write(ActWeight);
        writer.Write(NumEvents);
        writer.Write(EventIndex);
        writer.Write(BbMin.X);
        writer.Write(BbMin.Y);
        writer.Write(BbMin.Z);
        writer.Write(BbMax.X);
        writer.Write(BbMax.Y);
        writer.Write(BbMax.Z);
        writer.Write(NumBlends);
        writer.Write(AnimIndexIndex);
        writer.Write(MovementIndex);
        foreach (var g in GroupSize)
            writer.Write(g);
        foreach (var p in ParamIndex)
            writer.Write(p);
        foreach (var p in ParamStart)
            writer.Write(p);
        foreach (var p in ParamEnd)
            writer.Write(p);
        writer.Write(ParamParent);
        writer.Write(FadeInTime);
        writer.Write(FadeOutTime);
        writer.Write(LocalEntryNode);
        writer.Write(LocalExitNode);
        writer.Write(NodeFlags);
        writer.Write(EntryPhase);
        writer.Write(ExitPhase);
        writer.Write(LastFrame);
        writer.Write(NextSeq);
        writer.Write(Pose);
        writer.Write(NumIkRules);
        writer.Write(NumAutoLayers);
        writer.Write(AutoLayerIndex);
        writer.Write(WeightListIndex);
        writer.Write(PoseKeyIndex);
        writer.Write(NumIkLocks);
        writer.Write(IkLockIndex);
        writer.Write(KeyValueIndex);
        writer.Write(KeyValueSize);
        writer.Write(CyclePoseIndex);
        writer.Write(ActivityModifierIndex);
        writer.Write(NumActivityModifiers);
        foreach (var u in Unused)
            writer.Write(u);

        var returnPos = writer.BaseStream.Position;

        if (LabelIndex != 0 && !string.IsNullOrEmpty(Label))
        {
            writer.BaseStream.Position = baseOffset + LabelIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Label));
            writer.Write((byte)0);
        }

        if (ActivityNameIndex != 0 && !string.IsNullOrEmpty(ActivityName))
        {
            writer.BaseStream.Position = baseOffset + ActivityNameIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(ActivityName));
            writer.Write((byte)0);
        }

        if (NumEvents > 0 && EventIndex != 0 && Events != null)
        {
            writer.BaseStream.Position = baseOffset + EventIndex;
            foreach (var evt in Events)
                evt.WriteBinary(writer);
        }

        if (NumAutoLayers > 0 && AutoLayerIndex != 0 && AutoLayers != null)
        {
            writer.BaseStream.Position = baseOffset + AutoLayerIndex;
            foreach (var al in AutoLayers)
                al.WriteBinary(writer);
        }

        if (NumIkLocks > 0 && IkLockIndex != 0 && IkLocks != null)
        {
            writer.BaseStream.Position = baseOffset + IkLockIndex;
            foreach (var ik in IkLocks)
                ik.WriteBinary(writer);
        }

        if (AnimIndexIndex != 0 && AnimIndices != null && AnimIndices.Count > 0)
        {
            writer.BaseStream.Position = baseOffset + AnimIndexIndex;
            for (var y = 0; y < AnimIndices.Count; y++)
            {
                for (var x = 0; x < AnimIndices[y].Count; x++)
                {
                    writer.Write((short)AnimIndices[y][x]);
                }
            }
        }

        if (WeightListIndex > 0 && WeightList != null && WeightList.Count > 0)
        {
            writer.BaseStream.Position = baseOffset + WeightListIndex;
            foreach (var w in WeightList)
                writer.Write(w);
        }

        if (PoseKeyIndex > 0 && PoseKeys != null && PoseKeys.Count > 0)
        {
            writer.BaseStream.Position = baseOffset + PoseKeyIndex;
            foreach (var pk in PoseKeys)
                writer.Write(pk);
        }

        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlEvent
{
    public required float Cycle { get; set; }
    public required int Event { get; set; }
    public required int Type { get; set; }
    public required string Options { get; set; }
    public required int EventIndex { get; set; }
    public required string Name { get; set; }

    public static StudioMdlEvent ReadBinary(BinaryReader reader)
    {
        var baseOffset = reader.BaseStream.Position;
        var cycle = reader.ReadSingle();
        var eventId = reader.ReadInt32();
        var type = reader.ReadInt32();
        var options = new string(reader.ReadChars(64)).TrimEnd('\0');
        var szeventIndex = reader.ReadInt32();
        var returnPos = reader.BaseStream.Position;
        var name = BinaryReading.ReadStringUntilAt(reader, baseOffset + szeventIndex, 0);
        reader.BaseStream.Position = returnPos;
        return new StudioMdlEvent
        {
            Cycle = cycle,
            Event = eventId,
            Type = type,
            Options = options,
            EventIndex = szeventIndex,
            Name = name,
        };
    }

    public void WriteBinary(BinaryWriter writer)
    {
        var baseOffset = writer.BaseStream.Position;
        writer.Write(Cycle);
        writer.Write(Event);
        writer.Write(Type);
        var optionsBuf = (Options + '\0').PadRight(64, '\0')[..64];
        writer.Write(optionsBuf.ToCharArray());
        writer.Write(EventIndex);

        var returnPos = writer.BaseStream.Position;
        if (EventIndex != 0 && !string.IsNullOrEmpty(Name))
        {
            writer.BaseStream.Position = baseOffset + EventIndex;
            writer.Write(System.Text.Encoding.UTF8.GetBytes(Name));
            writer.Write((byte)0);
        }
        writer.BaseStream.Position = returnPos;
    }
}

public sealed class StudioMdlAutoLayer
{
    public required short Sequence { get; set; }
    public required short Pose { get; set; }
    public required int Flags { get; set; }
    public required float Start { get; set; }
    public required float Peak { get; set; }
    public required float Tail { get; set; }
    public required float End { get; set; }

    public static StudioMdlAutoLayer ReadBinary(BinaryReader reader) =>
        new()
        {
            Sequence = reader.ReadInt16(),
            Pose = reader.ReadInt16(),
            Flags = reader.ReadInt32(),
            Start = reader.ReadSingle(),
            Peak = reader.ReadSingle(),
            Tail = reader.ReadSingle(),
            End = reader.ReadSingle(),
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Sequence);
        writer.Write(Pose);
        writer.Write(Flags);
        writer.Write(Start);
        writer.Write(Peak);
        writer.Write(Tail);
        writer.Write(End);
    }
}

public sealed class StudioMdlIkLock
{
    public required int Chain { get; set; }
    public required float PosWeight { get; set; }
    public required float LocalQWeight { get; set; }
    public required int Flags { get; set; }
    public required int[] Unused { get; set; }

    public static StudioMdlIkLock ReadBinary(BinaryReader reader) =>
        new()
        {
            Chain = reader.ReadInt32(),
            PosWeight = reader.ReadSingle(),
            LocalQWeight = reader.ReadSingle(),
            Flags = reader.ReadInt32(),
            Unused = new int[4]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            },
        };

    public void WriteBinary(BinaryWriter writer)
    {
        writer.Write(Chain);
        writer.Write(PosWeight);
        writer.Write(LocalQWeight);
        writer.Write(Flags);
        foreach (var u in Unused)
            writer.Write(u);
    }
}
