using SourceLib.Core.Engine.Math;

namespace SourceLib.Core.Formats.MDL;

public sealed class MDLFormatParser : IBinaryFormatParser<StudioMdl>
{
    public StudioMdl Parse(byte[] data)
    {
        using var stream = new MemoryStream(data.ToArray());
        using var reader = new BinaryReader(stream);
        return Parse(reader);
    }

    public StudioMdl Parse(BinaryReader reader)
    {

        var idChars = reader.ReadChars(4);

        if (new string(idChars) != StudioMdlHeader.FormatId)
        {
            throw new InvalidDataException($"Invalid header {new string(idChars)}");
        }

        var header = new StudioMdlHeader
        {
            Id = idChars,
            Version = (StudioMdlVersion)reader.ReadInt32(),
            Checksum = reader.ReadInt32(),
            Name = new string(reader.ReadChars(StudioMdlHeader.NameLength)).TrimEnd('\0'),
            Length = reader.ReadInt32(),

            EyePosition = BinaryReading.ReadVector3(reader),
            IlluminationCenter = BinaryReading.ReadVector3(reader),
            HullMin = BinaryReading.ReadVector3(reader),
            HullMax = BinaryReading.ReadVector3(reader),
            ViewBbMin = BinaryReading.ReadVector3(reader),
            ViewBbMax = BinaryReading.ReadVector3(reader),

            Flags = (StudioMdlFlags)reader.ReadInt32(),

            BoneCount = reader.ReadInt32(),
            BoneIndex = reader.ReadInt32(),

            BoneControllerCount = reader.ReadInt32(),
            BoneControllerIndex = reader.ReadInt32(),

            HitboxSetCount = reader.ReadInt32(),
            HitboxSetIndex = reader.ReadInt32(),

            LocalAnimationCount = reader.ReadInt32(),
            LocalAnimationIndex = reader.ReadInt32(),

            LocalSequencesCount = reader.ReadInt32(),
            LocalSequencesIndex = reader.ReadInt32(),

            ActivityListVersion = reader.ReadInt32(),
            EventsIndexed = reader.ReadInt32(),

            TextureCount = reader.ReadInt32(),
            TextureIndex = reader.ReadInt32(),

            TextureDirectoryCount = reader.ReadInt32(),
            TextureDirectoryIndex = reader.ReadInt32(),

            SkinRefCount = reader.ReadInt32(),
            SkinFamilyCount = reader.ReadInt32(),
            SkinIndex = reader.ReadInt32(),

            BodyPartCount = reader.ReadInt32(),
            BodyPartIndex = reader.ReadInt32(),

            LocalAttachmentCount = reader.ReadInt32(),
            LocalAttachmentIndex = reader.ReadInt32(),

            LocalNodeCount = reader.ReadInt32(),
            LocalNodeIndex = reader.ReadInt32(),
            LocalNodeNameIndex = reader.ReadInt32(),

            FlexDescCount = reader.ReadInt32(),
            FlexDescIndex = reader.ReadInt32(),

            FlexControllerCount = reader.ReadInt32(),
            FlexControllerIndex = reader.ReadInt32(),

            FlexRuleCount = reader.ReadInt32(),
            FlexRuleIndex = reader.ReadInt32(),

            IkChainCount = reader.ReadInt32(),
            IkChainIndex = reader.ReadInt32(),

            MouthCount = reader.ReadInt32(),
            MouthIndex = reader.ReadInt32(),

            LocalPoseParameterCount = reader.ReadInt32(),
            LocalPoseParameterIndex = reader.ReadInt32(),

            SurfacePropIndex = reader.ReadInt32(),

            KeyValueIndex = reader.ReadInt32(),
            KeyValueSize = reader.ReadInt32(),

            LocalIkAutoplayLockCount = reader.ReadInt32(),
            LocalIkAutoplayLockIndex = reader.ReadInt32(),

            Mass = reader.ReadSingle(),
            Contents = reader.ReadInt32(),

            IncludeModelCount = reader.ReadInt32(),
            IncludeModelIndex = reader.ReadInt32(),

            UnusedVirtualModel = reader.ReadInt32(),

            AnimationBlockNameIndex = reader.ReadInt32(),
            AnimationBlockCount = reader.ReadInt32(),
            AnimationBlockIndex = reader.ReadInt32(),

            UnusedAnimationBlockModel = reader.ReadInt32(),

            BoneTableByNameIndex = reader.ReadInt32(),

            UnusedVertexBase = reader.ReadInt32(),
            UnusedIndexBase = reader.ReadInt32(),

            ConstantDirectionalLightDot = reader.ReadByte(),
            RootLod = reader.ReadByte(),
            NumAllowedRootLods = reader.ReadByte(),
            Unused = reader.ReadByte(),

            Unused4 = reader.ReadInt32(),

            FlexControllerUiCount = reader.ReadInt32(),
            FlexControllerUiIndex = reader.ReadInt32(),

            VertAnimFixedPointScale = reader.ReadSingle(),

            Unused3 = reader.ReadInt32(),

            StudioHdr2Index = reader.ReadInt32(),

            Unused2 = reader.ReadInt32(),
        };

        // Read uncovered strings and data referenced by the header
        var surfaceProp = ReadStringAt(reader, 0, header.SurfacePropIndex);
        var keyValues = ReadKeyValues(reader, header.KeyValueIndex, header.KeyValueSize);
        var boneTableByName = ReadBoneTableByName(reader, header.BoneTableByNameIndex, header.BoneCount);

        StudioMdlHeader2? header2 = null;

        if (header.StudioHdr2Index != 0)
        {
            header2 = ParseHeader2(reader, header.StudioHdr2Index);
        }

        var bones = ParseBones(reader, header.BoneIndex, header.BoneCount);
        var textures = ParseTextures(reader, header.TextureIndex, header.TextureCount);
        var textureDirectories = ParseTextureDirectories(
            reader,
            header.TextureDirectoryIndex,
            header.TextureDirectoryCount
        );
        var skins = ParseSkins(reader, header.SkinIndex, header.SkinRefCount, header.SkinFamilyCount);
        var bodyParts = ParseBodyParts(reader, header.BodyPartIndex, header.BodyPartCount);
        var hitboxSets = ParseHitboxSets(reader, header.HitboxSetIndex, header.HitboxSetCount);
        var attachments = ParseAttachments(reader, header.LocalAttachmentIndex, header.LocalAttachmentCount);
        var boneControllers = ParseBoneControllers(reader, header.BoneControllerIndex, header.BoneControllerCount);
        var flexDescs = ParseFlexDescs(reader, header.FlexDescIndex, header.FlexDescCount);
        var flexControllers = ParseFlexControllers(reader, header.FlexControllerIndex, header.FlexControllerCount);
        var flexRules = ParseFlexRules(reader, header.FlexRuleIndex, header.FlexRuleCount);
        var flexControllerUi = ParseFlexControllerUi(reader, header.FlexControllerUiIndex, header.FlexControllerUiCount);
        var ikChains = ParseIkChains(reader, header.IkChainIndex, header.IkChainCount);
        var poseParameters = ParsePoseParameters(reader, header.LocalPoseParameterIndex, header.LocalPoseParameterCount);
        var mouths = ParseMouths(reader, header.MouthIndex, header.MouthCount);
        var includeModels = ParseIncludeModels(reader, header.IncludeModelIndex, header.IncludeModelCount);
        var animBlocks = ParseAnimBlocks(reader, header.AnimationBlockIndex, header.AnimationBlockCount);
        var animations = ParseAnimations(reader, header.LocalAnimationIndex, header.LocalAnimationCount);
        var sequences = ParseSequences(reader, header.LocalSequencesIndex, header.LocalSequencesCount);

        StudioMdlLinearBone? linearBones = null;
        if (header2 is not null)
        {
            linearBones = ParseLinearBones(reader, header.StudioHdr2Index, header2);
        }

        var sourceBoneTransforms = ParseSourceBoneTransforms(reader, header2);
        var boneFlexDrivers = ParseBoneFlexDrivers(reader, header.StudioHdr2Index, header2);

        return new StudioMdl(
            header,
            header2,
            bones,
            textures,
            textureDirectories,
            skins,
            bodyParts,
            hitboxSets,
            attachments,
            boneControllers,
            flexDescs,
            flexControllers,
            flexRules,
            flexControllerUi,
            ikChains,
            poseParameters,
            mouths,
            includeModels,
            animBlocks,
            animations,
            sequences,
            linearBones,
            sourceBoneTransforms,
            boneFlexDrivers,
            surfaceProp,
            keyValues,
            boneTableByName
        );
    }

