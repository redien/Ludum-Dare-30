using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable()]
public class StateCollection
{
	[Serializable()]
	class State {
		public enum VisibleTo {
			All,
			One
		}
		
		public bool enabled;
		public int[] statesToEnableOnEnable;
		public int[] statesToDisableOnEnable;
		public VisibleTo visibleTo;
		public int prefab;
	}

	State[] states;
	
	public int Count {
		get {
			return states.Length;
		}
	}
	
	public StateCollection ()
	{
	}
	
	void SetStateInternal(int stateId, bool state) {
		states[stateId].enabled = state;
	}
	
	public delegate void StateChanged(int stateId, bool state);
	[field: NonSerialized]
	public event StateChanged stateChanged;
	
	public void SetState(int stateId, bool state, bool broadcast = true) {
		if (!states[stateId].enabled && state) {
			if (broadcast && stateChanged != null) {
				stateChanged(stateId, state);
			}
			StateWasEnabled(stateId);
		}
		
		if (states[stateId].enabled && !state) {
			if (broadcast && stateChanged != null) {
				stateChanged(stateId, state);
			}
		}

		SetStateInternal(stateId, state);
	}

	public bool GetState(int stateId) {
		return states[stateId].enabled;
	}
	
	void StateWasEnabled(int stateId) {
		if (states[stateId].statesToEnableOnEnable != null) {
			foreach (var stateIdToEnable in states[stateId].statesToEnableOnEnable) {
				SetStateInternal(stateIdToEnable, true);
			}
		}

		if (states[stateId].statesToDisableOnEnable != null) {
			foreach (var stateIdToDisable in states[stateId].statesToDisableOnEnable) {
				SetStateInternal(stateIdToDisable, false);
			}
		}
	}
	
	public int GetPrefabOf(int stateId) {
		return states[stateId].prefab;
	}
	
	public void Generate(int level) {
		states = new State[1];
		
		State state = new State();
		state.visibleTo = State.VisibleTo.All;
		states[0] = state;
	}
	
	public static StateCollection Deserialize(string serialized) {
		BinaryFormatter deserializer = new BinaryFormatter();
		
		using (MemoryStream stream = new MemoryStream()) {
			var converted = Convert.FromBase64String(serialized);
			stream.Write(converted, 0, converted.Length);
			stream.Seek(0, SeekOrigin.Begin);
	
	        return (StateCollection)deserializer.Deserialize(stream);
		}
	}
	
	public static string Serialize(StateCollection stateCollection) {
		using (MemoryStream stream = new MemoryStream()) {
			BinaryFormatter serializer = new BinaryFormatter();
			serializer.Serialize(stream, stateCollection);
			stream.Flush();
			stream.Position = 0;
			return Convert.ToBase64String(stream.ToArray());
		}
	}
}