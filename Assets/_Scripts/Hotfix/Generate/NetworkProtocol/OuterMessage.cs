using ProtoBuf;

using System.Collections.Generic;
using Fantasy;
using Fantasy.Network.Interface;
using Fantasy.Serialize;
#pragma warning disable CS8618

namespace Fantasy
{
	[ProtoContract]
	public partial class Vector3 : AMessage, IMessage, IProto
	{
		public static Vector3 Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Vector3>();
		}
		public override void Dispose()
		{
			x = default;
			y = default;
			z = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Vector3>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.Vector3; }
		[ProtoMember(1)]
		public float x { get; set; }
		[ProtoMember(2)]
		public float y { get; set; }
		[ProtoMember(3)]
		public float z { get; set; }
	}
	[ProtoContract]
	public partial class Hand : AMessage, IMessage, IProto
	{
		public static Hand Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Hand>();
		}
		public override void Dispose()
		{
			tag = default;
			position = default;
			rotation = default;
			scale = default;
			isHide = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Hand>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.Hand; }
		[ProtoMember(1)]
		public string tag { get; set; }
		[ProtoMember(2)]
		public Vector3 position { get; set; }
		[ProtoMember(3)]
		public Vector3 rotation { get; set; }
		[ProtoMember(4)]
		public Vector3 scale { get; set; }
		[ProtoMember(5)]
		public bool isHide { get; set; }
	}
	[ProtoContract]
	public partial class Controller : AMessage, IMessage, IProto
	{
		public static Controller Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Controller>();
		}
		public override void Dispose()
		{
			tag = default;
			position = default;
			rotation = default;
			scale = default;
			isHide = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Controller>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.Controller; }
		[ProtoMember(1)]
		public string tag { get; set; }
		[ProtoMember(2)]
		public Vector3 position { get; set; }
		[ProtoMember(3)]
		public Vector3 rotation { get; set; }
		[ProtoMember(4)]
		public Vector3 scale { get; set; }
		[ProtoMember(5)]
		public bool isHide { get; set; }
	}
	[ProtoContract]
	public partial class RegistryClientRequest : AMessage, IRequest, IProto
	{
		public static RegistryClientRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RegistryClientRequest>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RegistryClientRequest>(this);
#endif
		}
		[ProtoIgnore]
		public RegistryClientResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.RegistryClientRequest; }
	}
	[ProtoContract]
	public partial class RegistryClientResponse : AMessage, IResponse, IProto
	{
		public static RegistryClientResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RegistryClientResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			socketClinetUUID = default;
			currentIndex = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RegistryClientResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.RegistryClientResponse; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public int currentIndex { get; set; }
		[ProtoMember(3)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class PushRegistryMessage : AMessage, IMessage, IProto
	{
		public static PushRegistryMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushRegistryMessage>();
		}
		public override void Dispose()
		{
			playerIndex = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushRegistryMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushRegistryMessage; }
		[ProtoMember(1)]
		public int playerIndex { get; set; }
	}
	[ProtoContract]
	public partial class RemoveClientRequest : AMessage, IRequest, IProto
	{
		public static RemoveClientRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RemoveClientRequest>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RemoveClientRequest>(this);
#endif
		}
		[ProtoIgnore]
		public RemoveClientResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.RemoveClientRequest; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class RemoveClientResponse : AMessage, IResponse, IProto
	{
		public static RemoveClientResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RemoveClientResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			is_success = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RemoveClientResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.RemoveClientResponse; }
		[ProtoMember(1)]
		public bool is_success { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class PushRemoveClientMessage : AMessage, IMessage, IProto
	{
		public static PushRemoveClientMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushRemoveClientMessage>();
		}
		public override void Dispose()
		{
			playerIndex = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushRemoveClientMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushRemoveClientMessage; }
		[ProtoMember(1)]
		public int playerIndex { get; set; }
	}
	[ProtoContract]
	public partial class GetAllClientRequest : AMessage, IRequest, IProto
	{
		public static GetAllClientRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAllClientRequest>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAllClientRequest>(this);
#endif
		}
		[ProtoIgnore]
		public GetAllClientResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.GetAllClientRequest; }
	}
	[ProtoContract]
	public partial class GetAllClientResponse : AMessage, IResponse, IProto
	{
		public static GetAllClientResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAllClientResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			playerIndexList.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAllClientResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.GetAllClientResponse; }
		[ProtoMember(1)]
		public List<int> playerIndexList = new List<int>();
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class PlayerState : AMessage, IMessage, IProto
	{
		public static PlayerState Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PlayerState>();
		}
		public override void Dispose()
		{
			position = default;
			rotation = default;
			scale = default;
			leftHand = default;
			rightHand = default;
			leftController = default;
			rightController = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PlayerState>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PlayerState; }
		[ProtoMember(1)]
		public Vector3 position { get; set; }
		[ProtoMember(2)]
		public Vector3 rotation { get; set; }
		[ProtoMember(3)]
		public Vector3 scale { get; set; }
		[ProtoMember(4)]
		public Hand leftHand { get; set; }
		[ProtoMember(5)]
		public Hand rightHand { get; set; }
		[ProtoMember(6)]
		public Controller leftController { get; set; }
		[ProtoMember(7)]
		public Controller rightController { get; set; }
	}
	[ProtoContract]
	public partial class SubmitPlayerStateMessage : AMessage, IMessage, IProto
	{
		public static SubmitPlayerStateMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitPlayerStateMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			uuid = default;
			playerState = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitPlayerStateMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitPlayerStateMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public int uuid { get; set; }
		[ProtoMember(3)]
		public PlayerState playerState { get; set; }
	}
	[ProtoContract]
	public partial class PushPlayerStateResponseMessage : AMessage, IMessage, IProto
	{
		public static PushPlayerStateResponseMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushPlayerStateResponseMessage>();
		}
		public override void Dispose()
		{
			uuid = default;
			playerState = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushPlayerStateResponseMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushPlayerStateResponseMessage; }
		[ProtoMember(1)]
		public int uuid { get; set; }
		[ProtoMember(2)]
		public PlayerState playerState { get; set; }
	}
	[ProtoContract]
	public partial class SubmitAutoTransformSynchronizationMessage : AMessage, IMessage, IProto
	{
		public static SubmitAutoTransformSynchronizationMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitAutoTransformSynchronizationMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			uuid = default;
			position = default;
			rotation = default;
			scale = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitAutoTransformSynchronizationMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitAutoTransformSynchronizationMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public int uuid { get; set; }
		[ProtoMember(3)]
		public Vector3 position { get; set; }
		[ProtoMember(4)]
		public Vector3 rotation { get; set; }
		[ProtoMember(5)]
		public Vector3 scale { get; set; }
	}
	[ProtoContract]
	public partial class PushAutoTransformSynchronizationMessage : AMessage, IMessage, IProto
	{
		public static PushAutoTransformSynchronizationMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushAutoTransformSynchronizationMessage>();
		}
		public override void Dispose()
		{
			uuid = default;
			position = default;
			rotation = default;
			scale = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushAutoTransformSynchronizationMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushAutoTransformSynchronizationMessage; }
		[ProtoMember(1)]
		public int uuid { get; set; }
		[ProtoMember(2)]
		public Vector3 position { get; set; }
		[ProtoMember(3)]
		public Vector3 rotation { get; set; }
		[ProtoMember(4)]
		public Vector3 scale { get; set; }
	}
	[ProtoContract]
	public partial class SubmitAutoTransformSynchronizationOwnerMessage : AMessage, IMessage, IProto
	{
		public static SubmitAutoTransformSynchronizationOwnerMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitAutoTransformSynchronizationOwnerMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			uuid = default;
			playerIndex = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitAutoTransformSynchronizationOwnerMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitAutoTransformSynchronizationOwnerMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public int uuid { get; set; }
		[ProtoMember(3)]
		public int playerIndex { get; set; }
	}
	[ProtoContract]
	public partial class PushAutoTransformSynchronizationOwnerMessage : AMessage, IMessage, IProto
	{
		public static PushAutoTransformSynchronizationOwnerMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushAutoTransformSynchronizationOwnerMessage>();
		}
		public override void Dispose()
		{
			uuid = default;
			playerIndex = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushAutoTransformSynchronizationOwnerMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushAutoTransformSynchronizationOwnerMessage; }
		[ProtoMember(1)]
		public int uuid { get; set; }
		[ProtoMember(2)]
		public int playerIndex { get; set; }
	}
	[ProtoContract]
	public partial class GetAutoTransformSynchronizationOwnerRequest : AMessage, IRequest, IProto
	{
		public static GetAutoTransformSynchronizationOwnerRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAutoTransformSynchronizationOwnerRequest>();
		}
		public override void Dispose()
		{
			uuid = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAutoTransformSynchronizationOwnerRequest>(this);
#endif
		}
		[ProtoIgnore]
		public GetAutoTransformSynchronizationOwnerResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.GetAutoTransformSynchronizationOwnerRequest; }
		[ProtoMember(1)]
		public int uuid { get; set; }
	}
	[ProtoContract]
	public partial class GetAutoTransformSynchronizationOwnerResponse : AMessage, IResponse, IProto
	{
		public static GetAutoTransformSynchronizationOwnerResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAutoTransformSynchronizationOwnerResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			playerIndex = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAutoTransformSynchronizationOwnerResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.GetAutoTransformSynchronizationOwnerResponse; }
		[ProtoMember(1)]
		public int playerIndex { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class SubmitNetRigidbodySynchronizationMessage : AMessage, IMessage, IProto
	{
		public static SubmitNetRigidbodySynchronizationMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitNetRigidbodySynchronizationMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			uuid = default;
			velocity = default;
			angular_velocity = default;
			isKinematic = default;
			mass = default;
			angularDrag = default;
			freeze_position = default;
			freeze_rotation = default;
			useGravity = default;
			drag = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitNetRigidbodySynchronizationMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitNetRigidbodySynchronizationMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public int uuid { get; set; }
		[ProtoMember(3)]
		public Vector3 velocity { get; set; }
		[ProtoMember(4)]
		public Vector3 angular_velocity { get; set; }
		[ProtoMember(5)]
		public bool isKinematic { get; set; }
		[ProtoMember(6)]
		public float mass { get; set; }
		[ProtoMember(7)]
		public float angularDrag { get; set; }
		[ProtoMember(8)]
		public Vector3 freeze_position { get; set; }
		[ProtoMember(9)]
		public Vector3 freeze_rotation { get; set; }
		[ProtoMember(10)]
		public bool useGravity { get; set; }
		[ProtoMember(11)]
		public float drag { get; set; }
	}
	[ProtoContract]
	public partial class PushNetRigidbodySynchronizationMessage : AMessage, IMessage, IProto
	{
		public static PushNetRigidbodySynchronizationMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushNetRigidbodySynchronizationMessage>();
		}
		public override void Dispose()
		{
			uuid = default;
			velocity = default;
			angular_velocity = default;
			isKinematic = default;
			mass = default;
			angularDrag = default;
			freeze_position = default;
			freeze_rotation = default;
			useGravity = default;
			drag = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushNetRigidbodySynchronizationMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushNetRigidbodySynchronizationMessage; }
		[ProtoMember(1)]
		public int uuid { get; set; }
		[ProtoMember(2)]
		public Vector3 velocity { get; set; }
		[ProtoMember(3)]
		public Vector3 angular_velocity { get; set; }
		[ProtoMember(4)]
		public bool isKinematic { get; set; }
		[ProtoMember(5)]
		public float mass { get; set; }
		[ProtoMember(6)]
		public float angularDrag { get; set; }
		[ProtoMember(7)]
		public Vector3 freeze_position { get; set; }
		[ProtoMember(8)]
		public Vector3 freeze_rotation { get; set; }
		[ProtoMember(9)]
		public bool useGravity { get; set; }
		[ProtoMember(10)]
		public float drag { get; set; }
	}
	[ProtoContract]
	public partial class GetAllSubmitAutoTransformSynchronizationRequest : AMessage, IRequest, IProto
	{
		public static GetAllSubmitAutoTransformSynchronizationRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAllSubmitAutoTransformSynchronizationRequest>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAllSubmitAutoTransformSynchronizationRequest>(this);
#endif
		}
		[ProtoIgnore]
		public GetAllSubmitAutoTransformSynchronizationResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.GetAllSubmitAutoTransformSynchronizationRequest; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class GetAllSubmitAutoTransformSynchronizationResponse : AMessage, IResponse, IProto
	{
		public static GetAllSubmitAutoTransformSynchronizationResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAllSubmitAutoTransformSynchronizationResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			messageList.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAllSubmitAutoTransformSynchronizationResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.GetAllSubmitAutoTransformSynchronizationResponse; }
		[ProtoMember(1)]
		public List<SubmitAutoTransformSynchronizationMessage> messageList = new List<SubmitAutoTransformSynchronizationMessage>();
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class SubmitClientJoin2MultipleGameMessage : AMessage, IMessage, IProto
	{
		public static SubmitClientJoin2MultipleGameMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitClientJoin2MultipleGameMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitClientJoin2MultipleGameMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitClientJoin2MultipleGameMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class SubmitClientExit2MultipleGameMessage : AMessage, IMessage, IProto
	{
		public static SubmitClientExit2MultipleGameMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitClientExit2MultipleGameMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitClientExit2MultipleGameMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitClientExit2MultipleGameMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class CreateLivePreviewRouteMessage : AMessage, IMessage, IProto
	{
		public static CreateLivePreviewRouteMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<CreateLivePreviewRouteMessage>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<CreateLivePreviewRouteMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.CreateLivePreviewRouteMessage; }
	}
	[ProtoContract]
	public partial class SubmitGamePreviewMessage : AMessage, ICustomRouteMessage, IProto
	{
		public static SubmitGamePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<SubmitGamePreviewMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			preview_image_data = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<SubmitGamePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.SubmitGamePreviewMessage; }
		[ProtoIgnore]
		public int RouteType => Fantasy.RouteType.LivePreviewRoute;
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public byte[] preview_image_data { get; set; }
	}
	[ProtoContract]
	public partial class PushGamePreviewMessage : AMessage, IMessage, IProto
	{
		public static PushGamePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushGamePreviewMessage>();
		}
		public override void Dispose()
		{
			preview_image_data = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushGamePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushGamePreviewMessage; }
		[ProtoMember(1)]
		public byte[] preview_image_data { get; set; }
	}
	[ProtoContract]
	public partial class PushStartGamePreviewMessage : AMessage, IMessage, IProto
	{
		public static PushStartGamePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushStartGamePreviewMessage>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushStartGamePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushStartGamePreviewMessage; }
	}
	[ProtoContract]
	public partial class PushStopGamePreviewMessage : AMessage, IMessage, IProto
	{
		public static PushStopGamePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushStopGamePreviewMessage>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushStopGamePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushStopGamePreviewMessage; }
	}
	[ProtoContract]
	public partial class RegistryTeacherRequest : AMessage, IRequest, IProto
	{
		public static RegistryTeacherRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RegistryTeacherRequest>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RegistryTeacherRequest>(this);
#endif
		}
		[ProtoIgnore]
		public RegistryTeacherResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.RegistryTeacherRequest; }
	}
	[ProtoContract]
	public partial class RegistryTeacherResponse : AMessage, IResponse, IProto
	{
		public static RegistryTeacherResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RegistryTeacherResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			socket_teacher_UUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RegistryTeacherResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.RegistryTeacherResponse; }
		[ProtoMember(1)]
		public string socket_teacher_UUID { get; set; }
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class RemoveTeacherMessage : AMessage, IMessage, IProto
	{
		public static RemoveTeacherMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RemoveTeacherMessage>();
		}
		public override void Dispose()
		{
			socket_teacher_UUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RemoveTeacherMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.RemoveTeacherMessage; }
		[ProtoMember(1)]
		public string socket_teacher_UUID { get; set; }
	}
	[ProtoContract]
	public partial class GetAllPlayerRequest : AMessage, IRequest, IProto
	{
		public static GetAllPlayerRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAllPlayerRequest>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAllPlayerRequest>(this);
#endif
		}
		[ProtoIgnore]
		public GetAllPlayerResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.GetAllPlayerRequest; }
	}
	[ProtoContract]
	public partial class GetAllPlayerResponse : AMessage, IResponse, IProto
	{
		public static GetAllPlayerResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<GetAllPlayerResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			SocketClinetUUIDList.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<GetAllPlayerResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.GetAllPlayerResponse; }
		[ProtoMember(1)]
		public List<string> SocketClinetUUIDList = new List<string>();
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class PushTeacherRegistryPlayerMessage : AMessage, IMessage, IProto
	{
		public static PushTeacherRegistryPlayerMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushTeacherRegistryPlayerMessage>();
		}
		public override void Dispose()
		{
			SocketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushTeacherRegistryPlayerMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushTeacherRegistryPlayerMessage; }
		[ProtoMember(1)]
		public string SocketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class PushTeacherRemovePlayerMessage : AMessage, IMessage, IProto
	{
		public static PushTeacherRemovePlayerMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushTeacherRemovePlayerMessage>();
		}
		public override void Dispose()
		{
			SocketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushTeacherRemovePlayerMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushTeacherRemovePlayerMessage; }
		[ProtoMember(1)]
		public string SocketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class TeacherSubmitStartGamePreviewMessage : AMessage, IMessage, IProto
	{
		public static TeacherSubmitStartGamePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<TeacherSubmitStartGamePreviewMessage>();
		}
		public override void Dispose()
		{
			socket_teacher_UUID = default;
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<TeacherSubmitStartGamePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.TeacherSubmitStartGamePreviewMessage; }
		[ProtoMember(1)]
		public string socket_teacher_UUID { get; set; }
		[ProtoMember(2)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class TeacherSubmitStopGamePreviewMessage : AMessage, IMessage, IProto
	{
		public static TeacherSubmitStopGamePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<TeacherSubmitStopGamePreviewMessage>();
		}
		public override void Dispose()
		{
			socket_teacher_UUID = default;
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<TeacherSubmitStopGamePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.TeacherSubmitStopGamePreviewMessage; }
		[ProtoMember(1)]
		public string socket_teacher_UUID { get; set; }
		[ProtoMember(2)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class TeacherChangeListenPlayerMessage : AMessage, IMessage, IProto
	{
		public static TeacherChangeListenPlayerMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<TeacherChangeListenPlayerMessage>();
		}
		public override void Dispose()
		{
			socket_teacher_UUID = default;
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<TeacherChangeListenPlayerMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.TeacherChangeListenPlayerMessage; }
		[ProtoMember(1)]
		public string socket_teacher_UUID { get; set; }
		[ProtoMember(2)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class ClientRegistryLivePreviewMessage : AMessage, IMessage, IProto
	{
		public static ClientRegistryLivePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientRegistryLivePreviewMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientRegistryLivePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientRegistryLivePreviewMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class ClientRemoveLivePreviewMessage : AMessage, IMessage, IProto
	{
		public static ClientRemoveLivePreviewMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientRemoveLivePreviewMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientRemoveLivePreviewMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientRemoveLivePreviewMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
	}
	[ProtoContract]
	public partial class LoginRequest : AMessage, IRequest, IProto
	{
		public static LoginRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<LoginRequest>();
		}
		public override void Dispose()
		{
			user_name = default;
			password = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<LoginRequest>(this);
#endif
		}
		[ProtoIgnore]
		public LoginResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.LoginRequest; }
		[ProtoMember(1)]
		public string user_name { get; set; }
		[ProtoMember(2)]
		public string password { get; set; }
	}
	[ProtoContract]
	public partial class LoginResponse : AMessage, IResponse, IProto
	{
		public static LoginResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<LoginResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			status = default;
			token = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<LoginResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.LoginResponse; }
		[ProtoMember(1)]
		public int status { get; set; }
		[ProtoMember(2)]
		public string token { get; set; }
		[ProtoMember(3)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class RegistryTeacherAccountRequest : AMessage, IRequest, IProto
	{
		public static RegistryTeacherAccountRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RegistryTeacherAccountRequest>();
		}
		public override void Dispose()
		{
			user_name = default;
			password = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RegistryTeacherAccountRequest>(this);
#endif
		}
		[ProtoIgnore]
		public RegistryTeacherAccountResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.RegistryTeacherAccountRequest; }
		[ProtoMember(1)]
		public string user_name { get; set; }
		[ProtoMember(2)]
		public string password { get; set; }
	}
	[ProtoContract]
	public partial class RegistryTeacherAccountResponse : AMessage, IResponse, IProto
	{
		public static RegistryTeacherAccountResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RegistryTeacherAccountResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			status = default;
			data = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RegistryTeacherAccountResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.RegistryTeacherAccountResponse; }
		[ProtoMember(1)]
		public int status { get; set; }
		[ProtoMember(2)]
		public string data { get; set; }
		[ProtoMember(3)]
		public uint ErrorCode { get; set; }
	}
	[ProtoContract]
	public partial class TeacherChangeSceneMessage : AMessage, IMessage, IProto
	{
		public static TeacherChangeSceneMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<TeacherChangeSceneMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			scene_name = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<TeacherChangeSceneMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.TeacherChangeSceneMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public string scene_name { get; set; }
	}
	[ProtoContract]
	public partial class PushChangeSceneMessage : AMessage, IMessage, IProto
	{
		public static PushChangeSceneMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushChangeSceneMessage>();
		}
		public override void Dispose()
		{
			scene_name = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushChangeSceneMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushChangeSceneMessage; }
		[ProtoMember(1)]
		public string scene_name { get; set; }
	}
	[ProtoContract]
	public partial class TeacherSendHintMessage : AMessage, IMessage, IProto
	{
		public static TeacherSendHintMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<TeacherSendHintMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			hint_message = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<TeacherSendHintMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.TeacherSendHintMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public string hint_message { get; set; }
	}
	[ProtoContract]
	public partial class PushSendHintMessage : AMessage, IMessage, IProto
	{
		public static PushSendHintMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<PushSendHintMessage>();
		}
		public override void Dispose()
		{
			hint_message = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<PushSendHintMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.PushSendHintMessage; }
		[ProtoMember(1)]
		public string hint_message { get; set; }
	}
	[ProtoContract]
	public partial class ClientEnterMultipleSceneMessage : AMessage, IMessage, IProto
	{
		public static ClientEnterMultipleSceneMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientEnterMultipleSceneMessage>();
		}
		public override void Dispose()
		{
			player_name = default;
			scene_name = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientEnterMultipleSceneMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientEnterMultipleSceneMessage; }
		[ProtoMember(1)]
		public string player_name { get; set; }
		[ProtoMember(2)]
		public string scene_name { get; set; }
	}
	[ProtoContract]
	public partial class RoomClientItem : AMessage, IMessage, IProto
	{
		public static RoomClientItem Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<RoomClientItem>();
		}
		public override void Dispose()
		{
			player_index = default;
			player_pos = default;
			player_name = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<RoomClientItem>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.RoomClientItem; }
		[ProtoMember(1)]
		public int player_index { get; set; }
		[ProtoMember(2)]
		public Vector3 player_pos { get; set; }
		[ProtoMember(3)]
		public string player_name { get; set; }
	}
	[ProtoContract]
	public partial class ServerPushRoomClientMessage : AMessage, IMessage, IProto
	{
		public static ServerPushRoomClientMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ServerPushRoomClientMessage>();
		}
		public override void Dispose()
		{
			room_client_list.Clear();
			current_player_index = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ServerPushRoomClientMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ServerPushRoomClientMessage; }
		[ProtoMember(1)]
		public List<RoomClientItem> room_client_list = new List<RoomClientItem>();
		[ProtoMember(2)]
		public int current_player_index { get; set; }
	}
	[ProtoContract]
	public partial class ClientExitMultipleSceneMessage : AMessage, IMessage, IProto
	{
		public static ClientExitMultipleSceneMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientExitMultipleSceneMessage>();
		}
		public override void Dispose()
		{
			scene_name = default;
			player_index = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientExitMultipleSceneMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientExitMultipleSceneMessage; }
		[ProtoMember(1)]
		public string scene_name { get; set; }
		[ProtoMember(2)]
		public string player_index { get; set; }
	}
	[ProtoContract]
	public partial class ServerPushClientEnterMultipleSceneMessage : AMessage, IMessage, IProto
	{
		public static ServerPushClientEnterMultipleSceneMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ServerPushClientEnterMultipleSceneMessage>();
		}
		public override void Dispose()
		{
			add_name = default;
			player_index = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ServerPushClientEnterMultipleSceneMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ServerPushClientEnterMultipleSceneMessage; }
		[ProtoMember(1)]
		public string add_name { get; set; }
		[ProtoMember(2)]
		public int player_index { get; set; }
	}
	[ProtoContract]
	public partial class ServerPushClientExitMultipleSceneMessage : AMessage, IMessage, IProto
	{
		public static ServerPushClientExitMultipleSceneMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ServerPushClientExitMultipleSceneMessage>();
		}
		public override void Dispose()
		{
			player_index = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ServerPushClientExitMultipleSceneMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ServerPushClientExitMultipleSceneMessage; }
		[ProtoMember(1)]
		public int player_index { get; set; }
	}
	[ProtoContract]
	public partial class ClientCreateLineMessage : AMessage, IMessage, IProto
	{
		public static ClientCreateLineMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientCreateLineMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			line_guid = default;
			lineWidth = default;
			color = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientCreateLineMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientCreateLineMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public string line_guid { get; set; }
		[ProtoMember(3)]
		public float lineWidth { get; set; }
		[ProtoMember(4)]
		public Vector3 color { get; set; }
	}
	[ProtoContract]
	public partial class ClientAddLinePointMessage : AMessage, IMessage, IProto
	{
		public static ClientAddLinePointMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientAddLinePointMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			line_guid = default;
			pos_index = default;
			pos = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientAddLinePointMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientAddLinePointMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public string line_guid { get; set; }
		[ProtoMember(3)]
		public int pos_index { get; set; }
		[ProtoMember(4)]
		public Vector3 pos { get; set; }
	}
	[ProtoContract]
	public partial class ClientRemoveLineMessage : AMessage, IMessage, IProto
	{
		public static ClientRemoveLineMessage Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientRemoveLineMessage>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			line_guid = default;
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientRemoveLineMessage>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientRemoveLineMessage; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public string line_guid { get; set; }
	}
	[ProtoContract]
	public partial class ClientGetAllLineRequest : AMessage, IRequest, IProto
	{
		public static ClientGetAllLineRequest Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientGetAllLineRequest>();
		}
		public override void Dispose()
		{
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientGetAllLineRequest>(this);
#endif
		}
		[ProtoIgnore]
		public ClientGetAllLineResponse ResponseType { get; set; }
		public uint OpCode() { return OuterOpcode.ClientGetAllLineRequest; }
	}
	[ProtoContract]
	public partial class Line : AMessage, IMessage, IProto
	{
		public static Line Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<Line>();
		}
		public override void Dispose()
		{
			socketClinetUUID = default;
			line_guid = default;
			lineWidth = default;
			color = default;
			pos_List.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<Line>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.Line; }
		[ProtoMember(1)]
		public string socketClinetUUID { get; set; }
		[ProtoMember(2)]
		public string line_guid { get; set; }
		[ProtoMember(3)]
		public float lineWidth { get; set; }
		[ProtoMember(4)]
		public Vector3 color { get; set; }
		[ProtoMember(5)]
		public List<Vector3> pos_List = new List<Vector3>();
	}
	[ProtoContract]
	public partial class ClientGetAllLineResponse : AMessage, IResponse, IProto
	{
		public static ClientGetAllLineResponse Create(Scene scene)
		{
			return scene.MessagePoolComponent.Rent<ClientGetAllLineResponse>();
		}
		public override void Dispose()
		{
			ErrorCode = default;
			line_list.Clear();
#if FANTASY_NET || FANTASY_UNITY
			GetScene().MessagePoolComponent.Return<ClientGetAllLineResponse>(this);
#endif
		}
		public uint OpCode() { return OuterOpcode.ClientGetAllLineResponse; }
		[ProtoMember(1)]
		public List<Line> line_list = new List<Line>();
		[ProtoMember(2)]
		public uint ErrorCode { get; set; }
	}
}