    private string ReadString(BinaryReader reader, long offset)
    {
        if (offset <= 0 || offset >= reader.BaseStream.Length)
        {
            return string.Empty;
        }

        reader.BaseStream.Position = offset;

        return BinaryReading.ReadStringUntil(reader, 0);
    }

    private string ReadStringAt(BinaryReader reader, long baseOffset, int stringIndex)
    {
        if (stringIndex <= 0)
            return string.Empty;
        return ReadString(reader, baseOffset + stringIndex);
    }

    private string ReadKeyValues(BinaryReader reader, int keyValueIndex, int keyValueSize)
    {
        if (keyValueIndex <= 0 || keyValueSize <= 0)
            return string.Empty;

        reader.BaseStream.Position = keyValueIndex;
        return new string(reader.ReadChars(keyValueSize)).TrimEnd('\0');
    }

    private byte[] ReadBoneTableByName(BinaryReader reader, int boneTableByNameIndex, int boneCount)
    {
        if (boneTableByNameIndex <= 0 || boneCount <= 0)
            return [];

        reader.BaseStream.Position = boneTableByNameIndex;
        return reader.ReadBytes(boneCount);
    }

    private StudioMdlHeader2 ParseHeader2(BinaryReader reader, int headerOffset)
    {
        reader.BaseStream.Position = headerOffset;

        return new StudioMdlHeader2
        {
            SourceBoneTransformCount = reader.ReadInt32(),
            SourceBoneTransformIndex = reader.ReadInt32(),
            IlluminationPositionAttachmentIndex = reader.ReadInt32(),
            MaxEyeDeflection = reader.ReadSingle(),
            LinearBoneIndex = reader.ReadInt32(),
            NameIndex = reader.ReadInt32(),
            BoneFlexDriverCount = reader.ReadInt32(),
            BoneFlexDriverIndex = reader.ReadInt32(),

            Reserved = Enumerable
                .Range(0, StudioMdlHeader2.ReservedCount)
                .Select(_ => reader.ReadInt32())
                .ToArray(),
        };
    }

    private List<StudioMdlBone> ParseBones(BinaryReader reader, int bonesOffset, int boneCount)
    {
        var bones = new List<StudioMdlBone>(boneCount);

        reader.BaseStream.Position = bonesOffset;

        for (var i = 0; i < boneCount; i++)
        {
            var boneOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var parent = reader.ReadInt32();

            var boneController = new List<int>
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var position = BinaryReading.ReadVector3(reader);
            var quaternion = BinaryReading.ReadQuaternion(reader);
            var rotation = BinaryReading.ReadVector3(reader);
            var positionScale = BinaryReading.ReadVector3(reader);
            var rotationScale = BinaryReading.ReadVector3(reader);
            var poseToBone = BinaryReading.ReadMatrix(reader);
            var alignmentQuaternion = BinaryReading.ReadQuaternion(reader);

            var flags = (StudioMdlBoneFlags)reader.ReadInt32();

            var procedureType = reader.ReadInt32();
            var procedureIndex = reader.ReadInt32();
            var physicsBone = reader.ReadInt32();
            var surfacePropIndex = reader.ReadInt32();
            var contents = reader.ReadInt32();

            var unused = new List<int>
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var returnPosition = reader.BaseStream.Position;

            var name = ReadString(reader, boneOffset + nameIndex);
            var surfaceProp =
                surfacePropIndex != 0
                    ? ReadString(reader, boneOffset + surfacePropIndex)
                    : string.Empty;

            IStudioMdlBoneProcedure? procedural = null;

            if (procedureIndex != 0)
            {
                reader.BaseStream.Position = boneOffset + procedureIndex;
                procedural = ParseProceduralBone(reader, procedureType);
            }

            reader.BaseStream.Position = returnPosition;

            bones.Add(
                new StudioMdlBone
                {
                    NameIndex = nameIndex,
                    Parent = parent,
                    BoneController = boneController.ToArray(),

                    Position = position,
                    Quaternion = quaternion,
                    Rotation = rotation,
                    PositionScale = positionScale,
                    RotationScale = rotationScale,
                    PoseToBone = poseToBone,
                    AlignmentQuaternion = alignmentQuaternion,
                    Flags = flags,

                    ProcedureType = procedureType,
                    ProcedureIndex = procedureIndex,
                    PhysicsBone = physicsBone,
                    SurfacePropIndex = surfacePropIndex,
                    Contents = contents,

                    Unused = unused.ToArray(),

                    Name = name,
                    SurfaceProp = surfaceProp,
                    Procedural = procedural,
                }
            );
        }

        return bones;
    }

    private IStudioMdlBoneProcedure ParseProceduralBone(BinaryReader reader, int procedureType)
    {
        return procedureType switch
        {
            1 => ParseAxisInterpBone(reader),
            2 => ParseQuatInterpBone(reader),
            3 => ParseAimAtBone(reader),
            4 => ParseAimAtBone(reader),
            5 => ParseJiggleBone(reader),
            _ => throw new InvalidDataException($"Unknown bone procedure type: {procedureType}"),
        };
    }

    private StudioMdlAxisInterpBone ParseAxisInterpBone(BinaryReader reader)
    {
        return new StudioMdlAxisInterpBone
        {
            Control = reader.ReadInt32(),
            Axis = reader.ReadInt32(),

            Position =
            [
                BinaryReading.ReadVector3(reader),
                BinaryReading.ReadVector3(reader),
                BinaryReading.ReadVector3(reader),
                BinaryReading.ReadVector3(reader),
                BinaryReading.ReadVector3(reader),
                BinaryReading.ReadVector3(reader),
            ],

            Quaternion =
            [
                BinaryReading.ReadQuaternion(reader),
                BinaryReading.ReadQuaternion(reader),
                BinaryReading.ReadQuaternion(reader),
                BinaryReading.ReadQuaternion(reader),
                BinaryReading.ReadQuaternion(reader),
                BinaryReading.ReadQuaternion(reader),
            ],
        };
    }

    private StudioMdlQuatInterpBone ParseQuatInterpBone(BinaryReader reader)
    {
        var boneOffset = reader.BaseStream.Position;

        var bone = new StudioMdlQuatInterpBone
        {
            Control = reader.ReadInt32(),
            TriggerCount = reader.ReadInt32(),
            TriggerIndex = reader.ReadInt32(),
            Triggers = [],
        };

        if (bone.TriggerCount > 0 && bone.TriggerIndex != 0)
        {
            var returnPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = boneOffset + bone.TriggerIndex;

            bone.Triggers =
            [
                .. Enumerable
                    .Range(0, bone.TriggerCount)
                    .Select(_ => new StudioMdlQuatInterpInfo
                    {
                        InverseTolerance = reader.ReadSingle(),
                        Trigger = BinaryReading.ReadQuaternion(reader),
                        Position = BinaryReading.ReadVector3(reader),
                        Quaternion = BinaryReading.ReadQuaternion(reader),
                    }),
            ];

            reader.BaseStream.Position = returnPosition;
        }

        return bone;
    }

    private StudioMdlAimAtBone ParseAimAtBone(BinaryReader reader)
    {
        return new StudioMdlAimAtBone
        {
            Parent = reader.ReadInt32(),
            Aim = reader.ReadInt32(),
            AimVector = BinaryReading.ReadVector3(reader),
            UpVector = BinaryReading.ReadVector3(reader),
            BasePosition = BinaryReading.ReadVector3(reader),
        };
    }

    private StudioMdlJiggleBone ParseJiggleBone(BinaryReader reader)
    {
        return new StudioMdlJiggleBone
        {
            Flags = reader.ReadInt32(),
            Length = reader.ReadSingle(),
            TipMass = reader.ReadSingle(),
            YawStiffness = reader.ReadSingle(),
            YawDamping = reader.ReadSingle(),
            PitchStiffness = reader.ReadSingle(),
            PitchDamping = reader.ReadSingle(),
            AlongStiffness = reader.ReadSingle(),
            AlongDamping = reader.ReadSingle(),
            AngleLimit = reader.ReadSingle(),
            MinYaw = reader.ReadSingle(),
            MaxYaw = reader.ReadSingle(),
            YawFriction = reader.ReadSingle(),
            YawBounce = reader.ReadSingle(),
            MinPitch = reader.ReadSingle(),
            MaxPitch = reader.ReadSingle(),
            PitchFriction = reader.ReadSingle(),
            PitchBounce = reader.ReadSingle(),
            BaseMass = reader.ReadSingle(),
            BaseStiffness = reader.ReadSingle(),
            BaseDamping = reader.ReadSingle(),
            BaseMinLeft = reader.ReadSingle(),
            BaseMaxLeft = reader.ReadSingle(),
            BaseLeftFriction = reader.ReadSingle(),
            BaseMinUp = reader.ReadSingle(),
            BaseMaxUp = reader.ReadSingle(),
            BaseUpFriction = reader.ReadSingle(),
            BaseMinForward = reader.ReadSingle(),
            BaseMaxForward = reader.ReadSingle(),
            BaseForwardFriction = reader.ReadSingle(),
            BoingImpactSpeed = reader.ReadSingle(),
            BoingImpactAngle = reader.ReadSingle(),
            BoingDampingRate = reader.ReadSingle(),
            BoingFrequency = reader.ReadSingle(),
            BoingAmplitude = reader.ReadSingle(),
        };
    }

    private List<StudioMdlTexture> ParseTextures(
        BinaryReader reader,
        int texturesOffset,
        int textureCount
    )
    {
        var textures = new List<StudioMdlTexture>(textureCount);

        reader.BaseStream.Position = texturesOffset;

        for (var i = 0; i < textureCount; i++)
        {
            var textureOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var flags = reader.ReadInt32();
            var used = reader.ReadInt32();
            var unused1 = reader.ReadInt32();
            var material = reader.ReadInt32();
            var clientMaterial = reader.ReadInt32();

            var unused = new List<int>
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var returnPosition = reader.BaseStream.Position;
            var name = ReadString(reader, textureOffset + nameIndex);

            reader.BaseStream.Position = returnPosition;

            textures.Add(
                new StudioMdlTexture
                {
                    NameIndex = nameIndex,
                    Name = name,
                    Flags = flags,
                    Used = used,
                    Unused1 = unused1,
                    Material = material,
                    ClientMaterial = clientMaterial,
                    Unused = unused.ToArray(),
                }
            );
        }

        return textures;
    }

    private List<string> ParseTextureDirectories(
        BinaryReader reader,
        int textureDirectoriesOffset,
        int textureDirectoryCount
    )
    {
        var textureDirectories = new List<string>(textureDirectoryCount);

        reader.BaseStream.Position = textureDirectoriesOffset;

        for (var i = 0; i < textureDirectoryCount; i++)
        {
            var textureNameOffset = reader.ReadInt32();
            var returnPosition = reader.BaseStream.Position;

            var textureName = ReadString(reader, textureNameOffset);

            reader.BaseStream.Position = returnPosition;

            textureDirectories.Add(textureName);
        }

        return textureDirectories;
    }

    private List<int[]> ParseSkins(
        BinaryReader reader,
        int skinIndex,
        int skinRefCount,
        int skinFamilyCount
    )
    {
        var skins = new List<int[]>(skinFamilyCount);

        reader.BaseStream.Position = skinIndex;

        for (var family = 0; family < skinFamilyCount; family++)
        {
            var skin = new int[skinRefCount];

            for (var i = 0; i < skinRefCount; i++)
            {
                skin[i] = reader.ReadInt16();
            }

            skins.Add(skin);
        }

        return skins;
    }

    private List<StudioMdlBodyPart> ParseBodyParts(
        BinaryReader reader,
        int bodyPartIndex,
        int bodyPartCount
    )
    {
        var bodyParts = new List<StudioMdlBodyPart>(bodyPartCount);

        reader.BaseStream.Position = bodyPartIndex;

        for (var i = 0; i < bodyPartCount; i++)
        {
            var bodyPartOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var modelCount = reader.ReadInt32();
            var baseValue = reader.ReadInt32();
            var modelIndex = reader.ReadInt32();

            var returnPosition = reader.BaseStream.Position;
            var name = ReadString(reader, bodyPartOffset + nameIndex);

            reader.BaseStream.Position = bodyPartOffset + modelIndex;

            var models = new List<StudioMdlModel>(modelCount);

            for (var j = 0; j < modelCount; j++)
            {
                var modelOffset = reader.BaseStream.Position;

                var modelName = new string(reader.ReadChars(64)).TrimEnd('\0');
                var type = reader.ReadInt32();
                var boundingRadius = reader.ReadSingle();

                var meshCount = reader.ReadInt32();
                var meshIndex = reader.ReadInt32();

                var vertexCount = reader.ReadInt32();
                var vertexIndex = reader.ReadInt32();
                var tangentIndex = reader.ReadInt32();

                var attachmentCount = reader.ReadInt32();
                var attachmentIndex = reader.ReadInt32();

                var eyeballCount = reader.ReadInt32();
                var eyeballIndex = reader.ReadInt32();

                var vertexData = new int[4]
                {
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                };

                var unused = new int[6]
                {
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                };

                var modelReturnPosition = reader.BaseStream.Position;

                reader.BaseStream.Position = modelOffset + meshIndex;

                var meshes = ParseMeshes(reader, meshCount);

                reader.BaseStream.Position = modelOffset + eyeballIndex;

                var eyeballs = ParseEyeballs(reader, eyeballCount, modelOffset);

                reader.BaseStream.Position = modelReturnPosition;

                models.Add(
                    new StudioMdlModel
                    {
                        Name = modelName,
                        Type = type,
                        BoundingRadius = boundingRadius,

                        MeshCount = meshCount,
                        MeshIndex = meshIndex,

                        VertexCount = vertexCount,
                        VertexIndex = vertexIndex,
                        TangentIndex = tangentIndex,

                        AttachmentCount = attachmentCount,
                        AttachmentIndex = attachmentIndex,

                        EyeballCount = eyeballCount,
                        EyeballIndex = eyeballIndex,

                        VertexData = vertexData,
                        Unused = unused,

                        Meshes = meshes,
                        Eyeballs = eyeballs,
                    }
                );
            }

            reader.BaseStream.Position = returnPosition;

            bodyParts.Add(
                new StudioMdlBodyPart
                {
                    NameIndex = nameIndex,
                    ModelCount = modelCount,
                    Base = baseValue,
                    ModelIndex = modelIndex,
                    Name = name,
                    Models = models,
                }
            );

            reader.BaseStream.Position = bodyPartOffset + 16;
        }

        return bodyParts;
    }

    private List<StudioMdlMesh> ParseMeshes(BinaryReader reader, int meshCount)
    {
        var meshes = new List<StudioMdlMesh>(meshCount);

        for (var i = 0; i < meshCount; i++)
        {
            var material = reader.ReadInt32();
            var modelIndex = reader.ReadInt32();
            var numVertices = reader.ReadInt32();
            var vertexOffset = reader.ReadInt32();
            var numFlexes = reader.ReadInt32();
            var flexIndex = reader.ReadInt32();
            var materialType = reader.ReadInt32();
            var materialParam = reader.ReadInt32();
            var meshId = reader.ReadInt32();
            var center = BinaryReading.ReadVector3(reader);

            // mstudio_meshvertexdata_t: runtime pointer + 8 LOD vertex counts
            var vertexData = new int[9]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var unused = new int[8]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var meshOffset = reader.BaseStream.Position - 120;

            IList<StudioMdlFlex> flexes = [];

            if (numFlexes > 0 && flexIndex != 0)
            {
                flexes = ParseFlexes(reader, (int)(meshOffset + flexIndex), numFlexes);
            }

            meshes.Add(
                new StudioMdlMesh
                {
                    Material = material,
                    ModelIndex = modelIndex,
                    NumVertices = numVertices,
                    VertexOffset = vertexOffset,
                    NumFlexes = numFlexes,
                    FlexIndex = flexIndex,
                    MaterialType = materialType,
                    MaterialParam = materialParam,
                    MeshId = meshId,
                    Center = center,
                    VertexData = vertexData,
                    Unused = unused,
                    Flexes = flexes,
                }
            );
        }

        return meshes;
    }

    private List<StudioMdlFlex> ParseFlexes(BinaryReader reader, int flexOffset, int flexCount)
    {
        var flexes = new List<StudioMdlFlex>(flexCount);

        reader.BaseStream.Position = flexOffset;

        for (var i = 0; i < flexCount; i++)
        {
            var flexOffsetStart = reader.BaseStream.Position;

            var flexdesc = reader.ReadInt32();
            var target0 = reader.ReadSingle();
            var target1 = reader.ReadSingle();
            var target2 = reader.ReadSingle();
            var target3 = reader.ReadSingle();
            var numVerts = reader.ReadInt32();
            var vertIndex = reader.ReadInt32();
            var flexPair = reader.ReadInt32();
            var vertAnimType = reader.ReadByte();
            var unusedChar = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            var unused = new int[6]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            flexes.Add(
                new StudioMdlFlex
                {
                    FlexDesc = flexdesc,
                    Target0 = target0,
                    Target1 = target1,
                    Target2 = target2,
                    Target3 = target3,
                    NumVerts = numVerts,
                    VertIndex = vertIndex,
                    FlexPair = flexPair,
                    VertAnimType = vertAnimType,
                    UnusedChar = unusedChar,
                    Unused = unused,
                }
            );
        }

        return flexes;
    }

    private List<StudioMdlEyeball> ParseEyeballs(BinaryReader reader, int eyeballCount, long modelOffset)
    {
        var eyeballs = new List<StudioMdlEyeball>(eyeballCount);

        for (var i = 0; i < eyeballCount; i++)
        {
            var eyeballOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var bone = reader.ReadInt32();
            var org = BinaryReading.ReadVector3(reader);
            var zOffset = reader.ReadSingle();
            var radius = reader.ReadSingle();
            var up = BinaryReading.ReadVector3(reader);
            var forward = BinaryReading.ReadVector3(reader);
            var texture = reader.ReadInt32();
            var unused1 = reader.ReadInt32();
            var irisScale = reader.ReadSingle();
            var unused2 = reader.ReadInt32();

            var upperFlexDesc = new int[3]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var lowerFlexDesc = new int[3]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var upperTarget = new float[3]
            {
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
            };

            var lowerTarget = new float[3]
            {
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle(),
            };

            var upperLidFlexDesc = reader.ReadInt32();
            var lowerLidFlexDesc = reader.ReadInt32();

            var unused = new int[4]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var nonFacs = reader.ReadByte();
            var unused3 = new byte[] { reader.ReadByte(), reader.ReadByte(), reader.ReadByte() };
            var unused4 = new int[7]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, eyeballOffset, nameIndex);
            reader.BaseStream.Position = returnPosition;

            eyeballs.Add(
                new StudioMdlEyeball
                {
                    NameIndex = nameIndex,
                    Bone = bone,
                    Org = org,
                    ZOffset = zOffset,
                    Radius = radius,
                    Up = up,
                    Forward = forward,
                    Texture = texture,
                    Unused1 = unused1,
                    IrisScale = irisScale,
                    Unused2 = unused2,
                    UpperFlexDesc = upperFlexDesc,
                    LowerFlexDesc = lowerFlexDesc,
                    UpperTarget = upperTarget,
                    LowerTarget = lowerTarget,
                    UpperLidFlexDesc = upperLidFlexDesc,
                    LowerLidFlexDesc = lowerLidFlexDesc,
                    Unused = unused,
                    NonFacs = nonFacs,
                    Unused3 = unused3,
                    Unused4 = unused4,
                    Name = name,
                }
            );
        }

        return eyeballs;
    }

    private List<StudioMdlHitboxSet> ParseHitboxSets(
        BinaryReader reader,
        int hitboxSetIndex,
        int hitboxSetCount
    )
    {
        var hitboxSets = new List<StudioMdlHitboxSet>(hitboxSetCount);

        reader.BaseStream.Position = hitboxSetIndex;

        for (var i = 0; i < hitboxSetCount; i++)
        {
            var hitboxSetOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var hitboxCount = reader.ReadInt32();
            var hitboxIndex = reader.ReadInt32();

            var returnPosition = reader.BaseStream.Position;

            var name = ReadString(reader, hitboxSetOffset + nameIndex);

            reader.BaseStream.Position = hitboxSetOffset + hitboxIndex;

            var hitboxes = new List<StudioMdlHitbox>(hitboxCount);

            for (var j = 0; j < hitboxCount; j++)
            {
                var hitboxOffset = reader.BaseStream.Position;

                var bone = reader.ReadInt32();
                var group = reader.ReadInt32();
                var min = BinaryReading.ReadVector3(reader);
                var max = BinaryReading.ReadVector3(reader);
                var hitboxNameIndex = reader.ReadInt32();

                var unused = new int[8]
                {
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                    reader.ReadInt32(),
                };

                var hitboxName =
                    hitboxNameIndex != 0
                        ? ReadString(reader, hitboxOffset + hitboxNameIndex)
                        : string.Empty;

                reader.BaseStream.Position = hitboxOffset + 68;

                hitboxes.Add(
                    new StudioMdlHitbox
                    {
                        Bone = bone,
                        Group = group,
                        Min = min,
                        Max = max,
                        NameIndex = hitboxNameIndex,
                        Name = hitboxName,
                        Unused = unused,
                    }
                );
            }

            reader.BaseStream.Position = returnPosition;

            hitboxSets.Add(
                new StudioMdlHitboxSet
                {
                    NameIndex = nameIndex,
                    HitboxCount = hitboxCount,
                    HitboxIndex = hitboxIndex,
                    Name = name,
                    Hitboxes = hitboxes,
                }
            );

            reader.BaseStream.Position = hitboxSetOffset + 12;
        }

        return hitboxSets;
    }

    private List<StudioMdlAttachment> ParseAttachments(
        BinaryReader reader,
        int attachmentIndex,
        int attachmentCount
    )
    {
        var attachments = new List<StudioMdlAttachment>(attachmentCount);

        reader.BaseStream.Position = attachmentIndex;

        for (var i = 0; i < attachmentCount; i++)
        {
            var attachmentOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var flags = reader.ReadUInt32();
            var localBone = reader.ReadInt32();
            var local = BinaryReading.ReadMatrix(reader);

            var unused = new int[8]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var returnPosition = reader.BaseStream.Position;

            var name = ReadString(reader, attachmentOffset + nameIndex);

            reader.BaseStream.Position = returnPosition;

            attachments.Add(
                new StudioMdlAttachment
                {
                    NameIndex = nameIndex,
                    Flags = flags,
                    LocalBone = localBone,
                    Local = local,
                    Unused = unused,
                    Name = name,
                }
            );
        }

        return attachments;
    }

    private List<StudioMdlBoneController> ParseBoneControllers(
        BinaryReader reader,
        int boneControllerIndex,
        int boneControllerCount
    )
    {
        var controllers = new List<StudioMdlBoneController>(boneControllerCount);

        if (boneControllerCount == 0 || boneControllerIndex == 0)
            return controllers;

        reader.BaseStream.Position = boneControllerIndex;

        for (var i = 0; i < boneControllerCount; i++)
        {
            controllers.Add(
                new StudioMdlBoneController
                {
                    Bone = reader.ReadInt32(),
                    Type = reader.ReadInt32(),
                    Start = reader.ReadSingle(),
                    End = reader.ReadSingle(),
                    Rest = reader.ReadInt32(),
                    InputField = reader.ReadInt32(),
                    Unused = Enumerable.Range(0, 8).Select(_ => reader.ReadInt32()).ToArray(),
                }
            );
        }

        return controllers;
    }

    private List<StudioMdlFlexDesc> ParseFlexDescs(
        BinaryReader reader,
        int flexDescIndex,
        int flexDescCount
    )
    {
        var descs = new List<StudioMdlFlexDesc>(flexDescCount);

        if (flexDescCount == 0 || flexDescIndex == 0)
            return descs;

        reader.BaseStream.Position = flexDescIndex;

        for (var i = 0; i < flexDescCount; i++)
        {
            var descOffset = reader.BaseStream.Position;
            var facsIndex = reader.ReadInt32();
            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, descOffset, facsIndex);
            reader.BaseStream.Position = returnPosition;

            descs.Add(
                new StudioMdlFlexDesc
                {
                    FacsIndex = facsIndex,
                    Name = name,
                }
            );
        }

        return descs;
    }

    private List<StudioMdlFlexController> ParseFlexControllers(
        BinaryReader reader,
        int flexControllerIndex,
        int flexControllerCount
    )
    {
        var controllers = new List<StudioMdlFlexController>(flexControllerCount);

        if (flexControllerCount == 0 || flexControllerIndex == 0)
            return controllers;

        reader.BaseStream.Position = flexControllerIndex;

        for (var i = 0; i < flexControllerCount; i++)
        {
            var controllerOffset = reader.BaseStream.Position;

            var typeIndex = reader.ReadInt32();
            var nameIndex = reader.ReadInt32();
            var localToGlobal = reader.ReadInt32();
            var min = reader.ReadSingle();
            var max = reader.ReadSingle();

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, controllerOffset, nameIndex);
            var type = ReadStringAt(reader, controllerOffset, typeIndex);
            reader.BaseStream.Position = returnPosition;

            controllers.Add(
                new StudioMdlFlexController
                {
                    TypeIndex = typeIndex,
                    NameIndex = nameIndex,
                    LocalToGlobal = localToGlobal,
                    Min = min,
                    Max = max,
                    Name = name,
                    Type = type,
                }
            );
        }

        return controllers;
    }

    private List<StudioMdlFlexRule> ParseFlexRules(
        BinaryReader reader,
        int flexRuleIndex,
        int flexRuleCount
    )
    {
        var rules = new List<StudioMdlFlexRule>(flexRuleCount);

        if (flexRuleCount == 0 || flexRuleIndex == 0)
            return rules;

        reader.BaseStream.Position = flexRuleIndex;

        for (var i = 0; i < flexRuleCount; i++)
        {
            var ruleOffset = reader.BaseStream.Position;

            var flex = reader.ReadInt32();
            var numOps = reader.ReadInt32();
            var opIndex = reader.ReadInt32();

            IList<StudioMdlFlexOp> ops = [];

            if (numOps > 0 && opIndex != 0)
            {
                var returnPosition = reader.BaseStream.Position;
                reader.BaseStream.Position = ruleOffset + opIndex;

                ops = Enumerable
                    .Range(0, numOps)
                    .Select(_ => new StudioMdlFlexOp
                    {
                        Op = reader.ReadInt32(),
                        D = reader.ReadInt32(),
                    })
                    .ToList();

                reader.BaseStream.Position = returnPosition;
            }

            rules.Add(
                new StudioMdlFlexRule
                {
                    Flex = flex,
                    NumOps = numOps,
                    OpIndex = opIndex,
                    Ops = ops,
                }
            );
        }

        return rules;
    }

    private List<StudioMdlFlexControllerUi> ParseFlexControllerUi(
        BinaryReader reader,
        int flexControllerUiIndex,
        int flexControllerUiCount
    )
    {
        var uis = new List<StudioMdlFlexControllerUi>(flexControllerUiCount);

        if (flexControllerUiCount == 0 || flexControllerUiIndex == 0)
            return uis;

        reader.BaseStream.Position = flexControllerUiIndex;

        for (var i = 0; i < flexControllerUiCount; i++)
        {
            var uiOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var index0 = reader.ReadInt32();
            var index1 = reader.ReadInt32();
            var index2 = reader.ReadInt32();
            var remapType = reader.ReadByte();
            var stereo = reader.ReadByte();
            var unused = new byte[] { reader.ReadByte(), reader.ReadByte() };

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, uiOffset, nameIndex);
            reader.BaseStream.Position = returnPosition;

            uis.Add(
                new StudioMdlFlexControllerUi
                {
                    NameIndex = nameIndex,
                    Index0 = index0,
                    Index1 = index1,
                    Index2 = index2,
                    RemapType = remapType,
                    Stereo = stereo,
                    Unused = unused,
                    Name = name,
                }
            );
        }

        return uis;
    }

    private List<StudioMdlIkChain> ParseIkChains(
        BinaryReader reader,
        int ikChainIndex,
        int ikChainCount
    )
    {
        var chains = new List<StudioMdlIkChain>(ikChainCount);

        if (ikChainCount == 0 || ikChainIndex == 0)
            return chains;

        reader.BaseStream.Position = ikChainIndex;

        for (var i = 0; i < ikChainCount; i++)
        {
            var chainOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var linkType = reader.ReadInt32();
            var numLinks = reader.ReadInt32();
            var linkIndex = reader.ReadInt32();

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, chainOffset, nameIndex);

            IList<StudioMdlIkLink> links = [];

            if (numLinks > 0 && linkIndex != 0)
            {
                reader.BaseStream.Position = chainOffset + linkIndex;

                links = Enumerable
                    .Range(0, numLinks)
                    .Select(_ => new StudioMdlIkLink
                    {
                        Bone = reader.ReadInt32(),
                        KneeDir = BinaryReading.ReadVector3(reader),
                        Unused0 = BinaryReading.ReadVector3(reader),
                    })
                    .ToList();
            }

            reader.BaseStream.Position = returnPosition;

            chains.Add(
                new StudioMdlIkChain
                {
                    NameIndex = nameIndex,
                    LinkType = linkType,
                    NumLinks = numLinks,
                    LinkIndex = linkIndex,
                    Links = links,
                    Name = name,
                }
            );
        }

        return chains;
    }

    private List<StudioMdlPoseParamDesc> ParsePoseParameters(
        BinaryReader reader,
        int poseParamIndex,
        int poseParamCount
    )
    {
        var parameters = new List<StudioMdlPoseParamDesc>(poseParamCount);

        if (poseParamCount == 0 || poseParamIndex == 0)
            return parameters;

        reader.BaseStream.Position = poseParamIndex;

        for (var i = 0; i < poseParamCount; i++)
        {
            var paramOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var flags = reader.ReadInt32();
            var start = reader.ReadSingle();
            var end = reader.ReadSingle();
            var loop = reader.ReadSingle();

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, paramOffset, nameIndex);
            reader.BaseStream.Position = returnPosition;

            parameters.Add(
                new StudioMdlPoseParamDesc
                {
                    NameIndex = nameIndex,
                    Flags = flags,
                    Start = start,
                    End = end,
                    Loop = loop,
                    Name = name,
                }
            );
        }

        return parameters;
    }

    private List<StudioMdlMouth> ParseMouths(
        BinaryReader reader,
        int mouthIndex,
        int mouthCount
    )
    {
        var mouths = new List<StudioMdlMouth>(mouthCount);

        if (mouthCount == 0 || mouthIndex == 0)
            return mouths;

        reader.BaseStream.Position = mouthIndex;

        for (var i = 0; i < mouthCount; i++)
        {
            mouths.Add(
                new StudioMdlMouth
                {
                    Bone = reader.ReadInt32(),
                    ForwardX = reader.ReadSingle(),
                    ForwardY = reader.ReadSingle(),
                    ForwardZ = reader.ReadSingle(),
                    FlexDesc = reader.ReadInt32(),
                }
            );
        }

        return mouths;
    }

    private List<StudioMdlModelGroup> ParseIncludeModels(
        BinaryReader reader,
        int includeModelIndex,
        int includeModelCount
    )
    {
        var groups = new List<StudioMdlModelGroup>(includeModelCount);

        if (includeModelCount == 0 || includeModelIndex == 0)
            return groups;

        reader.BaseStream.Position = includeModelIndex;

        for (var i = 0; i < includeModelCount; i++)
        {
            var groupOffset = reader.BaseStream.Position;

            var labelIndex = reader.ReadInt32();
            var nameIndex = reader.ReadInt32();

            var returnPosition = reader.BaseStream.Position;
            var label = ReadStringAt(reader, groupOffset, labelIndex);
            var name = ReadStringAt(reader, groupOffset, nameIndex);
            reader.BaseStream.Position = returnPosition;

            groups.Add(
                new StudioMdlModelGroup
                {
                    LabelIndex = labelIndex,
                    NameIndex = nameIndex,
                    Label = label,
                    Name = name,
                }
            );
        }

        return groups;
    }

    private List<StudioMdlAnimBlock> ParseAnimBlocks(
        BinaryReader reader,
        int animBlockIndex,
        int animBlockCount
    )
    {
        var blocks = new List<StudioMdlAnimBlock>(animBlockCount);

        if (animBlockCount == 0 || animBlockIndex == 0)
            return blocks;

        reader.BaseStream.Position = animBlockIndex;

        for (var i = 0; i < animBlockCount; i++)
        {
            blocks.Add(
                new StudioMdlAnimBlock
                {
                    DataStart = reader.ReadInt32(),
                    DataEnd = reader.ReadInt32(),
                }
            );
        }

        return blocks;
    }

    private List<StudioMdlAnimDesc> ParseAnimations(
        BinaryReader reader,
        int animationIndex,
        int animationCount
    )
    {
        var anims = new List<StudioMdlAnimDesc>(animationCount);

        if (animationCount == 0 || animationIndex == 0)
            return anims;

        reader.BaseStream.Position = animationIndex;

        for (var i = 0; i < animationCount; i++)
        {
            var animOffset = reader.BaseStream.Position;

            var basePtr = reader.ReadInt32();
            var nameIndex = reader.ReadInt32();
            var fps = reader.ReadSingle();
            var flags = reader.ReadInt32();
            var numFrames = reader.ReadInt32();
            var numMovements = reader.ReadInt32();
            var movementIndex = reader.ReadInt32();

            var unused1 = new int[6]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            var animBlock = reader.ReadInt32();
            var animIndex = reader.ReadInt32();
            var numIkRules = reader.ReadInt32();
            var ikRuleIndex = reader.ReadInt32();
            var animBlockIkRuleIndex = reader.ReadInt32();
            var numLocalHierarchy = reader.ReadInt32();
            var localHierarchyIndex = reader.ReadInt32();
            var sectionIndex = reader.ReadInt32();
            var sectionFrames = reader.ReadInt32();
            var zeroFrameSpan = reader.ReadInt16();
            var zeroFrameCount = reader.ReadInt16();
            var zeroFrameIndex = reader.ReadInt32();
            var zeroFrameStallTime = reader.ReadSingle();

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, animOffset, nameIndex);

            IList<StudioMdlMovement> movements = [];
            if (numMovements > 0 && movementIndex != 0)
            {
                reader.BaseStream.Position = animOffset + movementIndex;
                movements = Enumerable
                    .Range(0, numMovements)
                    .Select(_ => new StudioMdlMovement
                    {
                        EndFrame = reader.ReadInt32(),
                        MotionFlags = reader.ReadInt32(),
                        V0 = reader.ReadSingle(),
                        V1 = reader.ReadSingle(),
                        Angle = reader.ReadSingle(),
                        Vector = BinaryReading.ReadVector3(reader),
                        Position = BinaryReading.ReadVector3(reader),
                    })
                    .ToList();
            }

            IList<StudioMdlAnimSection> sections = [];
            if (sectionFrames != 0 && sectionIndex != 0)
            {
                var sectionCount = (numFrames / sectionFrames) + 2;
                reader.BaseStream.Position = animOffset + sectionIndex;
                sections = Enumerable
                    .Range(0, sectionCount)
                    .Select(_ => new StudioMdlAnimSection
                    {
                        AnimBlock = reader.ReadInt32(),
                        AnimIndex = reader.ReadInt32(),
                    })
                    .ToList();
            }

            IList<StudioMdlIkRule> ikRules = [];
            if (numIkRules > 0 && ikRuleIndex != 0)
            {
                reader.BaseStream.Position = animOffset + ikRuleIndex;
                ikRules = Enumerable
                    .Range(0, numIkRules)
                    .Select(_ => ParseIkRule(reader))
                    .ToList();
            }

            IList<StudioMdlLocalHierarchy> localHierarchy = [];
            if (numLocalHierarchy > 0 && localHierarchyIndex != 0)
            {
                reader.BaseStream.Position = animOffset + localHierarchyIndex;
                localHierarchy = Enumerable
                    .Range(0, numLocalHierarchy)
                    .Select(_ => new StudioMdlLocalHierarchy
                    {
                        Bone = reader.ReadInt32(),
                        NewParent = reader.ReadInt32(),
                        Start = reader.ReadSingle(),
                        Peak = reader.ReadSingle(),
                        Tail = reader.ReadSingle(),
                        End = reader.ReadSingle(),
                        IStart = reader.ReadInt32(),
                        LocalAnimIndex = reader.ReadInt32(),
                        Unused = Enumerable.Range(0, 4).Select(_ => reader.ReadInt32()).ToArray(),
                    })
                    .ToList();
            }

            reader.BaseStream.Position = returnPosition;

            anims.Add(
                new StudioMdlAnimDesc
                {
                    BasePtr = basePtr,
                    NameIndex = nameIndex,
                    Fps = fps,
                    Flags = flags,
                    NumFrames = numFrames,
                    NumMovements = numMovements,
                    MovementIndex = movementIndex,
                    Unused1 = unused1,
                    AnimBlock = animBlock,
                    AnimIndex = animIndex,
                    NumIkRules = numIkRules,
                    IkRuleIndex = ikRuleIndex,
                    AnimBlockIkRuleIndex = animBlockIkRuleIndex,
                    NumLocalHierarchy = numLocalHierarchy,
                    LocalHierarchyIndex = localHierarchyIndex,
                    SectionIndex = sectionIndex,
                    SectionFrames = sectionFrames,
                    ZeroFrameSpan = zeroFrameSpan,
                    ZeroFrameCount = zeroFrameCount,
                    ZeroFrameIndex = zeroFrameIndex,
                    ZeroFrameStallTime = zeroFrameStallTime,
                    Offset = (int)animOffset,
                    Name = name,
                    Movements = movements,
                    Sections = sections,
                    IkRules = ikRules,
                    LocalHierarchy = localHierarchy,
                }
            );
        }

        return anims;
    }

    private StudioMdlIkRule ParseIkRule(BinaryReader reader)
    {
        var ruleOffset = reader.BaseStream.Position;

        return new StudioMdlIkRule
        {
            Index = reader.ReadInt32(),
            Type = reader.ReadInt32(),
            Chain = reader.ReadInt32(),
            Bone = reader.ReadInt32(),
            Slot = reader.ReadInt32(),
            Height = reader.ReadSingle(),
            Radius = reader.ReadSingle(),
            Floor = reader.ReadSingle(),
            Position = BinaryReading.ReadVector3(reader),
            Quaternion = BinaryReading.ReadQuaternion(reader),
            CompressedIkErrorIndex = reader.ReadInt32(),
            Unused2 = reader.ReadInt32(),
            IStart = reader.ReadInt32(),
            IkErrorIndex = reader.ReadInt32(),
            Start = reader.ReadSingle(),
            Peak = reader.ReadSingle(),
            Tail = reader.ReadSingle(),
            End = reader.ReadSingle(),
            Unused3 = reader.ReadSingle(),
            Contact = reader.ReadSingle(),
            Drop = reader.ReadSingle(),
            Top = reader.ReadSingle(),
            Unused6 = reader.ReadInt32(),
            Unused7 = reader.ReadInt32(),
            Unused8 = reader.ReadInt32(),
            AttachmentIndex = reader.ReadInt32(),
            Unused = Enumerable.Range(0, 7).Select(_ => reader.ReadInt32()).ToArray(),
            Attachment = string.Empty,
        };
    }

    private List<StudioMdlSeqDesc> ParseSequences(
        BinaryReader reader,
        int sequenceIndex,
        int sequenceCount
    )
    {
        var seqs = new List<StudioMdlSeqDesc>(sequenceCount);

        if (sequenceCount == 0 || sequenceIndex == 0)
            return seqs;

        reader.BaseStream.Position = sequenceIndex;

        for (var i = 0; i < sequenceCount; i++)
        {
            var seqOffset = reader.BaseStream.Position;

            var basePtr = reader.ReadInt32();
            var labelIndex = reader.ReadInt32();
            var activityNameIndex = reader.ReadInt32();
            var flags = reader.ReadInt32();
            var activity = reader.ReadInt32();
            var actWeight = reader.ReadInt32();
            var numEvents = reader.ReadInt32();
            var eventIndex = reader.ReadInt32();
            var bbMin = BinaryReading.ReadVector3(reader);
            var bbMax = BinaryReading.ReadVector3(reader);
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

            var returnPosition = reader.BaseStream.Position;
            var label = ReadStringAt(reader, seqOffset, labelIndex);
            var activityName = ReadStringAt(reader, seqOffset, activityNameIndex);

            IList<StudioMdlEvent> events = [];
            if (numEvents > 0 && eventIndex != 0)
            {
                reader.BaseStream.Position = seqOffset + eventIndex;
                events = Enumerable
                    .Range(0, numEvents)
                    .Select(_ =>
                    {
                        var eventOffset = reader.BaseStream.Position;
                        var eventCycle = reader.ReadSingle();
                        var eventId = reader.ReadInt32();
                        var eventType = reader.ReadInt32();
                        var options = new string(reader.ReadChars(64)).TrimEnd('\0');
                        var szeventIndex = reader.ReadInt32();
                        var eventReturnPosition = reader.BaseStream.Position;
                        var eventName = ReadStringAt(reader, eventOffset, szeventIndex);
                        reader.BaseStream.Position = eventReturnPosition;
                        return new StudioMdlEvent
                        {
                            Cycle = eventCycle,
                            Event = eventId,
                            Type = eventType,
                            Options = options,
                            EventIndex = szeventIndex,
                            Name = eventName,
                        };
                    })
                    .ToList();
            }

            IList<StudioMdlAutoLayer> autoLayers = [];
            if (numAutoLayers > 0 && autoLayerIndex != 0)
            {
                reader.BaseStream.Position = seqOffset + autoLayerIndex;
                autoLayers = Enumerable
                    .Range(0, numAutoLayers)
                    .Select(_ => new StudioMdlAutoLayer
                    {
                        Sequence = reader.ReadInt16(),
                        Pose = reader.ReadInt16(),
                        Flags = reader.ReadInt32(),
                        Start = reader.ReadSingle(),
                        Peak = reader.ReadSingle(),
                        Tail = reader.ReadSingle(),
                        End = reader.ReadSingle(),
                    })
                    .ToList();
            }

            IList<StudioMdlIkLock> ikLocks = [];
            if (numIkLocks > 0 && ikLockIndex != 0)
            {
                reader.BaseStream.Position = seqOffset + ikLockIndex;
                ikLocks = Enumerable
                    .Range(0, numIkLocks)
                    .Select(_ => new StudioMdlIkLock
                    {
                        Chain = reader.ReadInt32(),
                        PosWeight = reader.ReadSingle(),
                        LocalQWeight = reader.ReadSingle(),
                        Flags = reader.ReadInt32(),
                        Unused = Enumerable.Range(0, 4).Select(_ => reader.ReadInt32()).ToArray(),
                    })
                    .ToList();
            }

            // Blend grid: short[groupsize[1]][groupsize[0]] of animdesc indices
            IList<IList<int>> animIndices = [];
            var width = groupSize[0];
            var height = groupSize[1];
            if (animIndexIndex != 0 && width > 0 && height > 0)
            {
                reader.BaseStream.Position = seqOffset + animIndexIndex;
                for (var y = 0; y < height; y++)
                {
                    var row = new List<int>(width);
                    for (var x = 0; x < width; x++)
                    {
                        row.Add(reader.ReadInt16());
                    }
                    animIndices.Add(row);
                }
            }

            reader.BaseStream.Position = returnPosition;

            seqs.Add(
                new StudioMdlSeqDesc
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
                    Offset = (int)seqOffset,
                    Label = label,
                    ActivityName = activityName,
                    Events = events,
                    AutoLayers = autoLayers,
                    IkLocks = ikLocks,
                    AnimIndices = animIndices,
                }
            );
        }

        return seqs;
    }

    private StudioMdlLinearBone? ParseLinearBones(
        BinaryReader reader,
        int studioHdr2Offset,
        StudioMdlHeader2 header2
    )
    {
        if (header2.LinearBoneIndex == 0)
            return null;

        var linearBoneOffset = studioHdr2Offset + header2.LinearBoneIndex;
        reader.BaseStream.Position = linearBoneOffset;

        var boneCount = reader.ReadInt32();
        var flagsIndex = reader.ReadInt32();
        var parentIndex = reader.ReadInt32();
        var posIndex = reader.ReadInt32();
        var quatIndex = reader.ReadInt32();
        var rotIndex = reader.ReadInt32();
        var poseToBoneIndex = reader.ReadInt32();
        var posScaleIndex = reader.ReadInt32();
        var rotScaleIndex = reader.ReadInt32();
        var qAlignmentIndex = reader.ReadInt32();

        var unused = new int[6]
        {
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
            reader.ReadInt32(),
        };

        return new StudioMdlLinearBone
        {
            BoneCount = boneCount,
            FlagsIndex = flagsIndex,
            ParentIndex = parentIndex,
            PositionIndex = posIndex,
            QuaternionIndex = quatIndex,
            RotationIndex = rotIndex,
            PoseToBoneIndex = poseToBoneIndex,
            PositionScaleIndex = posScaleIndex,
            RotationScaleIndex = rotScaleIndex,
            AlignmentQuaternionIndex = qAlignmentIndex,
            Unused = unused,
        };
    }

    private List<StudioMdlSourceBoneTransform> ParseSourceBoneTransforms(
        BinaryReader reader,
        StudioMdlHeader2? header2
    )
    {
        var transforms = new List<StudioMdlSourceBoneTransform>();

        if (header2 is null || header2.SourceBoneTransformCount == 0)
            return transforms;

        reader.BaseStream.Position = header2.SourceBoneTransformIndex;

        for (var i = 0; i < header2.SourceBoneTransformCount; i++)
        {
            var transformOffset = reader.BaseStream.Position;

            var nameIndex = reader.ReadInt32();
            var preTransform = BinaryReading.ReadMatrix(reader);
            var postTransform = BinaryReading.ReadMatrix(reader);

            var returnPosition = reader.BaseStream.Position;
            var name = ReadStringAt(reader, transformOffset, nameIndex);
            reader.BaseStream.Position = returnPosition;

            transforms.Add(
                new StudioMdlSourceBoneTransform
                {
                    NameIndex = nameIndex,
                    PreTransform = preTransform,
                    PostTransform = postTransform,
                }
            );
        }

        return transforms;
    }

    private List<StudioMdlBoneFlexDriver> ParseBoneFlexDrivers(
        BinaryReader reader,
        int studioHdr2Offset,
        StudioMdlHeader2? header2
    )
    {
        var drivers = new List<StudioMdlBoneFlexDriver>();

        if (header2 is null || header2.BoneFlexDriverCount == 0)
            return drivers;

        var baseOffset = studioHdr2Offset + header2.BoneFlexDriverIndex;
        reader.BaseStream.Position = baseOffset;

        for (var i = 0; i < header2.BoneFlexDriverCount; i++)
        {
            var driverOffset = reader.BaseStream.Position;

            var boneIndex = reader.ReadInt32();
            var controlCount = reader.ReadInt32();
            var controlIndex = reader.ReadInt32();
            var unused = new int[3]
            {
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
            };

            drivers.Add(
                new StudioMdlBoneFlexDriver
                {
                    BoneIndex = boneIndex,
                    ControlCount = controlCount,
                    ControlIndex = controlIndex,
                    Unused = unused,
                }
            );
        }

        return drivers;
    }
}